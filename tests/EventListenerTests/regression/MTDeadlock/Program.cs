using System;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;

namespace MTDeadlock
{
    class SimpleEventListener : EventListener
    {
        public SimpleEventListener()
        {        
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name.Equals("Microsoft-Windows-DotNETRuntime"))
            {
		 Console.WriteLine($"{eventSource.Name} enabled");
                EnableEvents(eventSource, EventLevel.Informational, (EventKeywords)0x10004);
            }
            else if (eventSource.Name.Equals("System.Threading.Tasks.TplEventSource")) {
                Console.WriteLine($"{eventSource.Name} enabled");
                EnableEvents(eventSource, EventLevel.Verbose, (EventKeywords)0x80);
            } else {
            Console.WriteLine($"{eventSource.Name} not enabled"); }
        }

        protected override void OnEventWritten(EventWrittenEventArgs args)
        {
            Console.WriteLine($"{args.EventSource.Name}/{args.EventName}");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            using (var listener = new SimpleEventListener())
            {
                while (true)
                {
                    Task[] printTasks = new Task[10];
                    for (var i = 0; i < 10; i++) {
                        printTasks[i] = Task.Run(() => { while(true) { Task.Delay(1000);  } });
                    }
                }
            }
        }
    }
}
