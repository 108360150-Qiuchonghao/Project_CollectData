using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseUI
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }
        public delegate void SendEarTemp(double messages);
        public event SendEarTemp SendEvent;

        public  double eartemp;
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "")
            {
                MessageBox.Show("請輸入溫度", "error");
            }
            else
            {
                eartemp = double.Parse(this.textBox1.Text);
                //SendEvent(temp);
                Form3.temp = eartemp;
                this.Close();
            }
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
