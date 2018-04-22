using CefUnityLib.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefUnityServer.Tasks
{
    public class SetMouseTask : ITaskRunnable
    {
        protected MouseEventPipeMessage message;

        public SetMouseTask(MouseEventPipeMessage message)
        {
            this.message = message;
        }

        public void Run(BrowserHost host, PipeServer server)
        {
            host.HandleMouseEvent(message);
        }
    }
}
