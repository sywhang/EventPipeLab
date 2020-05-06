using System;
using System.Diagnostics.Tracing;

namespace CounterBenchmarks
{

    [EventSource(Name="BM-Counter-Source")]
    public sealed class BMCounterSource: EventSource
    {
        public class Keywords
        {
            public const EventKeywords EventOne = (EventKeywords)1;
            public const EventKeywords EventTwo = (EventKeywords)2;
            public const EventKeywords EventThree = (EventKeywords)4;
            public const EventKeywords EventFour= (EventKeywords)8;
        }

 
        // define the singleton instance of the event source
        public static BMCounterSource Log = new BMCounterSource();
        public EventCounter m_eventCounter;
        public PollingCounter m_pollingCounter;
        public IncrementingEventCounter m_incrementingEventCounter;
        public IncrementingPollingCounter m_incrementingPollingCounter;

        [Event(1, Message = "MyEvent1", Level = EventLevel.Informational, Keywords = Keywords.EventOne)]
        public void WriteEventOne()
        {
            WriteEvent(1);
        }

        [Event(2, Message = "MyEvent2", Level = EventLevel.Informational, Keywords = Keywords.EventTwo)]
        public void WriteEventTwo()
        {
            WriteEvent(2, "An arbitrary event payload with fixed string length");
        }

        [Event(3, Message = "MyEvent3", Level = EventLevel.Informational, Keywords = Keywords.EventThree)]
        public void WriteEventThree(string payload)
        {
            WriteEvent(3, payload);
        }

 
        public void WriteEventCounter(int val)
        {
            m_eventCounter.WriteMetric(val);
        }

   
        public void WriteEventCounter(double val)
        {
            m_eventCounter.WriteMetric(val);
        }

        public void WriteIncrementingEventCounter(int val)
        {
            m_incrementingEventCounter.Increment(val);
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
