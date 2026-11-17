using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace InstallerHost
{
    public static class PrerequisiteDetector
    {
        private static readonly string[] Architectures = { "x86", "x64" };
        private static readonly string[] Versions = { "2005", "2008", "2010", "2012", "2013", "2015_2022" };

        // Version of the VC++ v14 runtime hosted in the prerequisites, to update together with the archives
        private static readonly Version MinimumVC14Version = new Version(14, 52, 37029);

        /// <summary>
        /// Checks if all VC++ redistributables (2005 → 2022) are installed for both x86 and x64.
        /// Returns false immediately if any one is missing.
        /// </summary>
        public static bool IsVCppFullyInstalled()
        {
            foreach (var version in Versions)
            {
                foreach (var arch in Architectures)
                {
                    if (!IsVersionInstalled(version, arch))
                        return false; // stop immediately
                }
            }

            return true; // all found
        }

        /// <summary>
        /// True if the VC++ redistributable of a prerequisite archive (for example "vcredist2010_x86.zip") is already installed.
        /// </summary>
        public static bool IsVCppInstalled(string zipFileName)
        {
            string arch = zipFileName.Contains("x64") ? "x64" : "x86";

            foreach (var version in Versions)
            {
                // The archive of version "2015_2022" is named "vcredist2015_2017_2019_2022"
                if (zipFileName.StartsWith("vcredist" + version.Substring(0, 4)))
                    return IsVersionInstalled(version, arch);
            }

            return false;
        }

        private static bool IsVersionInstalled(string version, string arch)
        {
            switch (version)
            {
                // Older versions rely on Uninstall keys
                case "2005":
                case "2008":
                case "2010":
                case "2012":
                case "2013":
                    return IsInstalledFromUninstall(version, arch);

                // 2015+ uses the VC\Runtimes keys
                case "2015_2022":
                    return IsInstalledFromRuntimeKey($@"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\{arch}");

                default:
                    return false;
            }
        }

        private static bool IsInstalledFromRuntimeKey(string path)
        {
            // Check both 64-bit and 32-bit registry views
            foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                using (var key = baseKey.OpenSubKey(path, RegistryKeyPermissionCheck.ReadSubTree))
                {
                    if ((int?)key?.GetValue("Installed") != 1)
                        continue;

                    // An older runtime is reported as missing, so that it gets updated
                    var installed = new Version(Convert.ToInt32(key.GetValue("Major", 0)), Convert.ToInt32(key.GetValue("Minor", 0)), Convert.ToInt32(key.GetValue("Bld", 0)));
                    if (installed >= MinimumVC14Version)
                        return true;
                }
            }
            return false;
        }

        private static bool IsInstalledFromUninstall(string version, string arch)
        {
            string[] uninstallPaths = { @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall" };

            foreach (var path in uninstallPaths)
            {
                using (var key = Registry.LocalMachine.OpenSubKey(path, RegistryKeyPermissionCheck.ReadSubTree))
                {
                    if (key == null)
                        continue;

                    foreach (var subName in key.GetSubKeyNames())
                    {
                        using (var subKey = key.OpenSubKey(subName))
                        {
                            if (IsVCppName(subKey?.GetValue("DisplayName")?.ToString(), version, arch))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// True if an installed program name is the VC++ redistributable of this version and architecture.
        /// </summary>
        private static bool IsVCppName(string displayName, string version, string arch)
        {
            if (string.IsNullOrEmpty(displayName) || !displayName.Contains("Microsoft Visual C++ " + version))
                return false;

            // The 2005 x86 package has no architecture in its name: "Microsoft Visual C++ 2005 Redistributable"
            if (version == "2005" && arch == "x86")
                return !displayName.Contains("x64");

            return displayName.Contains(arch);
        }

        public static bool IsDirectXJun2010Installed()
        {
            const string marker = "XAudio2_7.dll";

            try
            {
                if (!File.Exists(Path.Combine(GetSystemDir(false), marker)))
                    return false;

                if (Environment.Is64BitOperatingSystem && !File.Exists(Path.Combine(GetSystemDir(true), marker)))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetSystemDir(bool wow64)
        {
            string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            if (wow64)
                return Path.Combine(windir, "SysWOW64");

            if (!Environment.Is64BitProcess && Environment.Is64BitOperatingSystem)
                return Path.Combine(windir, "Sysnative");

            return Path.Combine(windir, "System32");
        }

        public static bool IsDokanyInstalled()
        {
            string[] possibleServices =
            {
                @"SYSTEM\CurrentControlSet\Services\dokan1",
                @"SYSTEM\CurrentControlSet\Services\dokan2"
            };

            foreach (var servicePath in possibleServices)
            {
                using (var key = Registry.LocalMachine.OpenSubKey(servicePath, RegistryKeyPermissionCheck.ReadSubTree))
                {
                    if (key != null)
                        return true;
                }
            }

            return false;
        }

        public static bool IsWinFspInstalled()
        {
            string dll64 = @"C:\Program Files (x86)\WinFsp\bin\winfsp-x64.dll";
            string dll32 = @"C:\Program Files (x86)\WinFsp\bin\winfsp-x86.dll";

            return File.Exists(dll64) || File.Exists(dll32);
        }
    }
}
