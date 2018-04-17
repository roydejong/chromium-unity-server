using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefUnityServer.Tasks
{
    class RepaintFrameTask : ITaskRunnable
    {
        public void Run(BrowserHost host, PipeServer server)
        {
            host.Repaint();
        }
    }
}
