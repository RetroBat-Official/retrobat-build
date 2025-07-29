using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace RetroBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            // Start logging in build.log
            string exeName = AppDomain.CurrentDomain.FriendlyName;
            Logger.LogStart(exeName);

            // Read build.ini information
            string iniPath = "build.ini";

            BuilderOptions options = null;
            try
            {
                Logger.LogInfo("Reading build.ini file for options.");
                options = BuilderOptions.LoadBuilderOptions(iniPath);

                foreach (var prop in options.GetType().GetProperties())
                {
                    string name = prop.Name;
                    object value = prop.GetValue(options, null);
                    Console.WriteLine("{0} = {1}", name, value);
                    Logger.LogInfo(name + " = " + value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading config: " + ex.Message);
                Logger.Log("[ERROR] Error loading config file: " + ex.Message);
                return;
            }

            // Check existence of required tools
            if (!File.Exists(options.SevenZipPath))
            {
                Logger.Log("[ERROR] 7za.exe not found at: " + options.SevenZipPath);
                return;
            }

            if (!File.Exists(options.WgetPath))
            {
                Logger.Log("[ERROR] wget.exe not found at: " + options.WgetPath);
                return;
            }

            if (!File.Exists(options.CurlPath))
            {
                Logger.Log("[ERROR] curl.exe not found at: " + options.CurlPath);
                return;
            }

            Console.Clear();
            Console.WriteLine("RetroBat Builder Menu");
            Console.WriteLine("=====================");
            Console.WriteLine("1 - Download and configure");
            Console.WriteLine("2 - Create archive");
            Console.WriteLine("3 - Create installer (need archive created first)");
            Console.WriteLine("Q - Quit");
            Console.Write("Please type your choice here: ");

            var choice = Console.ReadLine()?.Trim().ToUpper();

            switch (choice)
            {
                case "1":
                    Logger.Log("[INFO] Option selected: Download and configure.");
                    Console.WriteLine("=====================");
                    GetPackages(options);
                    Console.WriteLine("=====================");
                    CreateTree(options);
                    Console.WriteLine("=====================");
                    CreateEmulatorFolders(options);
                    Console.WriteLine("=====================");
                    CreateSystemFolders(options);
                    Console.WriteLine("=====================");
                    GetLibretroCores(options);
                    Console.WriteLine("=====================");
                    GetEmulators(options);
                    Console.WriteLine("=====================");
                    CopyESFiles(options);
                    Console.WriteLine("=====================");
                    CreateVersionFile(options);
                    Console.WriteLine("=====================");
                    CopyTemplateFiles(options);
                    
                    break;
                case "2":
                    Logger.Log("[INFO] Option selected: Create archive.");
                    Console.WriteLine("=====================");
                    CreateZipFolder(options);
                    break;
                case "3":
                    Logger.Log("[INFO] Option selected: Create installer.");
                    Console.WriteLine("=====================");
                    Installer.CreateInstaller(options);
                    break;
                case "Q":
                    Console.WriteLine("Exiting...");
                    return;
            }

            Logger.Log("[INFO] Build finished succesfully.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void GetPackages(BuilderOptions options)
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string buildPath = Path.Combine(rootPath, "build");

            if (!Directory.Exists(buildPath))
            {
                try { Directory.CreateDirectory(buildPath); }
                catch (Exception ex)
                {
                    Logger.Log("[ERROR] Failed to create build directory: " + ex.Message);
                    Console.ReadKey();
                    return;
                }
            }

            string task = "get_packages";
            Logger.LogLabel(task);

            Console.WriteLine(":: GETTING REQUIRED PACKAGES...");

            // Ensure git submodule is initialized
            int gitResult = Methods.RunProcess("git", "submodule update --init", rootPath, out string output);
            if (gitResult != 0)
            {
                Logger.Log("[ERROR] Failed to initialize git submodules");
                Console.ReadKey();
                return;
            }

            // Get RetroBat Binaries
            if (options.GetRetrobatBinaries)
            {
                foreach (var file in Directory.GetFiles(rootPath, "*.txt"))
                {
                    string destFile = Path.Combine(buildPath, Path.GetFileName(file));
                    File.Copy(file, destFile, true);
                }

                string gitBranch = "master";
                string fileName = "retrobat_binaries_" + gitBranch + ".7z";
                string retrobatUrl = options.RetrobatBinariesBaseUrl + fileName;
                Methods.DownloadAndExtractArchive_WebClient(retrobatUrl, buildPath, options);
                Logger.LogInfo("retrobat binaries copied to " + buildPath);
            }

            // Get Emulationstation
            if (options.GetEmulationstation)
            {
                string esUrl = options.EmulationstationUrl;
                string esPath = Path.Combine(buildPath, "emulationstation");
                Methods.DownloadAndExtractArchive_Wget(esUrl, esPath, options);
                Logger.LogInfo("Emulationstation copied to " + esPath);
            }

            // Get Emulatorlauncher
            if (options.GetBatoceraPorts)
            {
                string elUrl = options.EmulatorlauncherUrl;
                string elPath = Path.Combine(buildPath, "emulationstation");
                Methods.DownloadAndExtractArchive_Wget(elUrl, elPath, options);
                Logger.LogInfo("Emulatorlauncher copied to " + elPath);
            }

            // Get BIOS files
            if (options.GetBios)
            {
                string biosPath = Path.Combine(buildPath, "bios");
                Methods.CloneOrUpdateGitRepo(options.BiosGitUrl, biosPath);
                var gitDir = Path.Combine(biosPath, ".git");
                Methods.DeleteGitFiles(biosPath);
            }

            // Get CARBON theme
            if (options.GetDefaultTheme)
            {
                string themePath = Path.Combine(buildPath, "emulationstation", ".emulationstation", "themes", "es-theme-carbon");
                Methods.CloneOrUpdateGitRepo(options.ThemePath, themePath);
                var gitDir = Path.Combine(themePath, ".git");
                Methods.DeleteGitFiles(themePath);
            }

            // Get Decorations
            if (options.GetDecorations)
            {
                string bezelPath = Path.Combine(buildPath, "system", "decorations");
                Methods.CloneOrUpdateGitRepo(options.DecorationsPath, bezelPath);
                var gitDir = Path.Combine(bezelPath, ".git");
                Methods.DeleteGitFiles(bezelPath);
            }

            // Copy system folder
            if (options.GetSystem)
            {
                string systemFolder = Path.Combine(rootPath, "system");

                if (Directory.Exists(systemFolder))
                {
                    Logger.LogInfo("Copying system folder.");
                    string systemBuildPath = Path.Combine(buildPath, "system");

                    // Copy all except .git and decorations folder
                    foreach (var dirPath in Directory.GetDirectories(systemFolder, "*", SearchOption.AllDirectories))
                    {
                        if (dirPath.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (dirPath.EndsWith("decorations", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var targetDir = dirPath.Replace(systemFolder, systemBuildPath);
                        if (!Directory.Exists(targetDir))
                            Directory.CreateDirectory(targetDir);
                    }

                    foreach (var filePath in Directory.GetFiles(systemFolder, "*.*", SearchOption.AllDirectories))
                    {
                        if (filePath.Contains(Path.Combine(".git", "")) || filePath.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (filePath.Contains("system\\decorations"))
                            continue;

                        var targetFile = filePath.Replace(systemFolder, systemBuildPath);
                        File.Copy(filePath, targetFile, true);
                    }

                    Logger.LogInfo("System folder copied.");
                }
                else
                {
                    string systemPath = Path.Combine(buildPath, "system");
                    Methods.CloneOrUpdateGitRepo(options.SystemPath, systemPath);
                    var gitDir = Path.Combine(systemPath, ".git");
                    Methods.DeleteGitFiles(systemPath);
                }
            }

            // Get Retroarch
            if (options.GetRetroarch)
            {
                string raVersion = options.RetroarchVersion;
                string retroarchPath = Path.Combine(buildPath, "emulators", "retroarch");
                string raUrl = options.RetroArchURL + "/stable/" + raVersion + "/windows/x86_64/RetroArch.7z";
                Methods.DownloadAndExtractArchive_Wget(raUrl, retroarchPath, options);

                string retroarchtempPath = Directory.GetDirectories(retroarchPath).FirstOrDefault(d => Path.GetFileName(d).Contains("RetroArch-Win64"));

                if (Directory.Exists(retroarchtempPath))
                {
                    foreach (var file in Directory.GetFiles(retroarchtempPath, "*", SearchOption.AllDirectories))
                    {
                        string relativePath = file.Substring(retroarchtempPath.Length + 1); // relative path from source
                        string destinationPath = Path.Combine(retroarchPath, relativePath);

                        // Ensure destination directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                        // Copy file
                        File.Copy(file, destinationPath, overwrite: true);
                    }

                    Console.WriteLine("All files copied successfully.");
                }
                else
                {
                    Console.WriteLine("Source directory does not exist.");
                }

                try 
                { 
                    Directory.Delete(retroarchtempPath, recursive: true);
                    Logger.LogInfo("RetroArch succesfully downloaded to " + retroarchPath);
                } 
                catch { Logger.LogInfo("[ERROR] Not able to delete RetroArch temp folder: " + retroarchtempPath); }
            }

            // Get WiimoteGun
            if (options.GetWiimotegun)
            {
                string wiimoteGunUrl = options.WiimoteGunURL;
                string wiimoteGunPath = Path.Combine(buildPath, "emulationstation");
                Methods.DownloadAndExtractArchive_Wget(wiimoteGunUrl, wiimoteGunPath, options);
                Logger.LogInfo("WiimoteGun copied to " + wiimoteGunPath);
            }
            
            // Get BatGui
            if (options.GetBatgui)
            {
                string batguiUrl = options.BatGUIURL;
                string batguiPath = Path.Combine(buildPath);
                Methods.DownloadAndExtractArchive_Wget(batguiUrl, batguiPath, options);
                Logger.LogInfo("BatGui copied to " + batguiPath);
            }
        }

        static void CreateTree(BuilderOptions options)
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string buildPath = Path.Combine(rootPath, "build");

            if (!Directory.Exists(buildPath))
            {
                try { Directory.CreateDirectory(buildPath); }
                catch (Exception ex)
                {
                    Logger.Log("[ERROR] Failed to create build directory: " + ex.Message);
                    Console.ReadKey();
                    return;
                }
            }

            string task = "build_tree";
            Logger.LogLabel(task);

            Console.WriteLine(":: BUILDING RETROBAT TREE...");

            string treeFile = Path.Combine(buildPath, "system", "configgen", "retrobat_tree.lst");

            if (!File.Exists(treeFile))
            {
                Logger.LogInfo("Missing 'retrobat_tree.lst' file.");
                return;
            }

            foreach (var relativePath in File.ReadAllLines(treeFile))
            {
                if (string.IsNullOrWhiteSpace(relativePath)) continue;

                string fullPath = Path.Combine(buildPath, relativePath.Trim());

                if (Directory.Exists(fullPath))
                {
                    Logger.LogInfo($"Directory already exists: {fullPath}");
                    continue;
                }
                try
                {
                    Directory.CreateDirectory(fullPath);
                    Logger.LogInfo($"Created: {fullPath}");
                }
                catch (Exception ex)
                {
                    Logger.LogInfo($"Failed to create {fullPath}: {ex.Message}");
                }
            }

            Logger.LogInfo("All folders processed.");
        }

        static void CreateEmulatorFolders(BuilderOptions options)
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string buildPath = Path.Combine(rootPath, "build");

            if (!Directory.Exists(buildPath))
            {
                try { Directory.CreateDirectory(buildPath); }
                catch (Exception ex)
                {
                    Logger.Log("[ERROR] Failed to create build directory: " + ex.Message);
                    Console.ReadKey();
                    return;
                }
            }

            string task = "emulator_folders";
            Logger.LogLabel(task);

            Console.WriteLine(":: CREATING EMULATOR FOLDERS...");

            string emulatorListFile = Path.Combine(buildPath, "system", "configgen", "emulators_names.lst");

            if (!File.Exists(emulatorListFile))
            {
                Logger.LogInfo("Missing 'emulators_names.lst' file.");
                return;
            }

            foreach (var relativePath in File.ReadAllLines(emulatorListFile))
            {
                if (string.IsNullOrWhiteSpace(relativePath)) continue;

                string fullPath = Path.Combine(buildPath, "emulators", relativePath.Trim());

                if (Directory.Exists(fullPath))
                {
                    Logger.LogInfo($"Directory already exists: {fullPath}");
                    continue;
                }
                try
                {
                    Directory.CreateDirectory(fullPath);
                    Logger.LogInfo($"Created: {fullPath}");
                }
                catch (Exception ex)
                {
                    Logger.LogInfo($"Failed to create {fullPath}: {ex.Message}");
                }
            }

            Logger.LogInfo("All emulator folders processed.");
        }

        static void CreateSystemFolders(BuilderOptions options)
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string buildPath = Path.Combine(rootPath, "build");

            if (!Directory.Exists(buildPath))
            {
                try { Directory.CreateDirectory(buildPath); }
                catch (Exception ex)
                {
                    Logger.Log("[ERROR] Failed to create build directory: " + ex.Message);
                    Console.ReadKey();
                    return;
                }
            }

            string task = "system_folders";
            Logger.LogLabel(task);

            Console.WriteLine(":: CREATING ROMS AND SAVE FOLDERS...");

            string systemListFile = Path.Combine(buildPath, "system", "configgen", "systems_names.lst");

            if (!File.Exists(systemListFile))
            {
                Logger.LogInfo("Missing 'systems_names.lst' file.");
                return;
            }

            foreach (var relativePath in File.ReadAllLines(systemListFile))
            {
                if (string.IsNullOrWhiteSpace(relativePath)) continue;

                string romPath = Path.Combine(buildPath, "roms", relativePath.Trim());

                if (Directory.Exists(romPath))
                {
                    Logger.LogInfo($"Directory already exists: {romPath}");
                    continue;
                }
                try
                {
                    Directory.CreateDirectory(romPath);
                    Logger.LogInfo($"Created: {romPath}");
                }
                catch (Exception ex)
                {
                    Logger.LogInfo($"Failed to create {romPath}: {ex.Message}");
                }

                string savesPath = Path.Combine(buildPath, "saves", relativePath.Trim());

                if (Directory.Exists(savesPath))
                {
                    Logger.LogInfo($"Directory already exists: {savesPath}");
                    continue;
                }
                try
                {
                    Directory.CreateDirectory(savesPath);
                    Logger.LogInfo($"Created: {savesPath}");
                }
                catch (Exception ex)
                {
                    Logger.LogInfo($"Failed to create {savesPath}: {ex.Message}");
                }
            }

            Logger.LogInfo("All roms folders processed.");
        }

        static void CopyESFiles(BuilderOptions options)
        {
            List<string> esFiles = new List<string>
            {
                "es_input.cfg",
                "es_padtokey.cfg",
                "es_settings.cfg",
                "es_systems.cfg"
            };

            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string buildPath = Path.Combine(rootPath, "build");

            if (!Directory.Exists(buildPath))
            {
                try { Directory.CreateDirectory(buildPath); }
                catch (Exception ex)
                {
                    Logger.Log("[ERROR] Failed to create build directory: " + ex.Message);
                    Console.ReadKey();
                    return;
                }
            }

            string task = "emulationstation_config";
            Logger.LogLabel(task);

            Console.WriteLine(":: COPY EMULATIONSTATION FILES...");

            string esTemplateFolder = Path.Combine(buildPath, "system", "templates", "emulationstation");
            string destPointFolder = Path.Combine(buildPath, "emulationstation", ".emulationstation");

            foreach (string file in esFiles)
            {
                string sourceFile = Path.Combine(esTemplateFolder, file);
                string destFile = Path.Combine(destPointFolder, file);

                if (!File.Exists(sourceFile))
                {
                    Logger.LogInfo($"Source file not found: {sourceFile}");
                    continue;
                }

                try
                {
                    File.Copy(sourceFile, destFile, true);
                    Logger.LogInfo($"Copied {file} to {destPointFolder}");
                }
                catch (Exception ex)
                {
                    Logger.LogInfo($"Failed to copy {file}: {ex.Message}");
                }
            }

            // Other files
            string esOtherFilesPath = Path.Combine(buildPath, "system", "resources", "emulationstation");
            string noticeFile = Path.Combine(esOtherFilesPath, "notice.pdf");
            if (!File.Exists(noticeFile))
            {
                Logger.LogInfo($"Source file not found: {noticeFile}");
            }
            else
            {
                string destNoticeFile = Path.Combine(destPointFolder, "notice.pdf");
                try
                {
                    File.Copy(noticeFile, destNoticeFile, true);
                    Logger.LogInfo($"Copied {noticeFile} to {destPointFolder}");
                }
                catch (Exception ex)
                {
                    Logger.LogInfo($"Failed to copy notice file: {ex.Message}");
                    return;
                }
            }

            string musicPath = Path.Combine(esOtherFilesPath, "music");
            string videoPath = Path.Combine(esOtherFilesPath, "video");
            string destMusicPath = Path.Combine(destPointFolder, "music");
            string destVideoPath = Path.Combine(destPointFolder, "video");

            if (!Directory.Exists(destMusicPath))
            {
                try
                {
                    Directory.CreateDirectory(destMusicPath);
                    Logger.LogInfo($"Created directory: {destMusicPath}");
                }
                catch (Exception ex)
                {
                    Logger.LogInfo($"Failed to create music directory: {ex.Message}");
                    return;
                }
            }

            if (!Directory.Exists(destVideoPath))
            {
                try
                {
                    Directory.CreateDirectory(destVideoPath);
                    Logger.LogInfo($"Created directory: {destVideoPath}");
                }
                catch (Exception ex)
                {
                    Logger.LogInfo($"Failed to create video directory: {ex.Message}");
                    return;
                }
            }

            foreach (var filePath in Directory.GetFiles(musicPath, "*.*"))
            {
                var targetFile = filePath.Replace(musicPath, destMusicPath);
                try 
                { 
                    File.Copy(filePath, targetFile, true);
                    Logger.LogInfo("Copid music file: " + targetFile);
                }
                catch (Exception ex)
                {
                    Logger.LogInfo($"Failed to copy music file {filePath} to {targetFile}: {ex.Message}");
                    return;
                }
            }

            foreach (var filePath in Directory.GetFiles(videoPath, "*.*"))
            {
                var targetFile = filePath.Replace(videoPath, destVideoPath);
                try 
                { 
                    File.Copy(filePath, targetFile, true);
                    Logger.LogInfo("Copid video file: " + targetFile);
                }
                catch (Exception ex)
                {
                    Logger.LogInfo($"Failed to copy video file {filePath} to {targetFile}: {ex.Message}");
                    return;
                }
            }

            // translations
            string templateTranslationsPath = Path.Combine(esTemplateFolder, "es_features.locale");
            string targetTranslationsPath = Path.Combine(buildPath, "emulationstation", "es_features.locale");

            if (!Directory.Exists(targetTranslationsPath))
            {
                try 
                { 
                    Directory.CreateDirectory(targetTranslationsPath);
                }
                catch (Exception ex)
                {
                    Logger.Log("[ERROR] Failed to create translations directory: " + ex.Message);
                    Console.ReadKey();
                    return;
                }
            }

            foreach (var dirPath in Directory.GetDirectories(templateTranslationsPath, "*", SearchOption.AllDirectories))
            {
                var targetDir = dirPath.Replace(templateTranslationsPath, targetTranslationsPath);
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);
            }

            foreach (var filePath in Directory.GetFiles(templateTranslationsPath, "*.*", SearchOption.AllDirectories))
            {
                var targetFile = filePath.Replace(templateTranslationsPath, targetTranslationsPath);
                File.Copy(filePath, targetFile, true);
            }
            Logger.LogInfo("Create locale folder: " + targetTranslationsPath);
        }
        
        static void CreateVersionFile(BuilderOptions options)
        {
            string task = "create_version";
            Logger.LogLabel(task);

            Console.WriteLine(":: CREATE VERSION FILES...");

            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string buildPath = Path.Combine(rootPath, "build");
            string versionFilePath = Path.Combine(buildPath, "system", "version.info");
            string esVersionFilePath = Path.Combine(buildPath, "emulationstation", "version.info");

            string version = options.RetrobatVersion + "-" + options.Branch + "-" + options.Architecture;

            File.WriteAllText(versionFilePath, version);
            Logger.LogInfo("Created version file: " + versionFilePath);
            File.WriteAllText(esVersionFilePath, version);
            Logger.LogInfo("Created version file: " + esVersionFilePath);
        }

        static void CopyTemplateFiles(BuilderOptions options)
        {
            string task = "copy_template";
            Logger.LogLabel(task);

            Console.WriteLine(":: COPY TEMPLATE FILES...");

            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string buildPath = Path.Combine(rootPath, "build");

            string sevenZipPath = Path.Combine(rootPath, "system", "tools", "7za.exe");

            if (!File.Exists(sevenZipPath))
            {
                Logger.Log("[ERROR] 7za.exe not found at: " + sevenZipPath);
                return;
            }

            string templateFile = Path.Combine(buildPath, "system", "configgen", "templates_files.lst");

            if (!File.Exists(templateFile))
            {
                Logger.LogInfo("Missing 'templates_files.lst' file.");
                return;
            }

            string[] mappings = File.ReadAllLines(templateFile);

            foreach (var line in mappings)
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains("|"))
                    continue;

                var parts = line.Split('|');
                string source = Path.Combine(buildPath, Methods.NormalizePath(parts[0]));
                string target = Path.Combine(buildPath, Methods.NormalizePath(parts[1]));

                Logger.LogInfo($"\nProcessing: {source} -> {target}");

                try
                {
                    if (File.Exists(source))
                    {
                        string ext = Path.GetExtension(source).ToLowerInvariant();

                        if (ext == ".zip")
                        {
                            Logger.LogInfo(" - Extracting ZIP using 7z...");
                            string targetDir = Directory.Exists(target) ? target : Path.GetDirectoryName(target);
                            Directory.CreateDirectory(targetDir);
                            Methods.ExtractZipWith7z(sevenZipPath, source, targetDir);
                        }
                        else
                        {
                            Logger.LogInfo(" - Copying file...");
                            Directory.CreateDirectory(Path.GetDirectoryName(target));
                            File.Copy(source, target, true);
                        }
                    }
                    else if (Directory.Exists(source))
                    {
                        Logger.LogInfo(" - Copying folder contents...");
                        Methods.CopyDirectory(source, target);
                    }
                    else
                    {
                        Logger.Log($"[ERROR] Source not found: {source}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"[ERROR] Error processing {source}: {ex.Message}");
                }
            }
        }

        static void GetLibretroCores(BuilderOptions options)
        {
            if (!options.GetLrcores)
            {
                Logger.LogInfo("Skipping Libretro cores download as per options.");
                return;
            }

            string task = "get_lrcores";
            Logger.LogLabel(task);

            Console.WriteLine(":: GETTING LIBRETRO CORES...");

            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string buildPath = Path.Combine(rootPath, "build");
            string targetCorePath = Path.Combine(buildPath, "emulators", "retroarch", "cores");

            string lrCoreListFile = Path.Combine(rootPath, "system", "configgen", "lrcores_names.lst");

            if (!File.Exists(lrCoreListFile))
            {
                Logger.LogInfo("Missing 'lrcores_names.lst' file.");
                return;
            }

            string ftpRootPath = options.RetrobatFTPPath;
            string coreUrl = ftpRootPath + options.Architecture + "/" + options.Branch + "/emulators/cores/";

            string[] lrCores = File.ReadAllLines(lrCoreListFile);

            foreach (var core in lrCores)
            {
                if (string.IsNullOrWhiteSpace(core))
                    continue;

                string coreZipFile = coreUrl + core + "_libretro.dll.zip";

                Thread.Sleep(3000);

                try
                {
                    int i = 0;
                    bool success = false;
                    for (i = 0; i < 5; i++)
                    {
                        success = Methods.DownloadAndExtractArchive_Wget(coreZipFile, targetCorePath, options);
                        if (success)
                        {
                            Logger.LogInfo("Libretro Core " + core + " copied to: " + targetCorePath);
                            break;
                        }
                        else
                            Thread.Sleep(4000);
                        i++;
                    }
                    
                    if (!success)
                    {
                        Logger.LogInfo($"[WARNING] Failed to download or extract core: {core} from FTP, looking on RetroArch buildbot");
                        coreZipFile = options.RetroArchURL + "/nightly/windows/x86_64/latest/" + core + "_libretro.dll.zip";
                        Methods.DownloadAndExtractArchive_Wget(coreZipFile, targetCorePath, options);
                    }
                }
                catch
                {
                    Logger.Log($"[ERROR] Error downloading RetroArch core.");
                }
            }
        }

        static void GetEmulators(BuilderOptions options)
        {
            if (!options.GetEmulators)
            {
                Logger.LogInfo("Skipping Emulators download as per options.");
                return;
            }

            string task = "get_emulators";
            Logger.LogLabel(task);

            Console.WriteLine(":: GETTING EMULATORS...");

            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string buildPath = Path.Combine(rootPath, "build");
            string targetEmuPath = Path.Combine(buildPath, "emulators");

            string emuListFile = Path.Combine(rootPath, "system", "configgen", "emulators_names.lst");

            if (!File.Exists(emuListFile))
            {
                Logger.LogInfo("Missing 'emulators_names.lst' file.");
                return;
            }

            string ftpRootPath = options.RetrobatFTPPath;
            string emuUrl = ftpRootPath + options.Architecture + "/" + options.Branch + "/emulators/";

            string[] emulators = File.ReadAllLines(emuListFile);

            foreach (var emu in emulators)
            {
                List<string> emuIgnore = new List<string>
                {
                    "retroarch",
                    "eden",
                    "3dsen",
                    "teknoparrot",
                    "citron",
                    "yuzu",
                    "pico8",
                    "ryujinx",
                    "steam",
                    "sudachi",
                    "suyu",
                    "yuzu-early-access"
                };
                if (string.IsNullOrWhiteSpace(emu))
                    continue;

                if (emuIgnore.Contains(emu))
                    continue;

                Thread.Sleep(3000);
                string emuZipFile = emuUrl + emu + ".7z";
                string targetPath = Path.Combine(targetEmuPath, emu);

                try
                {
                    int i = 0;
                    bool success = false;

                    for (i = 0; i < 5; i++)
                    {
                        success = Methods.DownloadAndExtractArchive_Wget(emuZipFile, targetPath, options);
                        if (success)
                        {
                            Logger.LogInfo("Emulator " + emu + " copied to: " + targetPath);
                            break;
                        }
                        else
                            Thread.Sleep(4000);
                        i++;
                    }

                    if (!success)
                    {
                        Logger.LogInfo($"[WARNING] Failed to download or extract emulator: {emu} from FTP.");
                    }
                }
                catch
                {
                    Logger.Log($"[ERROR] Error downloading Emulator.");
                }
            }
        }

        public static void CreateZipFolder(BuilderOptions options)
        {
            string task = "create_ziparchive";
            Logger.LogLabel(task);

            Console.WriteLine(":: CREATE ZIP ARCHIVE...");

            string zipName = "retrobat-v" + options.RetrobatVersion + "-" + options.Branch + "-" + options.Architecture + ".zip";
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string buildPath = Path.Combine(rootPath, "build");

            string sourceFolder = buildPath;
            string zipPath = Path.Combine(buildPath, zipName);

            if (Directory.Exists(sourceFolder))
            {
                if (File.Exists(zipPath))
                {
                    try { File.Delete(zipPath); } catch { }
                }

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = options.SevenZipPath;
                psi.Arguments = $"a -tzip \"{zipPath}\" \"{sourceFolder}\\*\"";
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.CreateNoWindow = true;

                using (Process proc = Process.Start(psi))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    string error = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();

                    Logger.LogInfo("7-Zip Output: " + output);

                    if (!string.IsNullOrEmpty(error))
                    {
                        Logger.LogInfo("7-Zip Errors: " + error);
                    }

                    Logger.LogInfo($"ZIP created at: {zipPath}");
                }

                // Writing sha256
                string sha256 = "";
                string shaFile = zipPath + ".sha256.txt";
                if (File.Exists(zipPath))
                {
                    using (var stream = File.OpenRead(zipPath))
                    using (var sha = SHA256.Create())
                    {
                        byte[] hashBytes = sha.ComputeHash(stream);
                        sha256 = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                    }

                    if (!string.IsNullOrEmpty(sha256))
                        File.WriteAllText(shaFile, sha256);
                }
            }
            else
            {
                Logger.LogInfo("[ERROR] Source folder does not exist.");
            }
        }
    }
}
