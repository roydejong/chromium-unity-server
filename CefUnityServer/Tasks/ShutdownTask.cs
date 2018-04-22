namespace CefUnityServer.Tasks
{
    class ShutdownTask : ITaskRunnable
    {
        public void Run(BrowserHost host, PipeServer server)
        {
            Logr.Log("Received shutdown request from network.");
            Program.ShutDown();
        }
    }
}