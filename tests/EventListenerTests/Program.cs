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
            Test_Listener_RuntimeEvents_WrongListener();
            Test_CustomSource_Listener();
        }

        static void Test_Listener_RuntimeEvents_SimpleGC()
        {
            using (SingleLoudEventListener listener = new SingleLoudEventListener("Microsoft-Windows-DotNETRuntime"))
            {
                // Invoke 3 GCs
                GC.Collect();
                GC.Collect();
                GC.Collect();

                listener.VerifyMinAndReportError("Test_Listener_RuntimeEvents_SimpleGC_Listener1", "GCStart_V1", 3);
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

            listener1.VerifyMinAndReportError("Test_Listener_RuntimeEvents_ManyListener_Listener1", "GCStart_V1", 3);
            listener2.VerifyMinAndReportError("Test_Listener_RuntimeEvents_ManyListener_Listener2", "GCStart_V1", 3);
        }

        /// <summary>
        /// Tests setting up 2 listeners, with 1 of them listening to nonexistent events
        /// </summary>
        static void Test_Listener_RuntimeEvents_WrongListener()
        {
            SingleLoudEventListener listener1 = new SingleLoudEventListener("Microsoft-Windows-DotNETRuntime");
            SingleLoudEventListener listener2 = new SingleLoudEventListener("SomeOtherSource");
            // Invoke 3 GCs
            GC.Collect();
            GC.Collect();
            GC.Collect();

            listener1.VerifyMinAndReportError("Test_Listener_RuntimeEvents_ManyListener_Listener1", "GCStart_V1", 3);
            listener2.VerifyLessThanAndReportError("Test_Listener_RuntimeEvents_ManyListener_Listener2", "GCStart_V1", 1);
        }

        static void Test_CustomSource_Listener()
        {
            using (SingleLoudEventListener listener = new SingleLoudEventListener("CustomEventSourceSimple"))
            {
                for (int i = 0; i < 100; i++)
                {
                    CustomEventSourceSimple.Log.Event1("arg1");
                }

                for (int i = 0; i < 100; i++)
                {
                    CustomEventSourceSimple.Log.Event2("arg1", "arg2");
                }

                listener.VerifyMinAndReportError("Test_CustomSource_Listener_evt1", "Event1", 100);
                listener.VerifyMinAndReportError("Test_CustomSource_Listener_evt2", "Event2", 100);

                listener.VerifyLessThanAndReportError("Test_CustomSource_Listener_evt1_lt", "Event1", 101);
                listener.VerifyLessThanAndReportError("Test_CustomSource_Listener_evt2_lt", "Event2", 101);
            }
        }

    }
}
