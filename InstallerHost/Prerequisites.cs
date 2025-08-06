using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ionic.Zip;

namespace InstallerHost
{
    public class PrerequisiteControl : UserControl
    {
        private MainForm mainForm;

        private Label lblIntro;
        private Label statusLabel;
        private CheckBox chkVCpp;
        private CheckBox chkDirectX;
        private Button btnCancel;
        private Button btnNext;
        private Button btnBack;
        private BackgroundWorker installerWorker;
        private ProgressBar progressBar;
        private bool installationComplete = false;

        public PrerequisiteControl(MainForm main)
        {
            mainForm = main;
            InitializeComponent();
            this.Resize += PrerequisiteControl_Resize;
        }

        private readonly Dictionary<string, InstallerInfo> vcRedistResources = new Dictionary<string, InstallerInfo>
        {
            { "vcredist2005_x64.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/q") },
            { "vcredist2005_x86.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/q") },
            { "vcredist2008_x64.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/qb") },
            { "vcredist2008_x86.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/qb") },
            { "vcredist2010_x64.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/passive /norestart") },
            { "vcredist2010_x86.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/passive /norestart") },
            { "vcredist2012_x64.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/passive /norestart") },
            { "vcredist2012_x86.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/passive /norestart") },
            { "vcredist2013_x64.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/passive /norestart") },
            { "vcredist2013_x86.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/passive /norestart") },
            { "vcredist2015_2017_2019_2022_x64.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/passive /norestart") },
            { "vcredist2015_2017_2019_2022_x86.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/passive /norestart") }
        };

        private readonly Dictionary<string, InstallerInfo> dxRedistResources = new Dictionary<string, InstallerInfo>
        {
            { "directx_Jun2010_redist.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/q") }
        };

        private void InitializeComponent()
        {
            this.SuspendLayout();

            int leftMargin = 20;

            lblIntro = new Label()
            {
                Text = Texts.GetString("PrerequisiteIntro"),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Left = leftMargin,
                Top = 20,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            chkVCpp = new CheckBox()
            {
                Text = Texts.GetString("vcText"),
                Left = leftMargin,
                Top = lblIntro.Bottom + 20,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Checked = true
            };

            chkDirectX = new CheckBox()
            {
                Text = Texts.GetString("dx9text"),
                Left = leftMargin,
                Top = chkVCpp.Bottom + 10,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Checked = true
            };

            progressBar = new ProgressBar()
            {
                Left = chkDirectX.Left,
                Top = chkDirectX.Bottom + 20,
                Width = 400,
                Height = 20,
                Minimum = 0,
                Maximum = (chkDirectX.Checked ? 1 : 0) + (chkVCpp.Checked ? vcRedistResources.Count : 0),
                Value = 0,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            statusLabel = new Label()
            {
                Left = progressBar.Left,
                Top = progressBar.Bottom + 10,
                Width = progressBar.Width,
                Height = 30,
                Text = "",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            btnCancel = new Button()
            {
                Text = Texts.GetString("Cancel"),
                Width = mainForm.buttonWidth,
                Height = mainForm.buttonHeight,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            };
            btnCancel.Click += BtnCancel_Click;

            btnNext = new Button()
            {
                Text = Texts.GetString("Next"),
                Width = mainForm.buttonWidth,
                Height = mainForm.buttonHeight,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnNext.Click += BtnNext_Click;

            btnBack = new Button()
            {
                Text = Texts.GetString("Back"),
                Width = mainForm.buttonWidth,
                Height = mainForm.buttonHeight,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnBack.Click += (s, e) => mainForm.ShowLicense();

            this.Controls.Add(lblIntro);
            this.Controls.Add(chkVCpp);
            this.Controls.Add(chkDirectX);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnNext);
            this.Controls.Add(btnBack);
            this.Controls.Add(progressBar);
            this.Controls.Add(statusLabel);

            UpdateStatusLabelSafe(Texts.GetString("WaitingSelect"));

            this.ResumeLayout(false);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (!installationComplete)
            {
                btnNext.Enabled = false;
                btnBack.Enabled = false;
                btnCancel.Enabled = false;

                statusLabel.Text = Texts.GetString("DownloadAndInstall");

                installerWorker = new BackgroundWorker();
                installerWorker.DoWork += InstallerWorker_DoWork;
                installerWorker.RunWorkerCompleted += InstallerWorker_RunWorkerCompleted;
                installerWorker.RunWorkerAsync();
            }
            else
            {
                mainForm.ShowInstall();
            }
        }

        private void InstallerWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                if (chkDirectX.Checked)
                {
                    Logger.Log("Launching DirectX installer...");
                    InstallDirectX();
                }

                if (chkVCpp.Checked)
                {
                    Logger.Log("Launching VC++ installer...");
                    InstallVCppAll();
                }

                int totalSteps = 0;
                if (chkDirectX.Checked)
                    totalSteps++;
                if (chkVCpp.Checked)
                    totalSteps += vcRedistResources.Count;

                progressBar.Value = 0;
                progressBar.Maximum = totalSteps;
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void InstallerWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Exception ex)
            {
                Logger.Log("Installation error: " + ex.Message);
                MessageBox.Show(Texts.GetString("InstallFail") + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                btnNext.Enabled = true;
                btnBack.Enabled = true;
                btnCancel.Enabled = true;
            }
            else
            {
                UpdateStatusLabelSafe(Texts.GetString("InstallComplete"));

                installationComplete = true;
                btnNext.Enabled = true;
                btnCancel.Enabled = true;
            }
        }

        private void PrerequisiteControl_Resize(object sender, EventArgs e)
        {
            btnCancel.Top = this.ClientSize.Height - btnCancel.Height - mainForm.bottomMargin;
            btnCancel.Left = this.ClientSize.Width - btnCancel.Width - mainForm.rightMargin;

            btnNext.Top = btnCancel.Top;
            btnNext.Left = btnCancel.Left - btnNext.Width - mainForm.spacing;

            btnBack.Top = btnCancel.Top;
            btnBack.Left = btnNext.Left - btnBack.Width - mainForm.spacing;
        }

        private void InstallDirectX()
        {
            string wgetPath = ExtractWgetExecutable();

            string tempRoot = Path.Combine(Path.GetTempPath(), "DXDownloads");
            if (!Directory.Exists(tempRoot))
                Directory.CreateDirectory(tempRoot);

            foreach (var kvp in dxRedistResources)
            {
                string zipFileName = kvp.Key;
                InstallerInfo info = kvp.Value;

                string zipLocalPath = Path.Combine(tempRoot, zipFileName);
                string extractPath = Path.Combine(tempRoot, Path.GetFileNameWithoutExtension(zipFileName));
                string fullUrl = kvp.Value.Url + zipFileName;
                
                try
                {
                    string tempExtractPathDX = Path.Combine(Path.GetTempPath(), "DirectXRedistTemp_" + Guid.NewGuid());
                    if (Directory.Exists(tempExtractPathDX))
                    {
                        try { Directory.Delete(tempExtractPathDX, true); } catch { }
                    }
                    string redistExePath = Path.Combine(tempExtractPathDX, "directx_Jun2010_redist.exe");
                    if (File.Exists(redistExePath))
                    {
                        try { File.Delete(redistExePath); } catch { }
                    }
                    string tempExtractPath = Path.Combine(Path.GetTempPath(), "DirectXRedist");
                    if (Directory.Exists(tempExtractPath))
                    {
                        try { Directory.Delete(tempExtractPath, true); } catch { }
                    }

                    try
                    {
                        Directory.CreateDirectory(tempExtractPathDX);
                        Directory.CreateDirectory(tempExtractPath);
                    }
                    catch { }

                    UpdateStatusLabelSafe(Texts.GetString("Downloading") + " " + zipFileName + "...");

                    Logger.Log($"Downloading ZIP from {fullUrl}...");
                    DownloadWithWget(wgetPath, fullUrl, zipLocalPath);
                    Logger.Log("Download complete.");

                    ExtractZipDotNetZip(zipLocalPath, extractPath);
                    Logger.Log("Extraction complete.");

                    string exePath = Path.Combine(extractPath, zipFileName.Replace(".zip", ".exe"));
                    if (!File.Exists(exePath))
                        throw new FileNotFoundException("Installer not found: " + exePath);

                    UpdateStatusLabelSafe(Texts.GetString("Extracting") + " " + zipFileName + "...");

                    var extractProcess = new Process();
                    extractProcess.StartInfo.FileName = exePath;
                    extractProcess.StartInfo.Arguments = $"/Q /T:\"{tempExtractPath}\"";
                    extractProcess.StartInfo.UseShellExecute = false;
                    extractProcess.StartInfo.CreateNoWindow = true;

                    extractProcess.Start();
                    extractProcess.WaitForExit();

                    if (extractProcess.ExitCode != 0)
                    {
                        throw new Exception($"Extraction failed with exit code {extractProcess.ExitCode}");
                    }

                    string dxSetupPath = Path.Combine(tempExtractPath, "DXSETUP.exe");
                    if (!File.Exists(dxSetupPath))
                    {
                        throw new FileNotFoundException("DXSETUP.exe not found after extraction.");
                    }

                    UpdateStatusLabelSafe(Texts.GetString("InstallDX"));

                    var installProcess = new Process();
                    installProcess.StartInfo.FileName = dxSetupPath;
                    installProcess.StartInfo.Arguments = "";
                    installProcess.StartInfo.UseShellExecute = true;

                    installProcess.Start();
                    installProcess.WaitForExit();
                    Logger.Log($"DirectX installation finished with exit code: {installProcess.ExitCode}");

                    try { Directory.Delete(zipLocalPath, true); } catch { }
                    try { Directory.Delete(tempExtractPath, true); } catch { }

                    UpdateProgressBarSafe();
                }
                catch (Exception ex)
                {
                    Logger.Log("DirectX installation failed: " + ex.Message);
                    MessageBox.Show("DirectX installation failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try { Directory.Delete(tempRoot, true); } catch { }
            }
        }

        private void InstallVCppAll()
        {
            string wgetPath = ExtractWgetExecutable();

            string tempRoot = Path.Combine(Path.GetTempPath(), "VCDownloads");
            if (!Directory.Exists(tempRoot))
                Directory.CreateDirectory(tempRoot);

            foreach (var kvp in vcRedistResources)
            {
                string zipFileName = kvp.Key;
                InstallerInfo info = kvp.Value;

                string zipLocalPath = Path.Combine(tempRoot, zipFileName);
                string extractPath = Path.Combine(tempRoot, Path.GetFileNameWithoutExtension(zipFileName));
                string fullUrl = kvp.Value.Url + zipFileName;

                try
                {
                    UpdateStatusLabelSafe(Texts.GetString("Downloading") + " " + zipFileName + "...");

                    Logger.Log($"Downloading ZIP from {fullUrl}...");
                    DownloadWithWget(wgetPath, fullUrl, zipLocalPath);
                    Logger.Log("Download and extraction complete.");

                    ExtractZipDotNetZip(zipLocalPath, extractPath);
                    Logger.Log("Extraction complete.");

                    UpdateStatusLabelSafe(Texts.GetString("Installing") + " " + zipFileName + "...");

                    string exePath = Path.Combine(extractPath, zipFileName.Replace(".zip", ".exe"));
                    if (!File.Exists(exePath))
                        throw new FileNotFoundException("Installer not found: " + exePath);

                    var process = new Process();
                    process.StartInfo.FileName = exePath;
                    process.StartInfo.Arguments = info.Arguments;
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.CreateNoWindow = false;

                    Logger.Log($"Running installer: {exePath} {info.Arguments}");
                    process.Start();
                    process.WaitForExit();

                    try { Directory.Delete(zipLocalPath, true); } catch { }
                    try { Directory.Delete(extractPath, true); } catch { }

                    Logger.Log($"Installer finished with code {process.ExitCode}");
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to install {zipFileName}: {ex.Message}");
                    MessageBox.Show($"Error installing {zipFileName}:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                UpdateProgressBarSafe();
            }

            try { Directory.Delete(tempRoot, true); } catch { }
        }

        public static string ExtractEmbeddedFile(string resourceName, string outputFile)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new Exception("Resource not found: " + resourceName);

                using (var fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }

            return outputFile;
        }

        public void BtnCancel_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(Texts.GetString("CancelSure"), Texts.GetString("CancelButtonTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void UpdateProgressBarSafe()
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() =>
                {
                    progressBar.Value = Math.Min(progressBar.Maximum, progressBar.Value + 1);
                }));
            }
            else
            {
                progressBar.Value = Math.Min(progressBar.Maximum, progressBar.Value + 1);
            }
        }

        private string ExtractWgetExecutable()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "wget.exe");
            var resourceName = "InstallerHost.resources.wget.exe";

            if (!File.Exists(tempPath))
            {
                using (var stream = typeof(PrerequisiteControl).Assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        throw new Exception("wget.exe resource not found.");

                    using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }

            return tempPath;
        }

        private void DownloadWithWget(string wgetPath, string url, string outputPath)
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = wgetPath;
                    process.StartInfo.Arguments = $"\"{url}\" -O \"{outputPath}\" --no-check-certificate";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    process.OutputDataReceived += (s, e) => { if (e.Data != null) Logger.Log("wget stdout: " + e.Data); };
                    process.ErrorDataReceived += (s, e) => { if (e.Data != null) Logger.Log("wget stderr: " + e.Data); };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();

                    if (process.ExitCode != 0)
                        throw new Exception($"wget failed with exit code {process.ExitCode}");
                }
            }
            catch { Logger.Log("Error Downloading" + url); }
        }

        private void ExtractZipDotNetZip(string zipPath, string extractTo)
        {
            using (ZipFile zip = ZipFile.Read(zipPath))
            {
                foreach (ZipEntry entry in zip)
                {
                    entry.Extract(extractTo, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        private void UpdateStatusLabelSafe(string text)
        {
            if (statusLabel.InvokeRequired)
            {
                statusLabel.Invoke(new Action(() => statusLabel.Text = text));
            }
            else
            {
                statusLabel.Text = text;
            }
        }
    }

    public class InstallerInfo
    {
        public string Url { get; set; }
        public string Arguments { get; set; }

        public InstallerInfo(string url, string arguments)
        {
            Url = url;
            Arguments = arguments;
        }
    }
}
