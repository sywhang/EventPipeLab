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
            Test_ThreadPool_Listener();
        }

        static void Test_Listener_RuntimeEvents_SimpleGC()
        {
            using (SingleLoudEventListener listener = new SingleLoudGCEventListener("Microsoft-Windows-DotNETRuntime"))
            {
                // Invoke 3 GCs
                GC.Collect();
                GC.Collect();
                GC.Collect();
                Thread.Sleep(1000);

                listener.VerifyMinAndReportError("Test_Listener_RuntimeEvents_SimpleGC_Listener1", "GCStart_V2", 3);
            }
        }

        static void Test_Listener_RuntimeEvents_ManyListener()
        {
            SingleLoudEventListener listener1 = new SingleLoudGCEventListener("Microsoft-Windows-DotNETRuntime");
            SingleLoudEventListener listener2 = new SingleLoudGCEventListener("Microsoft-Windows-DotNETRuntime");
            // Invoke 3 GCs
            GC.Collect();
            GC.Collect();
            GC.Collect();
            Thread.Sleep(1000);

            listener1.VerifyMinAndReportError("Test_Listener_RuntimeEvents_ManyListener_Listener1", "GCStart_V2", 3);
            listener2.VerifyMinAndReportError("Test_Listener_RuntimeEvents_ManyListener_Listener2", "GCStart_V2", 3);

        }

        /// <summary>
        /// Tests setting up 2 listeners, with 1 of them listening to nonexistent events
        /// </summary>
        static void Test_Listener_RuntimeEvents_WrongListener()
        {
            SingleLoudEventListener listener1 = new SingleLoudGCEventListener("Microsoft-Windows-DotNETRuntime");
            SingleLoudEventListener listener2 = new SingleLoudEventListener("SomeOtherSource");
            // Invoke 3 GCs
            GC.Collect();
            GC.Collect();
            GC.Collect();

            listener1.VerifyMinAndReportError("Test_Listener_RuntimeEvents_ManyListener_Listener1", "GCStart_V2", 3);
            listener2.VerifyLessThanAndReportError("Test_Listener_RuntimeEvents_ManyListener_Listener2", "GCStart_V2", 1);
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

        static void Test_ThreadPool_Listener()
        {
            using (SingleLoudEventListener listener = new SingleLoudThreadEventListener("Microsoft-Windows-DotNETRuntime"))
            {
                int someNumber = 0;
                Task[] tasks = new Task[100];
                for (int i = 0; i < 100; i++) 
                {
                    tasks[i] = Task.Run(() => { Task.Delay(i*100); someNumber += 1; });
                }

                Thread.Sleep(1000);

                listener.VerifyMinAndReportError("Test_ThreadPool_Listener", "ThreadPoolWorkerThreadStart", 1);
                
                listener.VerifyMinAndReportError("Test_ThreadPool_Listener", "ThreadCreated", 1);
                listener.VerifyMinAndReportError("Test_ThreadPool_Listener", "ThreadCreating", 1);
                
                listener.VerifyMinAndReportError("Test_ThreadPool_Listener", "ThreadRunning", 1);
                listener.VerifyMinAndReportError("Test_ThreadPool_Listener", "ThreadPoolWorkerThreadWait", 1);
            }
        }

    }
}
