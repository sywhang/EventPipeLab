using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace CounterBenchmarks
{
    public class SimpleCounterListener : EventListener
    {        
        private readonly EventLevel _level = EventLevel.Verbose;

        public int EventCount { get; private set; } = 0;
        public EventSource runtimeCounterSource;

        public SimpleCounterListener()
        {
        }

        protected override void OnEventSourceCreated(EventSource source)
        {
            if (source.Name.Equals("System.Runtime"))
            {
                runtimeCounterSource = source;
            }
        }

        public void StartListening(EventSource source, int keyword)
        {
            EnableEvents(source, EventLevel.Informational, (EventKeywords)keyword);
        }

        public void StartListeningToCounters(EventSource source)
        {
            EnableEvents(source, EventLevel.Informational, (EventKeywords)1, new Dictionary<string, string>(){ {"EventCounterInterval", "1" } });
        }

        public void StopListening(EventSource source)
        {
            DisableEvents(source);
        }

        public void StartListeningRuntimeCounters()
        {
            EnableEvents(runtimeCounterSource, EventLevel.Informational, (EventKeywords)1, new Dictionary<string, string>(){{ "EventCounterInterval", "1" }});
        }

        public void StopListeningRuntimeCounters()
        {
            DisableEvents(runtimeCounterSource);
        }


        private (String Name, String Value) getRelevantMetric(IDictionary<string, object> eventPayload)
        {
            String counterName = "";
            String counterMean = "";
            String counterIncrement = "";
            bool isIncrement = false;

            foreach ( KeyValuePair<string, object> payload in eventPayload )
            {
                String key = payload.Key;
                String val = payload.Value.ToString();

                if (key.Equals("Name"))
                {
                    counterName = val;
                }
                else if (key.Equals("Mean"))
                {
                    counterMean = val;
                }
                else if (key.Equals("Increment"))
                {
                    counterIncrement = val;
                    isIncrement = true;
                }
            }

            if (isIncrement)
            {
                return (counterName, counterIncrement);
            }
            else
            {
                return (counterName, counterMean);
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
        }
    }

}
