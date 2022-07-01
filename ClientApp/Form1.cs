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
using System.Net;
using System.Net.Sockets;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label3.Text = "";
        }
        public class DataGet//публичный класс для списка массивов
        {
            public static List<double> data = new List<double>();
        }
        private void SendNumber (Socket sock, int num)//метод отправки
        {
            byte[] buffer = BitConverter.GetBytes(num);
            sock.Send(buffer);
        }
        private static double ReceiveDouble(Socket sock)//метод приема
        {
            byte[] buffer = new byte[8];
            sock.Receive(buffer);
            return BitConverter.ToDouble(buffer, 0);
        }
        private void button3_Click(object sender, EventArgs e)//при нажатии на кнопку открываем файл .сым
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "Файлы-результаты (*.csv)|*.csv";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filename = openFileDialog1.FileName;
                    textBox1.Clear();
                    textBox1.Text = filename;
                }
            }
        }

        public void button1_Click(object sender, EventArgs e)//загружаем файл
        {
            if (textBox1.Text == "")//если он пустой
            {
                MessageBox.Show("Ничего не найдено!");
            }
            else
            {
                int i, j;
                List<double> data = new List<double>();
                label3.Text = "";
                string[] lines = File.ReadAllLines(textBox1.Text);//из файла берем линии и разделители
                string[] counters = lines[0].Split(',');//первая строчка для значений
                i = Convert.ToInt32(counters[0]);
                j = Convert.ToInt32(counters[1]);
                int[,] a = new int [j,i];//формируем массив
                for (int k = 1; k < lines.Length; k++)
                {
                    string[] values = lines[k].Split(',');//получаем значения
                    for (var v = 0; v < values.Length; v++)
                    {
                        a[k - 1, v] = Convert.ToInt32(values[v]);//и заполняем массив
                    }
                }
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect("127.0.0.1", 8080);
                SendNumber(s, i);//по сокету отправляем индексы массивов
                SendNumber(s, j);
                for (int p = 0; p < j; p++)
                {
                    for (int q = 0; q < i; q++)
                    {
                        SendNumber(s, a[p,q]);//а затем и сами данные массива
                    }
                }
                label3.Text = "Файлы загружены успешно!";//файлы загружены успешно
                double[] b = new double[6];//выделяем новый промежуточный массив для данных с сервера
                for (int n = 0; n <= 5; n++)
                {
                    b[n] = ReceiveDouble(s);//заполняем его
                }
                DataGet.data.AddRange(b);//и отправляем в основной список массивов data
                s.Shutdown(SocketShutdown.Both);//закрыв сокет
                s.Close();
            }
        }

        public void button2_Click(object sender, EventArgs e)//переход на новую форму
        {
            Form2 f = new Form2();
            f.Show();
        }
    }
}
