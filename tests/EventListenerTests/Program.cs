using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;

namespace EventListenerTests
{
    class Program
    {
        static void Main(string[] args)
        {
            // Need to execute this sequentially because EventListeners can override each other.
            Test_Listener_RuntimeEvents_SimpleGC();
            Test_Listener_RuntimeEvents_ManyListener();
        }

        static void Test_Listener_RuntimeEvents_SimpleGC()
        {
            using (SingleLoudEventListener listener = new SingleLoudEventListener("Microsoft-Windows-DotNETRuntime"))
            {
                // Invoke 3 GCs
                GC.Collect();
                GC.Collect();
                GC.Collect();

                listener.VerifyAndReportError("Test_Listener_RuntimeEvents_SimpleGC_Listener1", "GCStart_V1", 3);
            }
        }

        static void Test_Listener_RuntimeEvents_ManyListener()
        {
            SingleLoudEventListener listener1 = new SingleLoudEventListener("Microsoft-Windows-DotNETRuntime");
            SingleLoudEventListener listener2 = new SingleLoudEventListener("Microsoft-Windows-DotNETRuntime");
            // Invoke 3 GCs
            GC.Collect();
            GC.Collect();
            GC.Collect();

            listener1.VerifyAndReportError("Test_Listener_RuntimeEvents_ManyListener_Listener1", "GCStart_V1", 3);
            listener2.VerifyAndReportError("Test_Listener_RuntimeEvents_ManyListener_Listener2", "GCStart_V1", 3);
        }
    }
}
