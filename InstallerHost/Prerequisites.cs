using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstallerHost
{
    public class PrerequisiteControl : UserControl
    {
        private MainForm mainForm;

        private Label lblIntro;
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

        private readonly Dictionary<string, string> vcRedistResources = new Dictionary<string, string>()
        {
            // VC++ 2005
            { "InstallerHost.resources.vcredist2005_x64.exe", "/q" },
            { "InstallerHost.resources.vcredist2005_x86.exe", "/q" },

            // VC++ 2008
            { "InstallerHost.resources.vcredist2008_x64.exe", "/qb" },
            { "InstallerHost.resources.vcredist2008_x86.exe", "/qb" },

            // VC++ 2010
            { "InstallerHost.resources.vcredist2010_x64.exe", "/passive /norestart" },
            { "InstallerHost.resources.vcredist2010_x86.exe", "/passive /norestart" },

            // VC++ 2012
            { "InstallerHost.resources.vcredist2012_x64.exe", "/passive /norestart" },
            { "InstallerHost.resources.vcredist2012_x86.exe", "/passive /norestart" },

            // VC++ 2013
            { "InstallerHost.resources.vcredist2013_x64.exe", "/passive /norestart" },
            { "InstallerHost.resources.vcredist2013_x86.exe", "/passive /norestart" },

            // VC++ 2015–2022
            { "InstallerHost.resources.vcredist2015_2017_2019_2022_x64.exe", "/passive /norestart" },
            { "InstallerHost.resources.vcredist2015_2017_2019_2022_x86.exe", "/passive /norestart" },
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

            this.ResumeLayout(false);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (!installationComplete)
            {
                btnNext.Enabled = false;
                btnBack.Enabled = false;
                btnCancel.Enabled = false;

                int totalSteps = 0;
                if (chkDirectX.Checked)
                    totalSteps++;
                if (chkVCpp.Checked)
                    totalSteps += vcRedistResources.Count;

                progressBar.Value = 0;
                progressBar.Maximum = totalSteps;

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
                    InstallDirectXFromEmbedded();
                }

                if (chkVCpp.Checked)
                {
                    Logger.Log("Launching VC++ installer...");
                    InstallVCppAllAsync();
                }
            }
            catch (Exception ex)
            {
                e.Result = ex;  // Pass exception to completed handler
            }
        }

        private void InstallerWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Exception ex)
            {
                Logger.Log("Installation error: " + ex.Message);
                MessageBox.Show("Installation failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnNext.Enabled = true;
                btnBack.Enabled = true;
                btnCancel.Enabled = true;
            }
            else
            {
                installationComplete = true;
                btnNext.Enabled = true;
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

        private void InstallDirectXFromEmbedded()
        {
            string resourceName = "InstallerHost.resources.directx_Jun2010_redist.exe";

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

                ExtractEmbeddedFile(resourceName, redistExePath);

                var extractProcess = new Process();
                extractProcess.StartInfo.FileName = redistExePath;
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

                var installProcess = new Process();
                installProcess.StartInfo.FileName = dxSetupPath;
                installProcess.StartInfo.Arguments = "";
                installProcess.StartInfo.UseShellExecute = true;
                installProcess.StartInfo.CreateNoWindow = true;

                installProcess.Start();
                installProcess.WaitForExit();
                Logger.Log($"DirectX installation finished with exit code: {installProcess.ExitCode}");
                UpdateProgressBarSafe();
            }
            catch (Exception ex)
            {
                Logger.Log("DirectX installation failed: " + ex.Message);
                MessageBox.Show("DirectX installation failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InstallVCppAllAsync()
        {
            foreach (var res in vcRedistResources)
            {
                string resourceName = res.Key;
                string args = res.Value;

                string tempExtractPathVC = Path.Combine(Path.GetTempPath(), "vcPackageTemp");
                if (Directory.Exists(tempExtractPathVC))
                {
                    try { Directory.Delete(tempExtractPathVC, true); } catch { }
                }
                string redistExePath = Path.Combine(tempExtractPathVC, resourceName);
                if (File.Exists(redistExePath))
                {
                    try { File.Delete(redistExePath); } catch { }
                }

                try
                {
                    Directory.CreateDirectory(tempExtractPathVC);
                }
                catch { }

                ExtractEmbeddedFile(resourceName, redistExePath);

                try
                {
                    var process = new Process();
                    process.StartInfo.FileName = redistExePath;
                    process.StartInfo.Arguments = args;
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.CreateNoWindow = false;

                    process.Start();
                    process.WaitForExit();

                    Logger.Log($"Installed {res} with exit code {process.ExitCode}");
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to install {res}: {ex.Message}");
                    MessageBox.Show($"Error installing {res}:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                UpdateProgressBarSafe();
            }
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
    }
}
