using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadersAndWriters
{
    class Program
    {
        public static Mutex mutex = new Mutex();
        public static Semaphore db = new Semaphore(1, 1); // we can change last ind
        public static int rc = 0;
        public static int wc = 0;
        public static Random rnd = new Random();


        static void Main(string[] args)
        {
            for (int i = 0; i<rnd.Next(5,10);i++)
            {
                if (rnd.Next(0, 3) != 1)
                {
                    Thread R = new Thread(Reader);
                    R.Name = i.ToString();
                    R.Start();
                }
                else
                {
                    Thread W = new Thread(Writer);
                    W.Name = i.ToString();
                    W.Start();
                }
            }
        }

        public static void Reader()
        {
            while (true)
            {
                if (wc == 0)
                {
                    mutex.WaitOne(); // = down
                    rc++;
                    Console.WriteLine(rc + " read");
                    if (rc == 1) db.WaitOne();
                    mutex.ReleaseMutex();

                    Thread.Sleep(rnd.Next(100, 1000)); // read db
                    mutex.WaitOne();
                    rc--;
                    if (rc == 0) db.Release();
                    mutex.ReleaseMutex();
                    Console.WriteLine(rc + " read");
                    Thread.Sleep(rnd.Next(100, 1000));// use_data_read
                }
            }
        }

        public static void Writer()
        {
            while (true)
            {
                Random rnd = new Random();
                Thread.Sleep(rnd.Next(1000, 10000));// think_up_data()
                wc++;
                Console.WriteLine("Writer " + Thread.CurrentThread.Name + " need db");
                db.WaitOne();
                mutex.WaitOne();
                Console.WriteLine("rc = " + rc + " wr" + Thread.CurrentThread.Name + "is writing his data");
                Thread.Sleep(rnd.Next(10, 100)); // write
                mutex.ReleaseMutex();
                db.Release();
                Console.WriteLine("Writer " + Thread.CurrentThread.Name + " wrote everything");
                wc--;
                Console.WriteLine("check wc=" + wc); // checking
            }
        }
    }    
}
