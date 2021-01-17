using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace EventListenerTests
{
    [EventSource(Name ="CustomEventSourceSimple")]
    class CustomEventSourceSimple : EventSource
    {
        public static CustomEventSourceSimple Log = new CustomEventSourceSimple();

        [Event(1)]
        public void Event1(string someArg)
        {
            WriteEvent(1, someArg);
        }


        [Event(2)]
        public void Event2(string someArg1, string someArg2)
        {
            WriteEvent(2, someArg1, someArg2);
        }
    }
}
