using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace InstallerHost
{
    /// <summary>
    /// Archive appended to the setup executable by RetroBuild.
    /// File layout: InstallerHost.exe + archive + footer (archive length: 8 bytes, archive sha256: 32 bytes, signature: 8 bytes).
    /// </summary>
    static class SetupPayload
    {
        private const string Signature = "RBSETUP2";
        private const int FooterSize = 48;

        // The footer is searched in the last bytes of the file only (a code signing certificate can follow it)
        private const int SearchSize = 1024 * 1024;

        private static byte[] _hash;

        public static long Start { get; private set; }
        public static long Length { get; private set; }
        
        /// <summary>
        /// True if the archive is a 7z file, false if it is a zip file.
        /// </summary>
        public static bool Is7z { get; private set; }

        /// <summary>
        /// Locates the archive. Returns false if the setup file is incomplete or damaged.
        /// </summary>
        public static bool Find()
        {
            try
            {
                using (var fs = new FileStream(Application.ExecutablePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] tail = new byte[(int)Math.Min(fs.Length, SearchSize)];
                    long tailStart = fs.Length - tail.Length;

                    fs.Seek(tailStart, SeekOrigin.Begin);
                    for (int done = 0; done < tail.Length;)
                    {
                        int read = fs.Read(tail, done, tail.Length - done);
                        if (read <= 0)
                            return false;

                        done += read;
                    }

                    byte[] signature = Encoding.ASCII.GetBytes(Signature);

                    // Search backwards for the signature that ends the footer
                    for (int footer = tail.Length - FooterSize; footer >= 0; footer--)
                    {
                        if (!Matches(tail, footer + FooterSize - signature.Length, signature))
                            continue;

                        long length = BitConverter.ToInt64(tail, footer);
                        long start = tailStart + footer - length;
                        if (length <= 0 || start <= 0)
                            continue;

                        _hash = new byte[32];
                        Array.Copy(tail, footer + 8, _hash, 0, _hash.Length);
                        Start = start;
                        Length = length;

                        // A 7z file starts with '7' 'z' BC AF 27 1C
                        byte[] head = new byte[6];
                        fs.Seek(start, SeekOrigin.Begin);
                        fs.Read(head, 0, head.Length);
                        Is7z = head[0] == 0x37 && head[1] == 0x7A && head[2] == 0xBC && head[3] == 0xAF && head[4] == 0x27 && head[5] == 0x1C;

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("[ERROR] Unable to read the setup file: " + ex.Message);
            }

            return false;
        }

        private static bool Matches(byte[] data, int offset, byte[] value)
        {
            for (int i = 0; i < value.Length; i++)
                if (data[offset + i] != value[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Checks the archive against the sha256 stored in the footer. Progress is reported from 0 to 100.
        /// </summary>
        public static bool Verify(Action<int> reportProgress)
        {
            using (var fs = new FileStream(Application.ExecutablePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sha = new SHA256CryptoServiceProvider())
            {
                byte[] buffer = new byte[1024 * 1024];
                long remaining = Length;
                int lastPercent = -1;

                fs.Seek(Start, SeekOrigin.Begin);
                while (remaining > 0)
                {
                    int read = fs.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));
                    if (read <= 0)
                        return false;

                    sha.TransformBlock(buffer, 0, read, null, 0);
                    remaining -= read;

                    int percent = (int)((Length - remaining) * 100 / Length);
                    if (percent != lastPercent)
                    {
                        reportProgress?.Invoke(percent);
                        lastPercent = percent;
                    }
                }

                sha.TransformFinalBlock(buffer, 0, 0);
                return sha.Hash.SequenceEqual(_hash);
            }
        }

        /// <summary>
        /// Extracts a 7z archive with the embedded 7za.exe. Progress is reported from 0 to 100.
        /// </summary>
        public static void Extract7z(string destinationFolder, Action<int> reportProgress)
        {
            // Copied to the installation folder, not to the temp folder where running an exe is more often blocked
            string sevenZipPath = Path.Combine(destinationFolder, "RetroBat_7za_" + Guid.NewGuid().ToString("N") + ".exe");

            using (var resource = typeof(SetupPayload).Assembly.GetManifestResourceStream("InstallerHost.resources.7za.exe"))
            {
                if (resource == null)
                    throw new Exception("7za.exe resource not found.");

                using (var file = new FileStream(sevenZipPath, FileMode.Create, FileAccess.Write))
                    resource.CopyTo(file);
            }

            var errors = new StringBuilder();
            var percentRegex = new Regex(@"(\d+)%");
            int lastPercent = -1;

            var startInfo = new ProcessStartInfo
            {
                FileName = sevenZipPath,
                // 7-Zip finds the archive after InstallerHost.exe: "s64m" is how far it searches for it (8 MB by default, not enough)
                Arguments = "x -t7z:s64m \"" + Application.ExecutablePath + "\" -y -aoa -bso0 -bsp1",
                // Files are extracted to the working directory: no destination path to quote
                WorkingDirectory = destinationFolder,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                // Do not leave 7-Zip running if the installer is closed during the extraction
                EventHandler killOnExit = (s, e) =>
                {
                    try { process.Kill(); process.WaitForExit(2000); File.Delete(sevenZipPath); } catch { }
                };

                AppDomain.CurrentDomain.ProcessExit += killOnExit;
                try
                {
                    process.ErrorDataReceived += (s, e) => { if (e.Data != null) lock (errors) errors.AppendLine(e.Data); };
                    process.Start();
                    process.BeginErrorReadLine();

                    // The progress is written without new lines: read raw characters and keep the last percentage
                    char[] buffer = new char[256];
                    string pending = "";
                    int read;

                    while ((read = process.StandardOutput.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        pending += new string(buffer, 0, read);

                        var matches = percentRegex.Matches(pending);
                        if (matches.Count > 0)
                        {
                            int percent;
                            if (int.TryParse(matches[matches.Count - 1].Groups[1].Value, out percent) && percent <= 100 && percent != lastPercent)
                            {
                                reportProgress?.Invoke(percent);
                                lastPercent = percent;
                            }
                        }

                        if (pending.Length > 64)
                            pending = pending.Substring(pending.Length - 16);
                    }

                    process.WaitForExit();

                    // 7-Zip exit codes: 0 = success, 1 = warning, 2 or more = error
                    if (process.ExitCode == 1)
                        Logger.Log("[WARNING] 7-Zip warnings: " + errors);
                    else if (process.ExitCode != 0)
                        throw new Exception("7-Zip (code " + process.ExitCode + ") " + errors);

                    reportProgress?.Invoke(100);
                }
                finally
                {
                    AppDomain.CurrentDomain.ProcessExit -= killOnExit;
                    try { File.Delete(sevenZipPath); } catch { }
                }
            }
        }
    }
}