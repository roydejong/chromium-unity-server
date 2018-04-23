using CefUnityLib.Messages;

namespace CefUnityServer.Tasks
{
    public class SendMouseWheelEventTask : ITaskRunnable
    {
        protected MouseWheelEventPipeMessage mouseWheelMsg;

        public SendMouseWheelEventTask(MouseWheelEventPipeMessage message)
        {
            this.mouseWheelMsg = message;
        }

        public void Run(BrowserHost host, PipeServer server)
        {
            host.HandleMouseWheelEvent(mouseWheelMsg);
        }
    }
}
