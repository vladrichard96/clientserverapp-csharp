using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ServerApp
{
    class Program
    {
        private static int ReceiveNumber (Socket sock)//прием чисел
        {
            byte[] buffer = new byte[4];
            sock.Receive(buffer);
            return BitConverter.ToInt32(buffer);
        }
        private static void SendDouble(Socket sock, double num)//отправка новых чисел
        {
            byte[] buffer = BitConverter.GetBytes(num);
            sock.Send(buffer);
        }
        public static int[] SummaStrok(int str, int stb, int[,] value)//сумма строк
        {
            int[] sumstrok = new int[str];
            for (int m = 0; m < str; m++)
                for (int n = 0; n < stb; n++)
                {
                    sumstrok[m] += value[m, n];
                }
            return sumstrok;
        }
        public static int[] SummaStolb(int str, int stb, int[,] value)//сумма столбцов
        {
            int[] sumstolb = new int[stb];
            for (int m = 0; m < str; m++)
                for (int n = 0; n < stb; n++)
                {
                    sumstolb[n] += value[m, n];
                }
            return sumstolb;
        }
        public static double[] SrKvStrok(int str, int stb, int[,] value)//среднее квадратичное строк
        {
            double[] srkvstrok = new double[str];
            int[] sumstrok = new int[str];
            int[] srstrok = new int[str];
            double[] delta = new double[str];
            for (int m = 0; m < str; m++)
                for (int n = 0; n < stb; n++)
                {
                    sumstrok[m] += value[m, n];
                }
            for (int m = 0; m < str; m++)
                srstrok[m] = sumstrok[m] / stb;
            for (int m = 0; m < str; m++)
                for (int n = 0; n < stb; n++)
                {
                    delta[m] += Math.Pow((value[m, n] - srstrok[m]), 2);
                }
            for (int m = 0; m < str; m++)
                srkvstrok[m] = Math.Sqrt(delta[m] / stb);
            return srkvstrok;
        }
        public static double[] SrKvStolb(int str, int stb, int[,] value)//среднее квадратичное столбцов
        {
            double[] srkvstolb = new double[stb];
            int[] sumstolb = new int[stb];
            int[] srstolb = new int[stb];
            double[] delta = new double[stb];
            for (int m = 0; m < str; m++)
                for (int n = 0; n < stb; n++)
                {
                    sumstolb[n] += value[m, n];
                }
            for (int n = 0; n < stb; n++)
                srstolb[n] = sumstolb[n] / str;
            for (int m = 0; m < str; m++)
                for (int n = 0; n < stb; n++)
                {
                    delta[n] += Math.Pow((value[m, n] - srstolb[n]), 2);
                }
            for (int n = 0; n < stb; n++)
                srkvstolb[n] = Math.Sqrt(delta[n] / str);
            return srkvstolb;
        }
        public static double Determinant(int str, int stb, int[,] value)//расчет определителя методом гаусса
        {
            if (str!=stb) return 0;
            else
            {
                int n = str;
                if (n == 1) return value[0, 0];
                int shifts;
                SortMatrix(value, n, out shifts);
                for (int i = 1; i < n; ++i) // итерация методом Гаусса;
                {
                    int index = value[i, 0] / value[0, 0];
                    for (int j = 0; j < n; ++j)
                        value[i, j] -= index * value[0, j];
                }
                return value[0, 0] * Math.Pow(-1, shifts) * Determinant(n-1, n-1, generateSubMatrix(value, n));
            }
            static void swapArray(int[,] matrix, int index, int lengthOfMatrix)
            {
                for (int i = 0; i < lengthOfMatrix; ++i)
                swapDouble(ref matrix[index, i], ref matrix[index + 1, i]);
            }
            static void swapDouble(ref int Buffer1, ref int Buffer2)
            {
                int Tmp = Buffer1;
                Buffer1 = Buffer2;
                Buffer2 = Tmp;
            }
            static void SortMatrix(int[,] matrix, int n, out int shifts)
            {
                shifts = new int();
                bool flagOfSwapping = true;
                int numberOfIteration = new int();
                while (flagOfSwapping) // цикл выполняется пока есть хотя бы одна перестановка в ходе итерации;
                {
                    flagOfSwapping = false;
                    for (int i = 0; i < n - 1 - numberOfIteration; ++i)
                        if (matrix[i, 0] < matrix[i + 1, 0])
                        {
                            swapArray(matrix, i, n);
                            flagOfSwapping = true;
                            ++shifts;
                        }
                    numberOfIteration++;
                }
            }
            static int[,] generateSubMatrix(int[,] matrix, int n)
            {
                int[,] subMatrix = new int[n - 1, n - 1];
                for (int i = 1; i < n; ++i) // выделяется подматрица со строк [1, n);
                    for (int j = 1; j < n; ++j) // .. cтолбцов [1; n);
                        subMatrix[i - 1, j - 1] = matrix[i, j];
                return subMatrix;
            }
        }
        static void Main(string[] args)
        {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//открытие сокета
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            EndPoint ep = new IPEndPoint(ip, 8080);//связывание с конечной точкой
            server.Bind(ep);//прослушка
            server.Listen(1);//1 клиент в очереди
            int counter = 0;
            while (true) { 
            Socket client = server.Accept();//прием сокета сервера
            int i = ReceiveNumber(client);//прием индексов массива
            int j = ReceiveNumber(client);
            int[,] a = new int[j, i];//инициализация
            double[] b = new double[6];//создание нового массива для будущих данных
            double[] strok = new double[j];//массив для сумм строк, столбцов и квадратичных
            double[] stolb = new double[i];
            double[] srkvstrok = new double[j];
            double[] srkvstolb = new double[i];
            double avstrok = 0;//среднее значение строк, столбцов и квадратичных
            double avstolb = 0;
            double avsrkvstr = 0; 
            double avsrkvstb = 0;
            for (int m = 0; m < j; m++)
            for (int n = 0; n < i; n++)
                {
                    a[m, n] = ReceiveNumber(client);//прием и заполнение основного массива из файла
                    strok[m] = 0;
                    stolb[n] = 0;
                    srkvstrok[m] = 0;
                    srkvstolb[n] = 0;
                }
            
            for (int m = 0; m < j; m++)//расчет необходимых значений из матрицы
            for (int n = 0; n < i; n++)
                {
                    strok[m] = SummaStrok(j, i, a)[m] / i;
                    stolb[n] = SummaStolb(j, i, a)[n] / j;
                    srkvstrok[m] = SrKvStrok(j, i, a)[m];
                    srkvstolb[n] = SrKvStolb(j, i, a)[n];
                }
            double det = Determinant(j, i, a);
            for (int m = 0; m < j; m++) {//расчет средних значений для удобства
            avstrok += strok[m];
            avsrkvstr += srkvstrok[m];
            }
            for (int n = 0; n < i; n++) {
            avstolb += stolb[n];
            avsrkvstb += srkvstolb[n];
            }
            avstrok = avstrok / j;
            avsrkvstr = avsrkvstr / j;
            avstolb = avstolb / i;
            avsrkvstb = avsrkvstb / i;
            counter++;//счетчик для идентификатора загруженного файла
            b[0] = counter;//перенос значений в необходимый нам массив b
            b[1] = avstrok; 
            b[2] = avsrkvstr;
            b[3] = avstolb;
            b[4] = avsrkvstb;
            b[5] = det;
            for (int n=0; n<=5; n++)
                {
                    Console.WriteLine(b[n]);//для отчета записываем значение нового массива в сервер
                    SendDouble(client, b[n]);//и отправляем его в клиент
                }
            client.Shutdown(SocketShutdown.Both);//закрытие сокета после приема значений
            client.Close();
            }
        }
    }
}
