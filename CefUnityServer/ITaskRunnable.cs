using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefUnityServer
{
    public interface ITaskRunnable
    {
        void Run(BrowserHost host, PipeServer server);
    }
}
