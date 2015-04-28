using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class ChangeOptionsForm : Form
    {
        public ChangeOptionsForm()
        {
            InitializeComponent();
        }

        private void ChangeOptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Options.interval = (int)numericUpDown1.Value * 1000;
        }

        private void ChangeOptionsForm_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = Options.interval / 1000;
        }
    }
}
