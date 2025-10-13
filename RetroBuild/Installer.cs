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

            // Find your ZIP archive
            string zipName = $"retrobat-v{options.RetrobatVersion}-{options.Branch}-{options.Architecture}.zip";
            string zipPath = Path.Combine(rootPath, zipName);

            if (!File.Exists(zipPath))
            {
                Logger.Log("[INFO] zip not found, creating zip first.");
                try { Program.CreateZipFolderSharpZip(options); }
                catch (Exception ex)
                {
                    Logger.Log("[ERROR] Exception creating ZIP: " + ex.Message);
                }
            }

            if (!File.Exists(zipPath))
            {
                Logger.Log("[ERROR] No .zip file found at: " + zipPath);
                return;
            }
            Logger.LogInfo("Found zip file: " + zipPath);

            string installerHostExePath = Path.Combine(rootPath, "InstallerHost.exe");

            if (!File.Exists(installerHostExePath))
            {
                Logger.Log("[ERROR] InstallerHost.exe not found at: " + installerHostExePath);
                return;
            }
            Logger.LogInfo("Found InstallerHost.exe at: " + installerHostExePath);

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

                    // Write ZIP
                    using (var zipStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
                    {
                        zipStream.CopyTo(outputStream);
                    }

                    // Write ZIP length (as 8 bytes)
                    byte[] zipLengthBytes = BitConverter.GetBytes(new FileInfo(zipPath).Length);
                    outputStream.Write(zipLengthBytes, 0, zipLengthBytes.Length);
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

                Logger.LogInfo("Created final installer executable: " + finalInstallerPath);
            }
            catch (Exception ex)
            {
                Logger.Log("[ERROR] Exception creating installer: " + ex.Message);
            }
        }
    }
}
