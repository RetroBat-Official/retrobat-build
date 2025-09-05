using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using System.Drawing;

namespace InstallerHost
{
    public partial class InstallControl : UserControl
    {
        private MainForm mainForm;
        private string zipFilePath;
        private BackgroundWorker worker;

        private TextBox txtFolder;
        private Button btnBrowse;
        private Button btnInstall;
        private ProgressBar progressBar;
        private Button btnCancel;
        private Button btnBack;
        private Label lblTitle;
        private Label txtInfo;
        private Label lblFolderHint;

        public InstallControl(MainForm main)
        {
            mainForm = main;
            InitializeComponent();

            this.Load += InstallControl_Load;
            this.Resize += InstallControl_Resize;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            int leftMargin = 20;
            int rightMargin = 20;
            int spacing = 10;

            // Title Label
            lblTitle = new Label()
            {
                Text = Texts.GetString("InstallTitle"),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Left = leftMargin,
                Top = 20,
                AutoSize = true,  // size will be adjusted automatically here
            };

            // Info Label - set AutoSize = false here, width and height will be set on Resize
            txtInfo = new Label()
            {
                Text = Texts.GetString("InstallInfo"),
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                Left = leftMargin,
                Top = lblTitle.Bottom + spacing,
                AutoSize = false,
                // Width and Height will be set in Resize event handler
            };

            // Folder selection Label
            Label lblSelectFolder = new Label()
            {
                Text = Texts.GetString("SelectFolder"),
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                Left = leftMargin,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
            };

            // TextBox for folder path
            txtFolder = new TextBox()
            {
                Left = leftMargin,
                Width = 200,  // initial width, will be resized in Resize event
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };

            // Browse button
            btnBrowse = new Button()
            {
                Text = Texts.GetString("Browse"),
                Width = 80,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };
            btnBrowse.Click += BtnBrowse_Click;

            // Progress bar
            progressBar = new ProgressBar()
            {
                Height = 20,
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };

            // Hint Label
            lblFolderHint = new Label()
            {
                Text = Texts.GetString("InstallFolderHint"),
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                AutoSize = false,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };

            // Buttons Cancel, Install, Back
            btnCancel = new Button()
            {
                Text = Texts.GetString("Cancel"),
                Width = 80,
                Height = 30,
            };
            btnCancel.Click += BtnCancel_Click;

            btnInstall = new Button()
            {
                Text = Texts.GetString("Install"),
                Width = 80,
                Height = 30,
            };
            btnInstall.Click += BtnInstall_Click;

            btnBack = new Button()
            {
                Text = Texts.GetString("Back"),
                Width = 80,
                Height = 30,
            };
            btnBack.Click += (s, e) => mainForm.ShowPrerequisites();

            // Add controls to this UserControl
            this.Controls.Add(lblTitle);
            this.Controls.Add(txtInfo);
            this.Controls.Add(lblSelectFolder);
            this.Controls.Add(txtFolder);
            this.Controls.Add(btnBrowse);
            this.Controls.Add(progressBar);
            this.Controls.Add(lblFolderHint);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnInstall);
            this.Controls.Add(btnBack);

            this.ResumeLayout(false);
            this.PerformLayout(); // Perform layout now for autosize labels
        }

        private void InstallControl_Resize(object sender, EventArgs e)
        {
            int leftMargin = 20;
            int rightMargin = 20;
            int spacing = 10;
            int extraSpacingBelowInfo = 15;

            int controlWidth = this.ClientSize.Width - leftMargin - rightMargin;
            if (controlWidth < 100) controlWidth = 100;

            // Position and size Title label
            lblTitle.Left = leftMargin;
            lblTitle.Top = 20;
            lblTitle.Width = controlWidth;

            // Position and size Info label (txtInfo)
            txtInfo.Left = leftMargin;
            txtInfo.Top = lblTitle.Bottom + spacing;

            // Crucial: set width and update height for wrapping text
            txtInfo.MaximumSize = new Size(controlWidth, 0);
            txtInfo.Width = controlWidth;
            txtInfo.Height = txtInfo.PreferredHeight;

            // Position SelectFolder label
            var lblSelectFolder = this.Controls.OfType<Label>().FirstOrDefault(l => l.Text == Texts.GetString("SelectFolder"));
            if (lblSelectFolder != null)
            {
                lblSelectFolder.Left = leftMargin;
                lblSelectFolder.Top = txtInfo.Bottom + extraSpacingBelowInfo;
            }

            // Position and size Folder TextBox
            txtFolder.Left = leftMargin;
            txtFolder.Top = lblSelectFolder != null ? lblSelectFolder.Bottom + 5 : txtInfo.Bottom + extraSpacingBelowInfo;

            // Width calculation for folder TextBox - leave room for Browse button + spacing
            txtFolder.Width = Math.Max(200, controlWidth - btnBrowse.Width - spacing);

            // Position Browse button
            btnBrowse.Left = txtFolder.Right + spacing;
            btnBrowse.Top = txtFolder.Top - 2;

            // Position and size Progress bar
            progressBar.Left = leftMargin;
            progressBar.Top = txtFolder.Bottom + spacing;
            progressBar.Width = controlWidth;

            // Position Folder hint label
            lblFolderHint.Left = leftMargin;
            lblFolderHint.Top = progressBar.Bottom + 5;
            lblFolderHint.Width = controlWidth;

            // Position bottom buttons (Cancel, Install, Back)
            int bottom = this.ClientSize.Height - mainForm.bottomMargin - mainForm.buttonHeight;

            btnCancel.Top = bottom;
            btnCancel.Left = this.ClientSize.Width - mainForm.rightMargin - btnCancel.Width;

            btnInstall.Top = bottom;
            btnInstall.Left = btnCancel.Left - mainForm.spacing - btnInstall.Width;

            btnBack.Top = bottom;
            btnBack.Left = btnInstall.Left - mainForm.spacing - btnBack.Width;
        }

        private void InstallControl_Load(object sender, EventArgs e)
        {
            string defaultPath = "C:\\RetroBat";
            txtFolder.Text = defaultPath;

            zipFilePath = Path.Combine(Path.GetTempPath(), "embedded_installer.zip");

            try
            {
                Logger.Log("Extracting zip content.");
                ExtractEmbeddedZip(zipFilePath);
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to extract installer data.");
                MessageBox.Show(Texts.GetString("ExtractFail") + ex.Message);
                Application.Exit();
            }

            // Force layout to update sizes properly after load
            InstallControl_Resize(this, EventArgs.Empty);
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtFolder.Text = fbd.SelectedPath;
                }
            }
        }

        private void BtnInstall_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFolder.Text))
            {
                Logger.Log("[WARNING] No installation folder selected.");
                MessageBox.Show(Texts.GetString("ValidFolder"));
                return;
            }

            string selectedPath = txtFolder.Text;
            if (Directory.Exists(selectedPath))
            {
                if (Directory.EnumerateFileSystemEntries(selectedPath).Any())
                {
                    Logger.Log("[WARNING] Installation folder not empty.");
                    MessageBox.Show(Texts.GetString("FolderNotEmpty"));
                    return;
                }
            }
            else
            {
                try
                {
                    Logger.Log("[INFO] Creating installation folder.");
                    Directory.CreateDirectory(selectedPath);
                }
                catch (Exception ex)
                {
                    Logger.Log("[WARNING] Unable to create installation folder.");
                    MessageBox.Show(Texts.GetString("FailedFolder") + ex.Message);
                    return;
                }
            }

            btnInstall.Enabled = false;
            btnBrowse.Enabled = false;

            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            worker.DoWork += (workSender, workArgs) =>
            {
                try
                {
                    string destinationPath = txtFolder.Text;
                    ExtractZipToFolder(zipFilePath, destinationPath);
                }
                catch (Exception ex)
                {
                    workArgs.Result = ex;
                }
            };

            worker.ProgressChanged += (progressSender, progressArgs) =>
            {
                if (progressBar.InvokeRequired)
                {
                    progressBar.Invoke(new Action(() => progressBar.Value = progressArgs.ProgressPercentage));
                }
                else
                {
                    progressBar.Value = progressArgs.ProgressPercentage;
                }
            };

            worker.RunWorkerCompleted += (completeSender, completeArgs) =>
            {
                btnInstall.Enabled = true;
                btnBrowse.Enabled = true;

                if (completeArgs.Result is Exception ex)
                {
                    MessageBox.Show(Texts.GetString("InstallFail") + ex.Message);
                }
                else
                {
                    Logger.Log("Installation successful, showing finish screen.");
                    mainForm.ShowFinish(txtFolder.Text);
                }
            };

            worker.RunWorkerAsync();
        }

        public void BtnCancel_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(Texts.GetString("CancelSure"), Texts.GetString("CancelButtonTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void ExtractEmbeddedZip(string outputZipPath)
        {
            string exePath = Application.ExecutablePath;
            byte[] exeBytes = File.ReadAllBytes(exePath);

            // ZIP files start with "PK" signature: 0x50, 0x4B, 0x03, 0x04
            byte[] zipHeader = { 0x50, 0x4B, 0x03, 0x04 };
            int index = FindBytes(exeBytes, zipHeader);

            if (index < 0)
                throw new Exception("No ZIP header found in executable.");

            // Copy everything from the header to the end of the exe
            byte[] zipBytes = new byte[exeBytes.Length - index];
            Array.Copy(exeBytes, index, zipBytes, 0, zipBytes.Length);

            File.WriteAllBytes(outputZipPath, zipBytes);
        }

        private int FindBytes(byte[] buffer, byte[] pattern)
        {
            for (int i = 0; i <= buffer.Length - pattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (buffer[i + j] != pattern[j]) { match = false; break; }
                }
                if (match) return i;
            }
            return -1;
        }

        private void ExtractZipToFolder(string zipFilePath, string destinationFolder)
        {
            using (FileStream fs = File.OpenRead(zipFilePath))
            using (ZipFile zipFile = new ZipFile(fs))
            {
                // Calcule la taille totale des fichiers seulement
                long totalSize = zipFile.Cast<ZipEntry>()
                                        .Where(e => e.IsFile)
                                        .Sum(e => e.Size);

                long extractedSize = 0;

                foreach (ZipEntry entry in zipFile)
                {
                    string fullPath = Path.Combine(destinationFolder, entry.Name);

                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(fullPath);
                        continue;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                    using (Stream zipStream = zipFile.GetInputStream(entry))
                    using (FileStream outputStream = File.Create(fullPath))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, bytesRead);
                            extractedSize += bytesRead;

                            // On reporte la progression uniquement pour les fichiers
                            int percent = (int)((extractedSize * 100) / totalSize);
                            worker?.ReportProgress(percent);
                        }
                    }
                }
            }
        }
    }
}
