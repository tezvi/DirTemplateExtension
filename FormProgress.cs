using System.ComponentModel;
using System.Windows.Forms;

namespace DirTemplateExtension
{
    public partial class FormProgress : Form
    {
        private readonly BackgroundWorker _backgroundWorker;

        public FormProgress(string projectName, BackgroundWorker backgroundWorker)
        {
            _backgroundWorker = backgroundWorker;
            InitializeComponent();
            lblStatus.Text = string.Format(Resources.FormProgress_StatusLabel, projectName);
        }

        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            _backgroundWorker.CancelAsync();
        }
    }
}