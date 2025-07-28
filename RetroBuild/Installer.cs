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

            string installerProjectPath = Path.Combine(rootPath, "InstallerTemplate");
            string zipName = "retrobat-v" + options.RetrobatVersion + "-" + options.Branch + "-" + options.Architecture + ".zip";
            string zipPath = Directory.GetFiles(buildPath, zipName).FirstOrDefault();

            if (zipPath == null)
            {
                Logger.Log("[ERROR] No .zip file found in build folder with name: " + zipName);
                return;
            }

            // Copy zip into installer template Resources
            string resourcesDir = Path.Combine(installerProjectPath, "Resources");
            Directory.CreateDirectory(resourcesDir);
            string embeddedZip = Path.Combine(resourcesDir, "app.zip");

            File.Copy(zipPath, embeddedZip, true);
            Logger.LogInfo("Copied zip archive to installer resources.");

            // Compile the installer (assumes .csproj is configured correctly)

            Logger.LogInfo("Installer project path: " + installerProjectPath);
            Logger.LogInfo("Checking for .csproj files here:");
            var csprojFiles = Directory.GetFiles(installerProjectPath, "*.csproj");
            foreach (var file in csprojFiles)
                Logger.LogInfo(" - " + file);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
                Arguments = "InstallerTemplate.csproj /p:Configuration=Release",
                WorkingDirectory = installerProjectPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process proc = Process.Start(psi))
            {
                string output = proc.StandardOutput.ReadToEnd();
                string error = proc.StandardError.ReadToEnd();
                proc.WaitForExit();

                Logger.LogInfo(output);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    Logger.Log("[ERROR] Build errors:\n" + error);
                }
                else
                {
                    Logger.LogInfo("Installer built successfully.");
                }
            }
        }

    }
}
