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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int width;
            try
            {
                width = Convert.ToInt32(textBox1.Text);
                if (width < 2 || width > 50)
                    throw new Exception();
                Form1 frm = new Form1(width, width);
                this.Hide();
                frm.ShowDialog();
                this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Неверные данные в полях. Должны быть целые числа от 2 до 50");
            }
        }
    }
}
