using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RetroBuild
{
    class IniParser
    {
        private Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        public IniParser(string path)
        {
            Load(path);
        }

        private void Load(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("INI file not found", path);

            string section = "";
            foreach (var lineRaw in File.ReadAllLines(path))
            {
                string line = lineRaw.Trim();

                if (line.StartsWith(";") || line.StartsWith("#") || string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    section = line.Substring(1, line.Length - 2);
                    if (!data.ContainsKey(section))
                        data[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else if (line.Contains("="))
                {
                    int idx = line.IndexOf('=');
                    string key = line.Substring(0, idx).Trim();
                    string val = line.Substring(idx + 1).Trim();

                    if (!data.ContainsKey(section))
                        data[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    data[section][key] = val;
                }
            }
        }

        public string Get(string section, string key, string defaultValue = "")
        {
            if (data.ContainsKey(section) && data[section].ContainsKey(key))
                return data[section][key];
            return defaultValue;
        }
    }

    public class BuilderOptions
    {
        public string RetrobatVersion { get; set; }
        public string RetroarchVersion { get; set; }
        public string Branch { get; set; }
        public string Architecture { get; set; }
        public bool GetBatgui { get; set; }
        public bool GetBatoceraPorts { get; set; }
        public bool GetBios { get; set; }
        public bool GetDecorations { get; set; }
        public bool GetDefaultTheme { get; set; }
        public bool GetEmulationstation { get; set; }
        public bool GetEmulators { get; set; }
        public bool GetLrcores { get; set; }
        public bool GetRetroarch { get; set; }
        public bool GetRetrobatBinaries { get; set; }
        public bool GetSystem { get; set; }
        public bool GetWiimotegun { get; set; }
        public string SevenZipPath { get; set; }
        public string WgetPath { get; set; }
        public string CurlPath { get; set; }
        public string RetrobatBinariesBaseUrl { get; set; }
        public string EmulationstationUrl { get; set; }
        public string EmulatorlauncherUrl { get; set; }
        public string BiosGitUrl { get; set; }
        public string ThemePath { get; set; }
        public string DecorationsPath { get; set; }
        public string SystemPath { get; set; }
        public string RetroArchURL { get; set; }
        public string WiimoteGunURL { get; set; }
        public string BatGUIURL { get; set; }

        public static BuilderOptions LoadBuilderOptions(string iniFile)
        {
            var parser = new IniParser(iniFile);
            var opt = new BuilderOptions();

            string section = "BuilderOptions";

            opt.RetrobatVersion = parser.Get(section, "retrobat_version");
            opt.RetroarchVersion = parser.Get(section, "retroarch_version");
            opt.Branch = parser.Get(section, "branch", "stable");
            opt.Architecture = parser.Get(section, "architecture", "win64");

            opt.GetBatgui = parser.Get(section, "get_batgui") == "1";
            opt.GetBatoceraPorts = parser.Get(section, "get_batocera_ports") == "1";
            opt.GetBios = parser.Get(section, "get_bios") == "1";
            opt.GetDecorations = parser.Get(section, "get_decorations") == "1";
            opt.GetDefaultTheme = parser.Get(section, "get_default_theme") == "1";
            opt.GetEmulationstation = parser.Get(section, "get_emulationstation") == "1";
            opt.GetEmulators = parser.Get(section, "get_emulators") == "1";
            opt.GetLrcores = parser.Get(section, "get_lrcores") == "1";
            opt.GetRetroarch = parser.Get(section, "get_retroarch") == "1";
            opt.GetRetrobatBinaries = parser.Get(section, "get_retrobat_binaries") == "1";
            opt.GetSystem = parser.Get(section, "get_system") == "1";
            opt.GetWiimotegun = parser.Get(section, "get_wiimotegun") == "1";
            opt.SevenZipPath = Methods.PathCombineExeDir(parser.Get(section, "7za_path", "system\\tools\\7za.exe"));
            opt.WgetPath = Methods.PathCombineExeDir(parser.Get(section, "wget_path", "system\\tools\\wget.exe"));
            opt.CurlPath = Methods.PathCombineExeDir(parser.Get(section, "curl_path", "system\\tools\\curl.exe"));
            opt.RetrobatBinariesBaseUrl = parser.Get(section, "retrobat_binaries_url", "http://www.retrobat.ovh/repo/tools/");
            opt.EmulationstationUrl = parser.Get(section, "emulationstation_url", "https://github.com/RetroBat-Official/emulationstation/releases/download/continuous-master/EmulationStation-Win32.zip");
            opt.EmulatorlauncherUrl = parser.Get(section, "emulatorlauncher_url", "https://github.com/RetroBat-Official/emulatorlauncher/releases/download/continuous/batocera-ports.zip");
            opt.BiosGitUrl = parser.Get(section, "bios_git_url", "https://github.com/RetroBat-Official/emulatorlauncher/releases/download/continuous/batocera-ports.zip");
            opt.ThemePath = parser.Get(section, "theme_path", "https://github.com/fabricecaruso/es-theme-carbon");
            opt.DecorationsPath = parser.Get(section, "decorations_path", "https://github.com/RetroBat-Official/retrobat-bezels");
            opt.SystemPath = parser.Get(section, "retrobat_system_path", "https://github.com/RetroBat-Official/retrobat-setup/tree/master/system");
            opt.RetroArchURL = parser.Get(section, "retroarch_url", "https://buildbot.libretro.com/stable");
            opt.WiimoteGunURL = parser.Get(section, "wiimotegun_url", "https://github.com/fabricecaruso/WiimoteGun/releases/download/v1.1/WiimoteGun.zip");
            opt.BatGUIURL = parser.Get(section, "batgui_url", "https://github.com/xReppa/rb_gui/releases/download/2.0.56.0/BatGui2056.zip");
            return opt;
        }

        public static bool IsComponentEnabled(string key, BuilderOptions options)
        {
            switch (key)
            {
                case "bios":
                    return options.GetBios;
                case "decorations":
                    return options.GetDecorations;
                case "default_theme":
                    return options.GetDefaultTheme;
                case "emulationstation":
                    return options.GetEmulationstation;
                case "retrobat_binaries":
                    return options.GetRetrobatBinaries;
                case "batocera_ports":
                    return options.GetBatoceraPorts;
                case "retroarch":
                    return options.GetRetroarch;
                case "wiimotegun":
                    return options.GetWiimotegun;
                default:
                    return false;
            }
        }
    }

}
