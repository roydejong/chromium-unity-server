using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CefUnityServer
{
    class Program
    {
        protected static BrowserHost browserHost;
        protected static PipeServer pipeServer;
        protected static TaskRunner taskRunner;

        public static void Main(string[] args)
        {
            // Read args
            string pipeName = args.Length > 0 ? args[0] : "";

            if (String.IsNullOrWhiteSpace(pipeName))
            {
                pipeName = "default";
            }

            // Environment prep
            Console.Title = String.Format("CEF Ingame Browser [{0}]", pipeName);

            Environment.ExitCode = 1;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            Logr.Log("Starting CefUnityServer at", DateTime.Now);

            // Initialize core components
            Program.taskRunner = new TaskRunner();
            Program.pipeServer = new PipeServer(pipeName, taskRunner);
            Program.browserHost = new BrowserHost(taskRunner);

            // Initialize CEF browser and wait for it to be ready
            Logr.Log("Waiting for Chromium to be ready.");

            Program.browserHost.Start();

            Logr.Log("Browser started and initialized.");

            // Begin named pipe server for data comm
            pipeServer.StartAsNewTask();

            // Begin task runner
            Program.taskRunner.RunOnThread(browserHost, pipeServer);

            // Clean shutdown
            Logr.Log("Nothing left to do on main thread. (Clean shutdown)");
            Environment.ExitCode = 0;

            // Wait before shutdown so msg is visible in console & everything shuts down clean
            Thread.Sleep(1000);
        }

        public static void ShutDown()
        {
            Logr.Log("Beginning shutdown procedure.");

            // Shut down pipe server
            Program.pipeServer.Stop();

            // Stop task runner
            Program.taskRunner.Stop();

            // Stop browser process
            Program.browserHost.Stop();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logr.Log("UNHANDLED EXCEPTION:", e.ExceptionObject);
            Logr.Log("Forcing shutdown.");

            Environment.Exit(2);
        }
    }
}
