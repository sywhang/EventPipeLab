using System;
using System.Diagnostics.Tracing;
using System.Threading;

namespace corescaletest
{
    class MySource : EventSource
    {
        public static MySource Log = new MySource();
        public static string s_SmallPayload = new String('a', 100);
        public static string s_BigPayload = new String('a', 10000);

        public void FireSmallEvent() { WriteEvent(1, s_SmallPayload); }
        public void FireBigEvent() { WriteEvent(1, s_BigPayload); }
    }
    class Program
    {
        private static bool finished = false;
        static void ThreadProc()
        {
            while(true)
            {
                if (finished)
                {
                    break;
                }
                MySource.Log.FireSmallEvent();
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: dotnet run [number of threads]");
            }
            int numThreads = Int32.Parse(args[0]);

            Thread[] threads = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                threads[i] = new Thread(ThreadProc);
            }
            Console.WriteLine("Say hi");
            Console.ReadLine();

            for (int i = 0; i < numThreads; i++)
            {
                threads[i].Start();
            }
            
            Console.WriteLine("Sleeping for 1 minutes");
            Thread.Sleep(1 * 60 * 1000);
            finished = true;
            
            Console.WriteLine("Done. Goodbye!");
        }
    }
}
