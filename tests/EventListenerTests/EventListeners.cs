using System;
using System.Diagnostics.Tracing;
using System.Collections.Generic;

namespace EventListenerTests
{
    public abstract class TestEventListener : EventListener
    {
        public void VerifyAndReportError(string testName, string eventName, int minCount)
        {
            if (this.Verify(eventName, minCount))
            {
                Console.WriteLine($"Could not verify {eventName} having {minCount} events recorded");
            }
            else
            {
                Console.WriteLine($"Test passed: {testName}");
            }
        }
        public abstract bool Verify(string eventName, int minCount);
    }

    // SingleLoudEventListener listens to a single event provider at maximum verbosity and max keywords
    public class SingleLoudEventListener : TestEventListener
    {
        private string _providerToEnable;
        private Dictionary<string, int> _eventCount;
        public SingleLoudEventListener(string providerName)
        {
            _providerToEnable = providerName;
            _eventCount = new Dictionary<string, int>();
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name.Equals(_providerToEnable))
            {
                EnableEvents(eventSource, EventLevel.Verbose, EventKeywords.All);
            }
        }

        /// <summary>
        /// Returns whether there is at least minCount recorded event of eventName 
        /// </summary>
        /// <param name="eventName">Event name to check for</param>
        /// <param name="minCount">Minimum recorded event count</param>
        /// <returns></returns>
        public override bool Verify(string eventName, int minCount)
        {
            return _eventCount.ContainsKey(eventName) && _eventCount[eventName] >= minCount;
        }
    }
}