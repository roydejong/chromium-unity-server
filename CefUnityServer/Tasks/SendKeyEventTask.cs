using CefUnityLib.Messages;

namespace CefUnityServer.Tasks
{
    public class SendKeyEventTask : ITaskRunnable
    {
        protected KeyEventPipeMessage keyEventMsg;

        public SendKeyEventTask(KeyEventPipeMessage message)
        {
            this.keyEventMsg = message;
        }

        public void Run(BrowserHost host, PipeServer server)
        {
            host.HandleKeyEvent(keyEventMsg);
        }
    }
}
