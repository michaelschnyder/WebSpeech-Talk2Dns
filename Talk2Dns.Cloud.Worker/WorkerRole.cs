using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ARSoft.Tools.Net.Dns;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Talk2Dns.Core;

namespace Talk2Dns.Cloud.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private Talk2DnsServer server;

        public override void Run()
        {
            Trace.TraceInformation("Talk2Dns.Cloud.Worker is running");

            this.server.Start();

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            var roleInstanceEndpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["DnsEndpoint"];

            this.server = new Talk2DnsServer(roleInstanceEndpoint.IPEndpoint, "t2d.fas-net.ch");

            Trace.TraceInformation("Talk2Dns.Cloud.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Talk2Dns.Cloud.Worker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            this.server.Stop();

            base.OnStop();

            Trace.TraceInformation("Talk2Dns.Cloud.Worker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
