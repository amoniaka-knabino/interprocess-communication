using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SleepingHaircutter
{
    class Program
    {
        public static int clientsInTheHall = 0; // helping int
        public static bool mutex = true; // false - down ; true - up // Can I use this resource?
        public static int[] chairs;
        public static int chairsCount;
        public static int clientsQ;
        public static Random rnd = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("Slepping haircutter simulation. Classical problem of interproccess-communication");
            Console.WriteLine("How many chairs in the hall?");
            chairsCount = int.Parse(Console.ReadLine());
            chairs = new int[chairsCount+1]; // chair[0] is HC's chair
            for (int i = 0; i < chairsCount+1; i++)
            {
                chairs[i] = -1;
            }
            Console.WriteLine("If you want finite quantity of clients, write the quantity. If you don't - write -1");
            clientsQ = int.Parse(Console.ReadLine());


            Thread HC = new Thread(Work)
            {
                Name = "Haircutter"
            };
            HC.Start();

            if (clientsQ == -1)
            {
                while (true)
                {
                    for (int i = 0; i < 100; i++) // index for naming clients
                    {
                        Thread C = new Thread(NeedHaircut)
                        {
                            Name = i.ToString()
                        };
                        C.Start();
                        Thread.Sleep(rnd.Next(50, 100));// clients shouldn't come at once
                    }
                }
            }
            else
            {
                for (int i = 0; i < clientsQ; i++) // index for naming clients
                {
                    Thread C = new Thread(NeedHaircut) // C = client
                    {
                        Name = i.ToString()
                    };
                    C.Start();
                }
            }
        }

        public static void Work()
        {
            while (true)
            {
                while (CanISleep())
                {
                        Console.Write("\rHaircutter is sleeping");
                }
                Console.WriteLine("Haircutter got up");
                Console.WriteLine("Haircutter is working with client #{0} now", chairs[0]);
                Thread.Sleep(rnd.Next(100, 1000));
                while (!(mutex)) ; //wait for mutex
                mutex = false;
                chairs[0] = -1;
                MoveQueque();
                clientsInTheHall--;
                mutex = true;
                Console.WriteLine("Haircutter finished");
            }
        }

        public static void NeedHaircut()
        {
            while (!(mutex)) ; //wait for mutex
            mutex = false;
            Console.WriteLine("\nClient {0} come to hall ", Thread.CurrentThread.Name);
            if (clientsInTheHall == chairsCount)
            {
                Console.WriteLine("Client {0} went away", Thread.CurrentThread.Name);
            }
            else
            {
                for (int i = 0; i < chairsCount; i++)
                {
                    if (chairs[i] == -1)
                    {
                        chairs[i] = int.Parse(Thread.CurrentThread.Name);
                        Console.WriteLine("Client {0} is waiting on the place #{1}", Thread.CurrentThread.Name, i);
                        break;
                    }
                }
                clientsInTheHall++;
            }
            mutex = true;
        }

        public static bool CanISleep()
        {
            while (!(mutex)) ;
            mutex = false;
            bool a = clientsInTheHall == 0; // Is someone waiting?
            mutex = true;
            return a;
        }

        public static void MoveQueque()
        {
            for (int i = 0; i < chairsCount; i++)
            {
                chairs[i] = chairs[i + 1];
            }
        }
    }
}
