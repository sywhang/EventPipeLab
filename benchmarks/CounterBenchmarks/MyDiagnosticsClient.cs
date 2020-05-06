using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Diagnostics.NETCore.Client;


namespace CounterBenchmarks
{
    public class MyDiagnosticsClient
    {
        private DiagnosticsClient m_client;
        private EventPipeSession m_session;

        public MyDiagnosticsClient()
        {
            m_client = new DiagnosticsClient(Process.GetCurrentProcess().Id);
        }

        public void Start(string providerName, EventLevel level, long keywords)
        {
            var provider = new EventPipeProvider(providerName, level, keywords);
            m_session = m_client.StartEventPipeSession(new List<EventPipeProvider>() { provider });
            // Task that reads and does nothing
            Task streamTask = Task.Run(() =>
            {
                var buffer = new byte[1000];
                while (true)
                {
                    try
                    {
                        m_session.EventStream.Read(buffer, 0, 1000);
                    }
                    catch (Exception e)
                    {
                    }
                }
            });
        }

        public void Stop()
        {
            m_session.Stop();
        }
    }
}