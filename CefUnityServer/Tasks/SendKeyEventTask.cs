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
            switch (this.keyEventMsg.KeyEventType)
            {
                case KeyEventPipeMessage.TYPE_KEY_DOWN:
                    host.SendKeyDownEvent(keyEventMsg.KeyCode);
                    break;
                case KeyEventPipeMessage.TYPE_KEY_UP:
                    host.SendKeyUpEvent(keyEventMsg.KeyCode);
                    break;
                default:
                    Logr.Log("Key event task failed: Unrecognized event type:", this.keyEventMsg.KeyEventType);
                    break;
            }
        }
    }
}
