using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ClientApp
{
    public partial class Form2 : Form
    {
        public class GraphData //публичный класс для графика
        {
            public static int x;
            public static int elements;
            public static double [,] graphdata = new double[elements, 5];
        }

        public double EvklidDistance (List<double> a, List<double> b) //евклидово расстояние
        {
            double e = 0;
            for (int i = 1; i <= 5; i++)
            e += Math.Pow((b[i] - a[i]), 2);
            return Math.Sqrt(e);
        }
        public double GetMax (int x, double[,] arr) //максимум для диаграммы
        {
            double max = 0;
            for (int i=0; i<5; i++)
            {
                if (arr[x,i]>max)
                {
                    max = arr[x,i];
                }
            }
            return max;
        }
        public double GetMin(int x, double[,] arr) //минимум для диаграммы
        {
            double min = arr[x,0];
            for (int i = 0; i < 5; i++)
            {
                if (arr[x, i] < min)
                {
                    min = arr[x, i];
                }
            }
            return min;
        }
        public Form2()
        {
            InitializeComponent();
            List<double> data_ = Form1.DataGet.data;//получаем массив из формы1
            GraphData.elements = data_.Count() / 6;//считаем кол-во его элементов
            var subdata = new List<double>[GraphData.elements];//формируем список субмассива для расчетов
            GraphData.graphdata = new double[GraphData.elements, 5];//и для диаграммы
            GraphData.x = comboBox1.SelectedIndex;//х нужен для индекса
            if (GraphData.x == -1) GraphData.x = 0;
            for (int i=0; i< GraphData.elements; i++)
            {
                subdata[i] = new List<double>();
                subdata[i].AddRange(data_.Skip(6*i).Take(6).ToArray());
                for (int j = 0; j < 5; j++)
                GraphData.graphdata[i, j] = subdata[i][j+1];
            }
            for (int i = 0; i < data_.Count / 6; i++)
            comboBox1.Items.Add(data_[6 * i]);//добавляем индексы в комбобокс
            this.chart1.Series["Данные"].Points.AddXY("Ср. строк", GraphData.graphdata[GraphData.x, 0]);//строим диаграмму
            this.chart1.Series["Данные"].Points.AddXY("Ср. кв. строк", GraphData.graphdata[GraphData.x, 1]);
            this.chart1.Series["Данные"].Points.AddXY("Ср. столбов", GraphData.graphdata[GraphData.x, 2]);
            this.chart1.Series["Данные"].Points.AddXY("Ср. кв. столбов", GraphData.graphdata[GraphData.x, 3]);
            this.chart1.Series["Данные"].Points.AddXY("Определитель", GraphData.graphdata[GraphData.x, 4]);
        }

        public void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphData.x = comboBox1.SelectedIndex;//обновляя индекс выбираем новый субмассив
            chart1.Series.Clear();//обновляем и строим график
            chart1.Series.Add("Данные");
            this.chart1.Series["Данные"].Points.AddXY("Ср. строк", GraphData.graphdata[GraphData.x, 0]);
            this.chart1.Series["Данные"].Points.AddXY("Ср. кв. строк", GraphData.graphdata[GraphData.x, 1]);
            this.chart1.Series["Данные"].Points.AddXY("Ср. столбов", GraphData.graphdata[GraphData.x, 2]);
            this.chart1.Series["Данные"].Points.AddXY("Ср. кв. столбов", GraphData.graphdata[GraphData.x, 3]);
            this.chart1.Series["Данные"].Points.AddXY("Определитель", GraphData.graphdata[GraphData.x, 4]);
            chart1.ChartAreas[0].AxisY.Maximum = GetMax(GraphData.x, GraphData.graphdata);//обновляем минимумы и максимумы
            chart1.ChartAreas[0].AxisY.Minimum = GetMin(GraphData.x, GraphData.graphdata);//чтобы наблюдать новые результаты в диаграмме
        }

    }
}
