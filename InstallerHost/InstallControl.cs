using ICSharpCode.SharpZipLib.Zip;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstallerHost
{
    public partial class InstallControl : UserControl
    {
        private MainForm mainForm;        
        private BackgroundWorker worker;

        private TextBox txtFolder;
        private Button btnBrowse;
        private Button btnInstall;
        private ProgressBar progressBar;
        private Button btnCancel;
        private Button btnBack;
        private Label txtInfo;
        private Label lblFolderHint;
        private Allegoria.Controls.WizardPanel wizardHeader;
        private HorizontalLineCtrl horizontalLineCtrl1;
        private Label lblSelectFolder;

        public InstallControl(MainForm main)
        {
            mainForm = main;
            InitializeComponent();

            wizardHeader.Text = Texts.GetString("InstallTitle");
            txtInfo.Text = Texts.GetString("InstallInfo");
            lblSelectFolder.Text = Texts.GetString("SelectFolder");
            btnBrowse.Text = Texts.GetString("Browse...");
            lblFolderHint.Text = Texts.GetString("InstallFolderHint");
            btnCancel.Text = Texts.GetString("Cancel");
            btnInstall.Text = Texts.GetString("Install");
            btnBack.Text = Texts.GetString("< Back");            
        }

        private void InitializeComponent()
        {
            this.txtInfo = new System.Windows.Forms.Label();
            this.lblSelectFolder = new System.Windows.Forms.Label();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblFolderHint = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnInstall = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.horizontalLineCtrl1 = new InstallerHost.HorizontalLineCtrl();
            this.wizardHeader = new Allegoria.Controls.WizardPanel();
            this.SuspendLayout();
            // 
            // txtInfo
            // 
            this.txtInfo.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.txtInfo.Location = new System.Drawing.Point(21, 80);
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Size = new System.Drawing.Size(515, 59);
            this.txtInfo.TabIndex = 1;
            this.txtInfo.Text = "The installer program will install RetroBat in the folder below.\r\nTo continue, cl" +
    "ick Next. If you want to specify another folder, Click Browse.";
            // 
            // lblSelectFolder
            // 
            this.lblSelectFolder.AutoSize = true;
            this.lblSelectFolder.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSelectFolder.Location = new System.Drawing.Point(21, 152);
            this.lblSelectFolder.Name = "lblSelectFolder";
            this.lblSelectFolder.Size = new System.Drawing.Size(155, 13);
            this.lblSelectFolder.TabIndex = 2;
            this.lblSelectFolder.Text = "Select the installation folder:";
            // 
            // txtFolder
            // 
            this.txtFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolder.Location = new System.Drawing.Point(23, 178);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(427, 20);
            this.txtFolder.TabIndex = 3;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(453, 177);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(80, 25);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Click += new System.EventHandler(this.BtnBrowse_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(22, 389);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(510, 20);
            this.progressBar.TabIndex = 5;
            this.progressBar.Visible = false;
            // 
            // lblFolderHint
            // 
            this.lblFolderHint.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFolderHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFolderHint.Location = new System.Drawing.Point(20, 214);
            this.lblFolderHint.Name = "lblFolderHint";
            this.lblFolderHint.Size = new System.Drawing.Size(512, 133);
            this.lblFolderHint.TabIndex = 6;
            this.lblFolderHint.Text = "The program requires at least 3.38 GB of free disk space.\r\n\r\nDo not use folders w" +
    "ith spaces or special characters.";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(458, 439);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnInstall
            // 
            this.btnInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInstall.Location = new System.Drawing.Point(377, 439);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 26);
            this.btnInstall.TabIndex = 8;
            this.btnInstall.Text = "Install";
            this.btnInstall.Click += new System.EventHandler(this.BtnInstall_Click);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Location = new System.Drawing.Point(296, 439);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 26);
            this.btnBack.TabIndex = 9;
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
            this.horizontalLineCtrl1.TabIndex = 12;
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
            this.wizardHeader.TabIndex = 11;
            this.wizardHeader.Title = "InstallTitle";
            // 
            // InstallControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.horizontalLineCtrl1);
            this.Controls.Add(this.wizardHeader);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.lblSelectFolder);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblFolderHint);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.btnBack);
            this.Name = "InstallControl";
            this.Size = new System.Drawing.Size(548, 479);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            mainForm.ShowPrerequisites(false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            txtFolder.Text = "C:\\RetroBat";
            ActiveControl = btnInstall;  
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
                if (fbd.ShowDialog() == DialogResult.OK)
                    txtFolder.Text = fbd.SelectedPath;
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
                 //   MessageBox.Show(Texts.GetString("FolderNotEmpty"));
                //    return;
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

            progressBar.Visible = true;
            btnBack.Enabled = false;
            btnInstall.Enabled = false;
            btnBrowse.Enabled = false;

            worker = new BackgroundWorker() { WorkerReportsProgress = true };

            worker.DoWork += (workSender, workArgs) =>
            {
                try
                {
                    string destinationPath = txtFolder.Text;

                    using (var stream = GetEmbeddedZipStream())
                        ExtractZipStreamToFolder(stream, destinationPath);                    
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
                    return;
                }
                    
                progressBar.Value = progressArgs.ProgressPercentage;
            };

            worker.RunWorkerCompleted += (completeSender, completeArgs) =>
            {
                btnInstall.Enabled = true;
                btnBrowse.Enabled = true;
                btnBack.Enabled = true;
                progressBar.Visible = false;

                if (completeArgs.Result is Exception ex)
                {
#if DEBUG
                    if (MessageBox.Show(this, Texts.GetString("InstallFail") + ex.Message, null, MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                        mainForm.ShowFinish(txtFolder.Text);
#else
                    MessageBox.Show(this, Texts.GetString("InstallFail") + ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
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
                Application.Exit();
        }

        private Stream GetEmbeddedZipStream()
        {
            string exePath = Application.ExecutablePath;

            var fs = new FileStream(exePath, FileMode.Open, FileAccess.Read);
            if (fs.Length < 8)
                throw new Exception("Invalid installer: file too small.");

            // Read ZIP length stored in the last 8 bytes
            fs.Seek(-8, SeekOrigin.End);
            byte[] lengthBytes = new byte[8];
            int read = fs.Read(lengthBytes, 0, 8);
            if (read != 8)
                throw new Exception("Failed to read zip length footer.");

            long zipLength = BitConverter.ToInt64(lengthBytes, 0);
            long zipStart = fs.Length - zipLength - 8;

            if (zipLength <= 0 || zipStart < 0)
                throw new Exception("Invalid ZIP length in installer footer.");

            // Copy exactly zipLength bytes to output ZIP
            fs.Seek(zipStart, SeekOrigin.Begin);

            return new SubStream(fs, zipStart, zipLength);
        }

        class SubStream : Stream
        {
            private readonly Stream _baseStream;
            private readonly long _start;
            private readonly long _length;
            private long _position;

            public SubStream(Stream baseStream, long start, long length)
            {
                _baseStream = baseStream;
                _start = start;
                _length = length;
                _position = 0;

                _baseStream.Seek(_start, SeekOrigin.Begin);
            }

            public override bool CanRead => _baseStream.CanRead;
            public override bool CanSeek => _baseStream.CanSeek;
            public override bool CanWrite => false;

            public override long Length => _length;

            public override long Position
            {
                get => _position;
                set => Seek(value, SeekOrigin.Begin);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                long remaining = _length - _position;
                if (remaining <= 0)
                    return 0;

                if (count > remaining)
                    count = (int)remaining;

                int read = _baseStream.Read(buffer, offset, count);
                _position += read;
                return read;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                long newPos;

                if (origin == SeekOrigin.Begin)
                    newPos = offset;
                else if (origin == SeekOrigin.Current)
                    newPos = _position + offset;
                else if (origin == SeekOrigin.End)
                    newPos = _length + offset;
                else
                    throw new ArgumentOutOfRangeException("origin");

                if (newPos < 0 || newPos > _length)
                    throw new ArgumentOutOfRangeException(nameof(offset));

                _baseStream.Seek(_start + newPos, SeekOrigin.Begin);
                _position = newPos;
                return _position;
            }

            public override void Flush() => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        private void ExtractZipStreamToFolder(Stream fs, string destinationFolder)
        {
            using (var zipFile = new ZipFile(fs))
            {
                zipFile.IsStreamOwner = true;
                zipFile.UseZip64 = UseZip64.On;

                long totalSize = zipFile.Cast<ZipEntry>()
                                        .Where(e => e.IsFile)
                                        .Sum(e => e.Size);

                long extractedSize = 0;
                int lastPercent = -1;

                foreach (ZipEntry entry in zipFile)
                {
                    string entryName = entry.Name;
                    string fullPath = Path.Combine(destinationFolder, entryName);

                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(fullPath);
                        continue;
                    }

                    string dirPath = Path.GetDirectoryName(fullPath);
                    if (!string.IsNullOrEmpty(dirPath))
                        Directory.CreateDirectory(dirPath);

                    using (Stream zipStream = zipFile.GetInputStream(entry))
                    using (FileStream outputStream = File.Create(fullPath))
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, bytesRead);
                            extractedSize += bytesRead;

                            if (totalSize > 0)
                            {
                                int percent = (int)((extractedSize * 100) / totalSize);
                                if (percent != lastPercent)
                                {
                                    worker?.ReportProgress(percent);
                                    lastPercent = percent;
                                }
                            }
                        }
                    }
                }
            }
        }        
    }
}
