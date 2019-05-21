using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spreader;

namespace Spreader_Standalone_Client
{
    public partial class SpreaderMainForm : Form
    {
        public SpreaderMainForm()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrayIcon.Visible = false;
            Application.Exit();
        }

        private void SpreaderMainForm_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            Hide();
        }
    }
}
