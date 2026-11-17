using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
    }
}