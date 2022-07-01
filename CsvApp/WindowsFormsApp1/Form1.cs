using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            var baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appStorageFolder = Path.Combine(baseFolder, "csvedit");
            var fullPath = Path.Combine(appStorageFolder, @"data.csv");
            var i = Convert.ToInt32(textBox2.Text);
            var j = Convert.ToInt32(textBox1.Text);
            if (i > 0 & j > 0)
            {
                using (StreamWriter fi1 = new StreamWriter(File.Create(fullPath)))
                {
                    fi1.WriteLine(Convert.ToString(j) + "," + Convert.ToString(i));
                    int[,] a = new int[i, j];
                    for (int m = 0; m <= i - 1; m++)
                    {
                        for (int n = 0; n <= j - 1; n++)
                        {
                            a[m, n] = rnd.Next(1, 100);
                            if (n==j-1) fi1.Write(Convert.ToString(a[m, n]));
                            else fi1.Write(Convert.ToString(a[m, n]) + ",");
                        }
                        fi1.Write("\n");
                    }
                    MessageBox.Show("Файл успешно заполнен!");
                }
            }
            else MessageBox.Show("Задайте натуральные значения!");
        }
    }
}
