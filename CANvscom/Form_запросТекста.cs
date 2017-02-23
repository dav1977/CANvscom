using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Program
{
    public partial class Form_запросТекста : Form
    {
        public Form_запросТекста()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            data.text = richTextBox1.Text;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            data.text = null;
            this.Close();
        }
    }
}
