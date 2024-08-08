using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace go_test_second
{
    public partial class Form2 : Form
    {
        Form1 frm;
        public Form2(Form1 frm1)
        {
            frm = frm1;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frm.ChangeComment(textBox1.Text);
            this.Close();
        }
    }
}
