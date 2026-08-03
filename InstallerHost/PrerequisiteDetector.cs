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
                    if ((int?)key?.GetValue("Installed") == 1)
                        return true;
                }
            }
            return false;
        }

        private static bool IsInstalledFromUninstall(string version, string arch)
        {
            string[] uninstallPaths = { @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall" };

            if (version == "2005")
                version = "";

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
                            string displayName = subKey?.GetValue("DisplayName")?.ToString();

                            if (!string.IsNullOrEmpty(displayName) &&
                                displayName.Contains($"Microsoft Visual C++ {version}") &&
                                displayName.Contains(arch))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
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
