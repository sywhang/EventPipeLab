using System;
using System.Diagnostics.Tracing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace CounterBenchmarks
{
    [NativeMemoryProfiler]
    [MemoryDiagnoser]
    public class Benchmark
    {
        private const int N = 10000;
        private readonly byte[] data;

        private readonly SimpleCounterListener listener;

        private EventCounter eventCounter;

        public Benchmark()
        {
            listener = new SimpleCounterListener();
        }

        [Benchmark]
        public void WriteEventCounterWithListener()
        {
            listener.StartListening(BMCounterSource.Log);
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventCounter(i);
            }
            listener.StopListening(BMCounterSource.Log);
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
            listener.StartListening(BMCounterSource.Log);
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteEventCounter(i);
            }
            listener.StopListening(BMCounterSource.Log);
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
            listener.StartListening(BMCounterSource.Log);
            for (int i = 0; i < N; i++)
            {
                BMCounterSource.Log.WriteIncrementingEventCounter(i);
            }
            listener.StopListening(BMCounterSource.Log);
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
