using System;
using System.Diagnostics.Tracing;

namespace CounterBenchmarks
{

    [EventSource(Name="BM-Counter-Source")]
    public sealed class BMCounterSource: EventSource
    {
        // define the singleton instance of the event source
        public static BMCounterSource Log = new BMCounterSource();
        public EventCounter m_eventCounter;
        public PollingCounter m_pollingCounter;
        public IncrementingEventCounter m_incrementingEventCounter;
        public IncrementingPollingCounter m_incrementingPollingCounter;

        public void WriteEventCounter(int val)
        {
            m_eventCounter.WriteMetric(val);
        }

   
        public void WriteEventCounter(double val)
        {
            m_eventCounter.WriteMetric(val);
        }

        private BMCounterSource() : base(EventSourceSettings.EtwSelfDescribingEventFormat) 
        {
            // Counter names should ideally be the same length, so I'm naming them with number-suffix.
            this.m_eventCounter = new EventCounter("BenchmarkCounter1", this)
            {
                // Same thing for these fields
                DisplayName = "ADisplayName",
                DisplayUnits = "MSec"
            };

            this.m_pollingCounter = new PollingCounter("BenchmarkCounter2", this, () => 1)
            {
                DisplayName = "ADisplayName",
                DisplayUnits = "MSec"
            };

            this.m_incrementingEventCounter = new IncrementingEventCounter("BenchmarkCounter3", this)
            {
                DisplayName = "ADisplayName",
                DisplayUnits = "MSec"
            };

            this.m_incrementingPollingCounter = new IncrementingPollingCounter("BenchmarkCounter4", this, () => 1)
            {
                DisplayName = "ADisplayName",
                DisplayUnits = "MSec"
            };
        }
    }
}
