using System;
using System.Windows.Forms;

namespace InstallerTemplate
{
    public partial class InstallerForm : Form
    {
        string zipFilePath;

        public InstallerForm(string zipPath)
        {
            InitializeComponent();
            zipFilePath = zipPath;
        }
    }
}