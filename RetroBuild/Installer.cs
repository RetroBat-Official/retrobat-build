using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace RetroBuild
{
    class Installer
    {
        public static void CreateInstaller(BuilderOptions options)
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string buildPath = Path.Combine(rootPath, "build");

            // Find your ZIP archive
            string zipName = $"retrobat-v{options.RetrobatVersion}-{options.Branch}-{options.Architecture}.zip";
            string zipPath = Path.Combine(buildPath, zipName);

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
                // Read InstallerHost.exe bytes and ZIP bytes
                byte[] installerBytes = File.ReadAllBytes(installerHostExePath);
                byte[] zipBytes = File.ReadAllBytes(zipPath);

                // ZIP length footer is 8 bytes (long)
                long zipLengthLong = zipBytes.Length;
                byte[] zipLengthBytes = BitConverter.GetBytes(zipLengthLong);

                // Create combined array: InstallerHost + ZIP + 8 bytes for length
                byte[] combinedBytes = new byte[installerBytes.Length + zipBytes.Length + zipLengthBytes.Length];

                // Copy InstallerHost.exe bytes
                Buffer.BlockCopy(installerBytes, 0, combinedBytes, 0, installerBytes.Length);
                // Copy ZIP bytes
                Buffer.BlockCopy(zipBytes, 0, combinedBytes, installerBytes.Length, zipBytes.Length);
                // Copy ZIP length bytes at the very end
                Buffer.BlockCopy(zipLengthBytes, 0, combinedBytes, installerBytes.Length + zipBytes.Length, zipLengthBytes.Length);

                // Output final installer executable RetroBat-v7.3.0-stable-win64-setup
                string setupName = "RetroBat-v" + options.RetrobatVersion + "-" + options.Branch + "-" + options.Architecture + "-setup.exe";
                string finalInstallerPath = Path.Combine(buildPath, setupName);
                File.WriteAllBytes(finalInstallerPath, combinedBytes);

                Logger.LogInfo("Created final installer executable: " + finalInstallerPath);
            }
            catch (Exception ex)
            {
                Logger.Log("[ERROR] Exception creating installer: " + ex.Message);
            }
        }
    }
}
