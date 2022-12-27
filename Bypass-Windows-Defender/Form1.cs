using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bypass_Windows_Defender
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) // Button to initalize build action
        {
            using(SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Executable(*.exe) | *.exe";
                if(saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    new Builder(Properties.Resources.bypass, saveFileDialog.FileName);
                }
            }
        }
    }
}
