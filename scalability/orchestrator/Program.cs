using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;

#if _WINDOWS
using System.Security.Principal;
#endif // _WINDOWS

using System.Threading;
using System.Threading.Tasks;


using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;

namespace orchestrator
{
    class Program
    {
        static int NUM_CORES_MAX = 24;
        static int NUM_CORES_MIN = 1; // CHANGES THESE TO WHATEVER YOU WANT

        static int num_event_count = 0;
        static int cur_core_count;

        static Dictionary<int, int> eventCounts;

        static void Main(string[] args)
        {
            eventCounts = new Dictionary<int, int>();

#if _WINDOWS
            // Try to run in admin mode in Windows
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if (!isElevated)
            {
                Console.WriteLine("Must run in root/admin mode");
                return;
            }
#endif // _WINDOWS
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: dotnet run [path-to-corescaletest.exe]");
                return;
            }
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Not a file " + args[0]);
            }
            Measure(args[0]);
        }

        static void ThreadProc(Object arg)
        {
            Process eventWritingProc = (Process)arg;
            eventCounts[cur_core_count] = 0;
            DiagnosticsClient client = new DiagnosticsClient(eventWritingProc.Id);
            EventPipeSession session = client.StartEventPipeSession(new EventPipeProvider("MySource", EventLevel.Verbose, (long)-1, null));
            EventPipeEventSource source = new EventPipeEventSource(session.EventStream);
            source.Dynamic.All += (TraceEvent data) => {
                if (data.EventName == "FireSmallEvent")
                {
                    eventCounts[cur_core_count] += 1;
                }
            };
            source.Process();
        }

        static void Measure(string fileName)
        {
            for (int num_cores = NUM_CORES_MIN; num_cores <= NUM_CORES_MAX; num_cores++)
            {
                num_event_count = 0;
                cur_core_count = num_cores;

                Console.WriteLine("========================================================");
                Console.WriteLine("Starting run with proc count " + num_cores.ToString());

                Process eventWritingProc = new Process();
                eventWritingProc.StartInfo.FileName = fileName;
                eventWritingProc.StartInfo.Arguments = num_cores.ToString();
                eventWritingProc.StartInfo.UseShellExecute = false;
                eventWritingProc.StartInfo.RedirectStandardInput = true;
                eventWritingProc.Start();

                Console.WriteLine("Here");
                // Set affinity and priority
                long affinityMask = 0;
                for (int j = 0; j < num_cores; j++)
                {
                    affinityMask |= (1 << j);
                }
                eventWritingProc.ProcessorAffinity = (IntPtr)((long)eventWritingProc.ProcessorAffinity & affinityMask);
                eventWritingProc.PriorityClass = ProcessPriorityClass.RealTime; // Set the process priority to highest possible

                // Start listening to the event.


                CancellationTokenSource  ct = new CancellationTokenSource();
                Thread t = new Thread(ThreadProc);
                t.Start(eventWritingProc);

                // start the target process
                StreamWriter writer = eventWritingProc.StandardInput;
                writer.WriteLine("\r\n");
                eventWritingProc.WaitForExit();

                t = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                
                Console.WriteLine("Done with proc count " + num_cores.ToString());
                Console.WriteLine("========================================================");

            }
            
            Console.WriteLine("**** Summary ****");
            for (int i = NUM_CORES_MIN; i <= NUM_CORES_MAX; i++)
            {
                Console.WriteLine(eventCounts[i]);
            }
        }
    }
}
