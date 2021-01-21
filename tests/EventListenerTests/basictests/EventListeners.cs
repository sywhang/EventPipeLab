using System;
using System.Diagnostics.Tracing;
using System.Collections.Generic;

namespace EventListenerTests
{
    public abstract class TestEventListener : EventListener
    {
        public void VerifyMinAndReportError(string testName, string eventName, int minCount)
        {
            Console.WriteLine("Verifying min!");
            if (!VerifyMin(eventName, minCount))
            {
                Console.WriteLine($"Could not verify {eventName} having at least {minCount} events recorded");
            }
            else
            {
                Console.WriteLine($"Test passed: {testName}");
            }
        }
        public void VerifyMaxAndReportError(string testName, string eventName, int maxCount)
        {
            if (!this.VerifyMax(eventName, maxCount))
            {
                Console.WriteLine($"Could not verify {eventName} having at most {maxCount} events recorded");
            }
            else
            {
                Console.WriteLine($"Test passed: {testName}");
            }
        }
        public void VerifyLessThanAndReportError(string testName, string eventName, int maxCount)
        {
            if (!this.VerifyLessThan(eventName, maxCount))
            {
                Console.WriteLine($"Could not verify {eventName} having less than {maxCount} events recorded");
            }
            else
            {
                Console.WriteLine($"Test passed: {testName}");
            }
        }
        public abstract bool VerifyMin(string eventName, int minCount);
        public abstract bool VerifyMax(string eventName, int minCount);
        public abstract bool VerifyLessThan(string eventName, int minCount);

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
                Console.WriteLine("Enabling " + eventSource.Name);
                EnableEvents(eventSource, EventLevel.Verbose, EventKeywords.All);
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs args)
        {
            if (_eventCount == null)
                return;
            if (_eventCount.ContainsKey(args.EventName))
                _eventCount[args.EventName] += 1;
            else
                _eventCount[args.EventName] = 1;
        }

        public void PrintAllKeys()
        {
            foreach (var key in _eventCount.Keys)
            {
                Console.WriteLine(key);
            }
        }

        /// <summary>
        /// Returns whether there is at least minCount recorded event of eventName 
        /// </summary>
        /// <param name="eventName">Event name to check for</param>
        /// <param name="minCount">Minimum recorded event count</param>
        /// <returns></returns>
        public override bool VerifyMin(string eventName, int minCount)
        {
            if (_eventCount.ContainsKey(eventName))
            {
                Console.WriteLine(_eventCount[eventName]);
            }
            return _eventCount.ContainsKey(eventName) && _eventCount[eventName] >= minCount;
        }

        /// <summary>
        /// Returns whether there is at most minCount recorded event of eventName 
        /// </summary>
        /// <param name="eventName">Event name to check for</param>
        /// <param name="maxCount">Maximum recorded event count</param>
        /// <returns></returns>
        public override bool VerifyMax(string eventName, int maxCount)
        {
            return _eventCount.ContainsKey(eventName) && _eventCount[eventName] <= maxCount;
        }


        /// <summary>
        /// Returns whether there is less than maxCount recorded event of eventName 
        /// </summary>
        /// <param name="eventName">Event name to check for</param>
        /// <param name="maxCount">Maximum recorded event count</param>
        /// <returns></returns>
        public override bool VerifyLessThan(string eventName, int maxCount)
        {
            return _eventCount.ContainsKey(eventName) && _eventCount[eventName] < maxCount;
        }
    }

    public class SingleLoudGCEventListener : SingleLoudEventListener
    {
        public SingleLoudGCEventListener(string providerName) : base(providerName)
        {
        }
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name.Equals("Microsoft-Windows-DotNETRuntime"))
            {
                Console.WriteLine("Enabling " + eventSource.Name);
                EnableEvents(eventSource, EventLevel.Verbose, (EventKeywords)0x1);
            }
        }
    }

    public class SingleLoudThreadEventListener : SingleLoudEventListener
    {
        public SingleLoudThreadEventListener(string providerName) : base(providerName)
        {
        }
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name.Equals("Microsoft-Windows-DotNETRuntime"))
            {
                Console.WriteLine("Enabling " + eventSource.Name);
                EnableEvents(eventSource, EventLevel.Verbose, (EventKeywords)0x10000);
            }
        }
    }
}