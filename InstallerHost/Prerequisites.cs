using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace InstallerHost
{
    public class PrerequisiteControl : UserControl
    {
        private MainForm mainForm;
        private Label lblAllInstalled;
        private Label statusLabel;
        private CheckBox chkVCpp;
        private CheckBox chkDirectX;
        private CheckBox chkDokany;
        private Button btnCancel;
        private Button btnNext;
        private Button btnBack;
        private BackgroundWorker installerWorker;
        private ProgressBar progressBar;
        private bool installationComplete = false;
        private Allegoria.Controls.WizardPanel wizardHeader;
        private HorizontalLineCtrl horizontalLineCtrl1;

        public PrerequisiteControl(MainForm main)
        {
            mainForm = main;
            InitializeComponent();

            wizardHeader.Text = Texts.GetString("PrerequisiteIntro");
            lblAllInstalled.Text = Texts.GetString("All prerequisites installed");
            chkVCpp.Text = Texts.GetString("vcText");
            chkDirectX.Text = Texts.GetString("dx9text");
            chkDokany.Text = Texts.GetString("dokanyText");
            btnCancel.Text = Texts.GetString("Cancel");
            btnNext.Text = Texts.GetString("Next >");
            btnBack.Text = Texts.GetString("< Back");

            UpdateStatusLabelSafe(Texts.GetString("WaitingSelect"));
            UpdatePrerequisiteCheckboxes();            
        }

        public bool SkipIfAllInstalled()
        {
#if DEBUG
            return false;
#endif 
            return !chkVCpp.Enabled && !chkDirectX.Enabled && !chkDokany.Enabled;
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ActiveControl = btnNext;
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

        private readonly Dictionary<string, InstallerInfo> dokanResources = new Dictionary<string, InstallerInfo>
        {
            { "DokanSetup.zip", new InstallerInfo("http://retrobat.ovh/repo/win64/prerequisites/", "/quiet") }
        };

        private void InitializeComponent()
        {
            this.lblAllInstalled = new System.Windows.Forms.Label();
            this.chkVCpp = new System.Windows.Forms.CheckBox();
            this.chkDirectX = new System.Windows.Forms.CheckBox();
            this.chkDokany = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.statusLabel = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.horizontalLineCtrl1 = new InstallerHost.HorizontalLineCtrl();
            this.wizardHeader = new Allegoria.Controls.WizardPanel();
            this.SuspendLayout();
            // 
            // lblAllInstalled
            // 
            this.lblAllInstalled.AutoSize = true;
            this.lblAllInstalled.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblAllInstalled.Location = new System.Drawing.Point(20, 81);
            this.lblAllInstalled.Name = "lblAllInstalled";
            this.lblAllInstalled.Size = new System.Drawing.Size(176, 20);
            this.lblAllInstalled.TabIndex = 1;
            this.lblAllInstalled.Text = "All prerequisites installed";
            this.lblAllInstalled.Visible = false;
            // 
            // chkVCpp
            // 
            this.chkVCpp.AutoSize = true;
            this.chkVCpp.Checked = true;
            this.chkVCpp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVCpp.Location = new System.Drawing.Point(24, 117);
            this.chkVCpp.Name = "chkVCpp";
            this.chkVCpp.Size = new System.Drawing.Size(59, 17);
            this.chkVCpp.TabIndex = 2;
            this.chkVCpp.Text = "vcText";
            // 
            // chkDirectX
            // 
            this.chkDirectX.AutoSize = true;
            this.chkDirectX.Checked = true;
            this.chkDirectX.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDirectX.Location = new System.Drawing.Point(24, 143);
            this.chkDirectX.Name = "chkDirectX";
            this.chkDirectX.Size = new System.Drawing.Size(60, 17);
            this.chkDirectX.TabIndex = 3;
            this.chkDirectX.Text = "dx9text";
            // 
            // chkDokany
            // 
            this.chkDokany.AutoSize = true;
            this.chkDokany.Location = new System.Drawing.Point(24, 169);
            this.chkDokany.Name = "chkDokany";
            this.chkDokany.Size = new System.Drawing.Size(82, 17);
            this.chkDokany.TabIndex = 4;
            this.chkDokany.Text = "dokanyText";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(23, 300);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(510, 22);
            this.progressBar.TabIndex = 8;
            this.progressBar.Visible = false;
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.Location = new System.Drawing.Point(21, 325);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(512, 23);
            this.statusLabel.TabIndex = 9;
            this.statusLabel.Text = "status";
            this.statusLabel.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(458, 439);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNext.Location = new System.Drawing.Point(377, 439);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 26);
            this.btnNext.TabIndex = 6;
            this.btnNext.Text = "Next >";
            this.btnNext.Click += new System.EventHandler(this.BtnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBack.Location = new System.Drawing.Point(296, 439);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 26);
            this.btnBack.TabIndex = 7;
            this.btnBack.Text = "< Back";
            this.btnBack.Click += new System.EventHandler(this.BtnBack_Click);
            // 
            // horizontalLineCtrl1
            // 
            this.horizontalLineCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontalLineCtrl1.Location = new System.Drawing.Point(0, 427);
            this.horizontalLineCtrl1.Name = "horizontalLineCtrl1";
            this.horizontalLineCtrl1.Size = new System.Drawing.Size(547, 2);
            this.horizontalLineCtrl1.TabIndex = 11;
            this.horizontalLineCtrl1.Text = "horizontalLineCtrl1";
            // 
            // wizardHeader
            // 
            this.wizardHeader.BackColor = System.Drawing.SystemColors.Window;
            this.wizardHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.wizardHeader.Image = global::InstallerHost.Properties.Resources.logo_icon;
            this.wizardHeader.Location = new System.Drawing.Point(0, 0);
            this.wizardHeader.Name = "wizardHeader";
            this.wizardHeader.Size = new System.Drawing.Size(548, 60);
            this.wizardHeader.TabIndex = 10;
            this.wizardHeader.Title = "PrerequisiteIntro";
            // 
            // PrerequisiteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.horizontalLineCtrl1);
            this.Controls.Add(this.wizardHeader);
            this.Controls.Add(this.lblAllInstalled);
            this.Controls.Add(this.chkVCpp);
            this.Controls.Add(this.chkDirectX);
            this.Controls.Add(this.chkDokany);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.statusLabel);
            this.Name = "PrerequisiteControl";
            this.Size = new System.Drawing.Size(548, 479);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            mainForm.ShowLicense();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (!(chkDirectX.Enabled && chkDirectX.Checked) && !(chkVCpp.Enabled && chkVCpp.Checked) && !(chkDokany.Enabled && chkDokany.Checked))
            {
                mainForm.ShowInstall();
                return;
            }

            if (!installationComplete)
            {
                btnNext.Enabled = false;
                btnBack.Enabled = false;
                btnCancel.Enabled = false;

                statusLabel.Text = Texts.GetString("DownloadAndInstall");
                statusLabel.Visible = true;

                int totalSteps = 0;
                if (chkDirectX.Enabled && chkDirectX.Checked)
                    totalSteps++;
                if (chkVCpp.Enabled && chkVCpp.Checked)
                    totalSteps += vcRedistResources.Count;
                if (chkDokany.Enabled && chkDokany.Checked)
                    totalSteps++;

                progressBar.Value = 0;
                progressBar.Maximum = totalSteps;
                progressBar.Visible = true;

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
                if (chkDirectX.Enabled && chkDirectX.Checked)
                {
                    Logger.Log("Launching DirectX installer...");
                    InstallDirectX();
                }

                if (chkVCpp.Enabled && chkVCpp.Checked)
                {
                    Logger.Log("Launching VC++ installer...");
                    InstallVCppAll();
                }

                if (chkDokany.Enabled && chkDokany.Checked)
                {
                    Logger.Log("Launching Dokany installer...");
                    InstallDokany();
                }

                int totalSteps = 0;
                if (chkDirectX.Enabled && chkDirectX.Checked)
                    totalSteps++;
                if (chkVCpp.Enabled && chkVCpp.Checked)
                    totalSteps += vcRedistResources.Count;
                if (chkDokany.Enabled && chkDokany.Checked)
                    totalSteps++;
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
                progressBar.Value = progressBar.Maximum;
            }
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

                    ExtractZipToFolder(zipLocalPath, extractPath);
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

                    ExtractZipToFolder(zipLocalPath, extractPath);
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

        private void InstallDokany()
        {
            string wgetPath = ExtractWgetExecutable();

            string tempRoot = Path.Combine(Path.GetTempPath(), "dokanyDownloads");
            if (!Directory.Exists(tempRoot))
                Directory.CreateDirectory(tempRoot);

            foreach (var kvp in dokanResources)
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

                    ExtractZipToFolder(zipLocalPath, extractPath);
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
                Application.Exit();
        }

        private void UpdateProgressBarSafe()
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() =>
                {
                    if (progressBar.Value < progressBar.Maximum)
                        progressBar.Value++;
                }));
            }
            else
            {
                if (progressBar.Value < progressBar.Maximum)
                    progressBar.Value++;
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

        private void ExtractZipToFolder(string zipFilePath, string destinationFolder, Action<int> progress = null)
        {
            using (FileStream fs = File.OpenRead(zipFilePath))
            using (ZipFile zipFile = new ZipFile(fs))
            {
                long totalSize = 0;
                foreach (ZipEntry entry in zipFile)
                {
                    if (!entry.IsFile)
                        continue;
                    totalSize += entry.Size;
                }

                long extractedSize = 0;

                foreach (ZipEntry entry in zipFile)
                {
                    if (!entry.IsFile)
                        continue;

                    string entryFileName = entry.Name;
                    string fullPath = Path.Combine(destinationFolder, entryFileName);

                    string directoryName = Path.GetDirectoryName(fullPath);
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    using (Stream zipStream = zipFile.GetInputStream(entry))
                    using (FileStream outputStream = File.Create(fullPath))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, bytesRead);
                            extractedSize += bytesRead;

                            if (progress != null)
                            {
                                int percent = (int)((extractedSize * 100) / totalSize);
                                progress(percent);
                            }
                        }
                    }
                }
            }
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

        private void UpdateStatusLabelSafe(string text)
        {
            if (statusLabel.InvokeRequired)
            {
                statusLabel.Invoke(new Action(() => statusLabel.Text = text));
                return;
            }
                
            statusLabel.Text = text;
        }

        private void UpdatePrerequisiteCheckboxes()
        {
            try
            {
                // VC++
                chkVCpp.Enabled = !PrerequisiteDetector.IsVCppFullyInstalled();
                if (!chkVCpp.Enabled)
                    chkVCpp.Checked = true;

                // DirectX
                chkDirectX.Enabled = !PrerequisiteDetector.IsDirectXJun2010Installed();
                if (!chkDirectX.Enabled)
                    chkDirectX.Checked = true;

                // Dokany
                chkDokany.Enabled = !PrerequisiteDetector.IsDokanyInstalled();
                if (!chkDokany.Enabled)
                    chkDokany.Checked = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Error detecting prerequisites: " + ex.Message);

                // fallback to defaults if detection fails
                chkVCpp.Enabled = true;
                chkVCpp.Checked = true;

                chkDirectX.Enabled = true;
                chkDirectX.Checked = true;

                chkDokany.Enabled = true;
                chkDokany.Checked = false;
            }

            // Update progress bar max
            int totalSteps = 0;
            if (chkDirectX.Enabled && chkDirectX.Checked) 
                totalSteps++;
            if (chkVCpp.Enabled && chkVCpp.Checked) 
                totalSteps += vcRedistResources.Count;
            if (chkDokany.Enabled && chkDokany.Checked) 
                totalSteps++;

            progressBar.Maximum = Math.Max(1, totalSteps);
            progressBar.Value = 0;

            lblAllInstalled.Visible = !chkVCpp.Enabled && !chkDirectX.Enabled && !chkDokany.Enabled;
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
