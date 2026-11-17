using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RetroBuild
{
    class Installer
    {
        public static void CreateInstaller(BuilderOptions options)
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string buildPath = rootPath;

            // Archive embedded in the setup: 7z (smaller) or the zip that is also used for updates
            bool use7z = options.InstallerFormat == "7z";
            string archiveName = $"retrobat-v{options.RetrobatVersion}-{options.Branch}-{options.Architecture}" + (use7z ? ".7z" : ".zip");
            string archivePath = Path.Combine(rootPath, archiveName);

            string installerHostExePath = Path.Combine(rootPath, "InstallerHost.exe");

            if (!File.Exists(installerHostExePath))
            {
                Logger.Log("[ERROR] InstallerHost.exe not found at: " + installerHostExePath);
                return;
            }
            Logger.LogInfo("Found InstallerHost.exe at: " + installerHostExePath);

            if (use7z)
            {
                // Always rebuilt and deleted afterwards: the setup never embeds the archive of a previous build
                if (!Program.Create7zArchive(options, archivePath))
                    return;
            }
            else if (!File.Exists(archivePath))
            {
                Logger.Log("[INFO] zip not found, creating zip first.");
                try { Program.CreateZipFolderSharpZip(options); }
                catch (Exception ex)
                {
                    Logger.Log("[ERROR] Exception creating ZIP: " + ex.Message);
                }
            }

            if (!File.Exists(archivePath))
            {
                Logger.Log("[ERROR] No archive found at: " + archivePath);
                return;
            }
            Logger.LogInfo("Found archive: " + archivePath);

            try
            {
                // Output final installer executable RetroBat-v7.3.0-stable-win64-setup
                string setupName = "RetroBat-v" + options.RetrobatVersion + "-" + options.Branch + "-" + options.Architecture + "-setup.exe";
                string finalInstallerPath = Path.Combine(buildPath, setupName);

                using (var outputStream = new FileStream(finalInstallerPath, FileMode.Create, FileAccess.Write))
                {
                    // Write InstallerHost.exe
                    using (var installerStream = new FileStream(installerHostExePath, FileMode.Open, FileAccess.Read))
                    {
                        installerStream.CopyTo(outputStream);
                    }

                    // Write archive
                    using (var archiveStream = new FileStream(archivePath, FileMode.Open, FileAccess.Read))
                    {
                        archiveStream.CopyTo(outputStream);
                    }

                    // Write footer, read by SetupPayload.cs in InstallerHost: archive length (8 bytes) + archive sha256 (32 bytes) + signature (8 bytes)
                    byte[] archiveLengthBytes = BitConverter.GetBytes(new FileInfo(archivePath).Length);
                    outputStream.Write(archiveLengthBytes, 0, archiveLengthBytes.Length);

                    byte[] archiveHash;
                    using (var archiveStream = File.OpenRead(archivePath))
                    using (var sha = SHA256.Create())
                        archiveHash = sha.ComputeHash(archiveStream);
                    outputStream.Write(archiveHash, 0, archiveHash.Length);

                    byte[] signature = Encoding.ASCII.GetBytes("RBSETUP2");
                    outputStream.Write(signature, 0, signature.Length);
                }

                // Writing sha256
                string sha256 = "";
                string shaFile = finalInstallerPath + ".sha256.txt";
                if (File.Exists(finalInstallerPath))
                {
                    using (var stream = File.OpenRead(finalInstallerPath))
                    using (var sha = SHA256.Create())
                    {
                        byte[] hashBytes = sha.ComputeHash(stream);
                        sha256 = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                    }

                    if (!string.IsNullOrEmpty(sha256))
                        File.WriteAllText(shaFile, sha256);
                }

                Logger.LogInfo("Created final installer executable: " + finalInstallerPath + " (" + new FileInfo(finalInstallerPath).Length + " bytes)");
            }
            catch (Exception ex)
            {
                Logger.Log("[ERROR] Exception creating installer: " + ex.Message);
            }
            finally
            {
                if (use7z)
                    try { File.Delete(archivePath); } catch { }
            }
        }
    }
}
