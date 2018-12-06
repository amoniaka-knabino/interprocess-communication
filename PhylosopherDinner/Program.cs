using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhylosopherDinner
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Введите число философов");
            int n = int.Parse(Console.ReadLine());
            Phylo.ChangeN(n);
            var PhyloList = new int[n]; // генерация случайной последовательности философов
            for (int i = 0; i<n;i++)
            {
                PhyloList[i] = -1;
            }
            Random rnd = new Random();
            for (int i = 0; i < n; i++)
            {
                while (true)
                {
                    var j = rnd.Next(0, n);
                    if (!PhyloList.Contains(j))
                    {
                        PhyloList[i] = j;
                        break;
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                Phylo phylo = new Phylo(PhyloList[i]);
            }
        }
    }

    class Phylo
    {
        // 0 - think 1 - hungry 2 - eat
        public static Random rnd = new Random();
        public static int N=1000; 

        public static void ChangeN(int n)
        {
            N = n; // how many phylosophersN = n;
        }

        public static int[] state = new int[N];
        public static Semaphore[] s = new Semaphore[N];
        static Mutex mutex = new Mutex();
        Thread CurThread;

        public Phylo(int i)
        {
            s[i] = new Semaphore(0, 1);
            CurThread = new Thread(Live);
            CurThread.Name = i.ToString();
            CurThread.Start();
        }

        public static int Left(int i)
        {
            return (i+N-1)%N;
        }

        public static int Right(int i)
        {
            return (i + 1) % N;
        }

        public static void Live()
        {
            while (true)
            {
                var i = int.Parse(Thread.CurrentThread.Name);
                Thread.Sleep(rnd.Next(100,1000));//размышляет
                TakeForks(i, state);
                Thread.Sleep(rnd.Next(100, 1000));
                PutForks(i, state);
                Thread.Sleep(rnd.Next(100, 1000)); //философ наелся, снова размышляет
            }
        }

        public static void TakeForks(int i, int[] state)
        {
            mutex.WaitOne();
            state[i] = 1; 
            Test(i, state); //попытка взять вилки

            mutex.ReleaseMutex();
            s[i].WaitOne();//не удалось взять вилки, ожидание
        }

        public static void PutForks(int i, int[] state)
        {
            if (state[i] == 2)
                Console.WriteLine(i + " сыт");
            mutex.WaitOne();
            state[i] = 0;
            Test(Left(i), state);
            Test(Right(i), state);
            mutex.ReleaseMutex(); 
        }
        public static void Test(int i, int[] state)
        {
            if (state[i] == 1 && state[Left(i)] != 2 && state[Right(i)] != 2)
            {
                state[i] = 2;
                s[i].Release(1);
                Console.WriteLine("Ест " + i);
            }
            else
                Console.WriteLine("Ждет " + i);
        }
    }
}
