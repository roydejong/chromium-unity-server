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
            if (message.MouseEventType == MouseEventPipeMessage.TYPE_MOVE)
            {
                host.MoveMouse(message.CoordX, message.CoordY);
            }
            else if (message.MouseEventType == MouseEventPipeMessage.TYPE_MOUSE_DOWN)
            {
                host.ClickLeftMouse(message.CoordX, message.CoordY);
            }
            else if (message.MouseEventType == MouseEventPipeMessage.TYPE_MOUSE_UP)
            {
                host.ReleaseLeftMouse(message.CoordX, message.CoordY);
            }
        }
    }
}
