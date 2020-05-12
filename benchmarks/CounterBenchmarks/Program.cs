using System;
using System.Diagnostics.Tracing;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;


namespace CounterBenchmarks
{
    public class Benchmark
    {
        private const int N = 10000;
        private readonly byte[] data;

        private readonly SimpleCounterListener listener;
        private readonly MyDiagnosticsClient client;

        private EventCounter eventCounter;

        public Benchmark()
        {
            listener = new SimpleCounterListener();
            client = new MyDiagnosticsClient();
        }

        [Benchmark]
        public void WriteEventCounterWithoutListener()
        {
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventCounter(i);
            }
        }

        [Benchmark]
        public void WriteEventCounterWithListener()
        {
            listener.StartListeningToCounters(BMCounterSource.Log);
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventCounter(i);
            }
            listener.StopListening(BMCounterSource.Log);
        }

        [Benchmark]
        public void WriteEventCounterWithClient()
        {
            client.StartListeningToCounters("BM-Counter-Source");
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventCounter(i);
            }
            client.Stop();
        }


        [Benchmark]
        public void WritePollingCounterWithoutListener()
        {
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventCounter(i);
            }
        }

        [Benchmark]
        public void WritePollingCounterWithListener()
        {
            listener.StartListeningToCounters(BMCounterSource.Log);
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventCounter(i);
            }
            listener.StopListening(BMCounterSource.Log);
        }

        [Benchmark]
        public void WritePollingCounterWithClient()
        {
            client.StartListeningToCounters("BM-Counter-Source");
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventCounter(i);
            }
            client.Stop();
        }

        [Benchmark]
        public void WriteIncrementingEventCounterWithoutListener()
        {
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteIncrementingEventCounter(i);
            }
        }

        [Benchmark]
        public void WriteIncrementingEventCounterWithListener()
        {
            listener.StartListeningToCounters(BMCounterSource.Log);
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteIncrementingEventCounter(i);
            }
            listener.StopListening(BMCounterSource.Log);
        }

        [Benchmark]
        public void WriteSimpleEventWithListener()
        {
            listener.StartListening(BMCounterSource.Log, 1);
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventOne();
            }
            listener.StopListening(BMCounterSource.Log);
        }

        [Benchmark]
        public void WriteFixedStringEventWithListener()
        {
            listener.StartListening(BMCounterSource.Log, 2);
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventTwo();
            }
            listener.StopListening(BMCounterSource.Log);
        }

        [Benchmark]
        public void Write100StringEventWithListener()
        {
            listener.StartListening(BMCounterSource.Log, 3);
            string bigStr = new string('a', 100);
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventThree(bigStr);
            }
            listener.StopListening(BMCounterSource.Log);
        }

        [Benchmark]
        public void Write1000StringEventWithListener()
        {
            listener.StartListening(BMCounterSource.Log, 3);
            string bigStr = new string('a', 10000);
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventThree(bigStr);
            }
            listener.StopListening(BMCounterSource.Log);
        }

        [Benchmark]
        public void Write100000StringEventWithListener()
        {
            listener.StartListening(BMCounterSource.Log, 3);
            string bigStr = new string('a', 100000);
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventThree(bigStr);
            }
            listener.StopListening(BMCounterSource.Log);
        }


        [Benchmark]
        public void Write300000StringEventWithListener()
        {
            listener.StartListening(BMCounterSource.Log, 3);
            string bigStr = new string('a', 300000);
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventThree(bigStr);
            }
            listener.StopListening(BMCounterSource.Log);
        }

        [Benchmark]
        public void Write1000StringWithListenerWith2Threads()
        {
            listener.StartListening(BMCounterSource.Log, 3);
            string bigStr = new string('a', 1000);
            Thread[] t = new Thread[2];
            for (int i = 0; i < 2; i++)
            {
                t[i] = new Thread(() => { BMCounterSource.Log.WriteEventThree(bigStr); });
                t[i].Start();
            }
            listener.StopListening(BMCounterSource.Log);
        }


        [Benchmark]
        public void Write1000StringWithListenerWith5Threads()
        {
            listener.StartListening(BMCounterSource.Log, 3);
            string bigStr = new string('a', 1000);
            Thread[] t = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                t[i] = new Thread(() => { BMCounterSource.Log.WriteEventThree(bigStr); });
                t[i].Start();
            }
            listener.StopListening(BMCounterSource.Log);
        }

        [Benchmark]
        public void Write1000StringWithListenerWith10Threads()
        {
            listener.StartListening(BMCounterSource.Log, 3);
            string bigStr = new string('a', 1000);
            Thread[] t = new Thread[10];
            for (int i = 0; i < 10; i++)
            {
                t[i] = new Thread(() => { BMCounterSource.Log.WriteEventThree(bigStr); });
                t[i].Start();
            }
            listener.StopListening(BMCounterSource.Log);
        }

        [Benchmark]
        public void WriteString1000WithListenerWith50Threads()
        {
            listener.StartListening(BMCounterSource.Log, 3);
            string bigStr = new string('a', 1000);
            Thread[] t = new Thread[50];
            for (int i = 0; i < 50; i++)
            {
                t[i] = new Thread(() => { BMCounterSource.Log.WriteEventThree(bigStr); });
                t[i].Start();
            }
            listener.StopListening(BMCounterSource.Log);
        }

        [Benchmark]
        public void WriteString1000WithClientWith5Threads()
        {
            client.Start("BM-Counter-Source", EventLevel.Informational, (long)3);
            string bigStr = new string('a', 1000);
            Thread[] t = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                t[i] = new Thread(() => { BMCounterSource.Log.WriteEventThree(bigStr); });
                t[i].Start();
            }
            client.Stop();
        }

        [Benchmark]
        public void WriteString1000WithClientWith10Threads()
        {
            client.Start("BM-Counter-Source", EventLevel.Informational, (long)3);
            string bigStr = new string('a', 1000);
            Thread[] t = new Thread[10];
            for (int i = 0; i < 10; i++)
            {
                t[i] = new Thread(() => { BMCounterSource.Log.WriteEventThree(bigStr); });
                t[i].Start();
            }
            client.Stop();
        }

        [Benchmark]
        public void WriteString1000WithClientWith50Threads()
        {
            client.Start("BM-Counter-Source", EventLevel.Informational, (long)3);
            string bigStr = new string('a', 1000);
            Thread[] t = new Thread[50];
            for (int i = 0; i < 50; i++)
            {
                t[i] = new Thread(() => { BMCounterSource.Log.WriteEventThree(bigStr); });
                t[i].Start();
            }
            client.Stop();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmark>();
        }
    }
}
