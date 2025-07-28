using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace RetroBuild
{
    internal class Methods
    {
        public static string PathCombineExeDir(string relativePath)
        {
            string exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.GetFullPath(Path.Combine(exeDir, relativePath));
        }

        public static void CopyDirectory(string sourceDir, string destDir)
        {
            foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourceDir, destDir));

            foreach (string filePath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
                File.Copy(filePath, filePath.Replace(sourceDir, destDir), true);
        }

        public static int RunProcess(string exe, string args, string workingDir, out string output)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = args,
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process();
            process.StartInfo = startInfo;

            try
            {
                process.Start();
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return process.ExitCode;
            }
            catch (Exception ex)
            {
                output = "[ERROR] Failed to run process: " + ex.Message;
                return -1;
            }
            finally
            {
                process.Dispose();
            }
        }

        public static bool DownloadAndExtractArchive_WebClient(string url, string outputDir, BuilderOptions options)
        {
            string fileName = Path.GetFileName(new Uri(url).AbsolutePath);
            string tempFile = Path.Combine(Path.GetTempPath(), fileName);

            Logger.LogInfo("Downloading (WebClient): " + url);

            try
            {
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadFile(url, tempFile);
                }

                Logger.LogInfo("Download complete: " + tempFile);

                return ExtractArchive(tempFile, outputDir, options);
            }
            catch (Exception ex)
            {
                Logger.Log("[ERROR] Download or extract failed: " + ex.Message);
                return false;
            }
            finally
            {
                TryDeleteFile(tempFile);
            }
        }

        public static bool DownloadAndExtractArchive_Curl(string url, string outputDir, BuilderOptions options)
        {
            string fileName = Path.GetFileName(new Uri(url).AbsolutePath);
            string tempFile = Path.Combine(Path.GetTempPath(), fileName);

            Logger.LogInfo("Downloading (curl): " + url);

            try
            {
                var curlArgs = $"--silent --show-error --fail -L \"{url}\" -o \"{tempFile}\"";
                var curlStartInfo = new ProcessStartInfo
                {
                    FileName = options.CurlPath,
                    Arguments = curlArgs,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var curlProcess = Process.Start(curlStartInfo))
                {
                    curlProcess.WaitForExit();

                    string curlStdErr = curlProcess.StandardError.ReadToEnd();

                    if (curlProcess.ExitCode != 0)
                    {
                        Logger.Log("[ERROR] Curl download failed: " + curlStdErr);
                        return false;
                    }
                }

                Logger.LogInfo("Download complete: " + tempFile);

                return ExtractArchive(tempFile, outputDir, options);
            }
            catch (Exception ex)
            {
                Logger.Log("[ERROR] Download or extract failed: " + ex.Message);
                return false;
            }
            finally
            {
                TryDeleteFile(tempFile);
            }
        }

        public static bool DownloadAndExtractArchive_Wget(string url, string outputDir, BuilderOptions options)
        {
            string fileName = Path.GetFileName(new Uri(url).AbsolutePath);
            string tempFile = Path.Combine(Path.GetTempPath(), fileName);

            Logger.LogInfo("Downloading (wget): " + url);

            try
            {
                var wgetArgs = $"--quiet --no-check-certificate --read-timeout=20 --timeout=15 -t 3 -O \"{tempFile}\" \"{url}\"";
                var wgetStartInfo = new ProcessStartInfo
                {
                    FileName = options.WgetPath,
                    Arguments = wgetArgs,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var wgetProcess = Process.Start(wgetStartInfo))
                {
                    wgetProcess.WaitForExit();

                    string wgetStdErr = wgetProcess.StandardError.ReadToEnd();

                    if (wgetProcess.ExitCode != 0)
                    {
                        Logger.Log("[ERROR] Wget download failed: " + wgetStdErr);
                        return false;
                    }
                }

                Logger.LogInfo("Download complete: " + tempFile);

                return ExtractArchive(tempFile, outputDir, options);
            }
            catch (Exception ex)
            {
                Logger.Log("[ERROR] Download or extract failed: " + ex.Message);
                return false;
            }
            finally
            {
                TryDeleteFile(tempFile);
            }
        }

        // Helper method for extracting archives using 7-zip
        private static bool ExtractArchive(string archivePath, string outputDir, BuilderOptions options)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var startInfo = new ProcessStartInfo
            {
                FileName = options.SevenZipPath,
                Arguments = $"x \"{archivePath}\" -o\"{outputDir}\" -y",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();

                string stdOut = process.StandardOutput.ReadToEnd();
                string stdErr = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0)
                {
                    Logger.Log("[ERROR] Extraction failed: " + stdErr);
                    return false;
                }

                Logger.LogInfo("Extraction complete to: " + outputDir);
                return true;
            }
        }

        public static bool CloneOrUpdateGitRepo(string repoUrl, string buildFolder)
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "retrobat_bios_temp");

            try
            {
                Logger.LogInfo("Downloading (git): " + repoUrl);

                // Delete temp folder if exists
                if (Directory.Exists(tempFolder))
                {
                    if (Directory.Exists(tempFolder))
                    {
                        foreach (var file in Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                File.SetAttributes(file, FileAttributes.Normal);
                                File.Delete(file);
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                Console.WriteLine($"Access denied to file: {file}");
                                // Log or handle as needed
                            }
                        }

                        try
                        {
                            Directory.Delete(tempFolder, true);
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Console.WriteLine($"Access denied to directory: {tempFolder}");
                        }
                    }
                }

                // Clone into temp folder
                var cloneInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = $"clone {repoUrl} \"{tempFolder}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                using (var cloneProcess = Process.Start(cloneInfo))
                {
                    cloneProcess.WaitForExit();
                    string error = cloneProcess.StandardError.ReadToEnd();
                    if (cloneProcess.ExitCode != 0)
                    {
                        Logger.Log($"[ERROR] Git clone failed: {error}");
                        return false;
                    }
                }

                Logger.LogInfo("git downloaded from: " + repoUrl);

                // Ensure build folder exists
                if (!Directory.Exists(buildFolder))
                    Directory.CreateDirectory(buildFolder);

                // Copy all except .git folder
                foreach (var dirPath in Directory.GetDirectories(tempFolder, "*", SearchOption.AllDirectories))
                {
                    if (dirPath.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var targetDir = dirPath.Replace(tempFolder, buildFolder);
                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);
                }

                foreach (var filePath in Directory.GetFiles(tempFolder, "*.*", SearchOption.AllDirectories))
                {
                    if (filePath.Contains(Path.Combine(".git", "")) || filePath.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var targetFile = filePath.Replace(tempFolder, buildFolder);
                    File.Copy(filePath, targetFile, true);
                }

                Logger.LogInfo($"Repository copied to {buildFolder}.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"[ERROR] Failed to clone and copy repo: {ex.Message}");
                return false;
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tempFolder))
                        Directory.Delete(tempFolder, true);
                }
                catch { }
            }
        }

        public static void DeleteGitFiles(string path)
        {
            if (Directory.Exists(path))
            {
                // Delete .git folder if it exists
                string gitPath = Path.Combine(path, ".git");
                if (Directory.Exists(gitPath))
                {
                    try
                    {
                        Directory.Delete(gitPath, true);
                        Logger.LogInfo("Deleted .git folder from " + path);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("[ERROR] Failed to delete .git folder: " + ex.Message);
                    }

                    var gitFiles = Directory.GetFiles(path, ".git", SearchOption.AllDirectories);
                    foreach (var file in gitFiles)
                    {
                        try
                        {
                            File.SetAttributes(file, FileAttributes.Normal);
                            File.Delete(file);
                            Console.WriteLine($"Deleted file: {file}");
                            Logger.LogInfo("Deleted .git files from " + path);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log("[ERROR] Failed to delete .git files: " + ex.Message);
                        }
                    }
                }
            }
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
            }
            catch
            {
                // ignore
            }
        }

        public static void ExtractZipWith7z(string sevenZipExe, string zipFilePath, string outputDir)
        {
            var process = new Process();
            process.StartInfo.FileName = sevenZipExe;
            process.StartInfo.Arguments = $"x \"{zipFilePath}\" -o\"{outputDir}\" -y";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"7z extraction failed: {error}");
            }
        }

        public static string NormalizePath(string path)
        {
            return path.Trim().TrimStart('\\').Replace('\\', Path.DirectorySeparatorChar);
        }
    }
}
