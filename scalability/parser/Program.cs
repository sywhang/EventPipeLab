using System;
using System.Diagnostics.Tracing;
using System.IO;
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace parser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: dotnet run [pid] [mode]");
                return;
            }

            int pid = Int32.Parse(args[0]);
            string mode = args[1];
            if (mode == "rt")
            {
                UseEPES(pid);
            }
            else if (mode == "post")
            {
                UseFS(pid);
            }
            else
            {
                Console.WriteLine("Not a valid mode. Supported modes: rt, post");
            }
        }


        /// <summary>
        /// This uses EventPipeEventSource's Stream constructor to parse the events real-time.
        /// It then returns the number of events read.
        /// </summary>
        static void UseEPES(int pid)
        {
            int eventsRead = 0;
            DiagnosticsClient client = new DiagnosticsClient(pid);
            EventPipeSession session = client.StartEventPipeSession(new EventPipeProvider("MySource", EventLevel.Verbose));

            Console.WriteLine("session open");
            EventPipeEventSource epes = new EventPipeEventSource(session.EventStream);
            epes.Dynamic.All += (TraceEvent data) => {
                eventsRead += 1;
            };
            epes.Process();
            Console.WriteLine("Used realtime.");
            Console.WriteLine("Read total: " + eventsRead.ToString());
            Console.WriteLine("Dropped total: " + epes.EventsLost.ToString());
        }

        /// <summary>
        /// This uses CopyToAsync to copy the trace into a filesystem first, and then uses EventPipeEventSource
        /// on the file to post-process it and return the total # of events read.
        /// </summary>
        static void UseFS(int pid)
        {
            int eventsRead = 0;
            const string fileName = "./temp.nettrace";
            DiagnosticsClient client = new DiagnosticsClient(pid);
            EventPipeSession session = client.StartEventPipeSession(new EventPipeProvider("MySource", EventLevel.Verbose));

            Console.WriteLine("session open");

            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                Task copyTask = session.EventStream.CopyToAsync(fs);
                while(!copyTask.Wait(100));
            }
            EventPipeEventSource epes = new EventPipeEventSource(fileName);
            epes.Dynamic.All += (TraceEvent data) => {
                eventsRead += 1;
            };
            epes.Process();
            Console.WriteLine("Used post processing.");
            Console.WriteLine("Read total: " + eventsRead.ToString());
            Console.WriteLine("Dropped total: " + epes.EventsLost.ToString());
        }
    }
}
