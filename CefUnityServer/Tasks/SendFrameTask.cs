using CefUnityLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefUnityServer.Tasks
{
    class SendFrameTask : ITaskRunnable
    {
        protected byte[] buffer;

        public SendFrameTask(byte[] bitmapData)
        {
            this.buffer = bitmapData;
        }

        public void Run(BrowserHost host, PipeServer server)
        {
            if (buffer != null && buffer.Length > 0)
            {
                //Logr.Log("Send frame");
                server.SendData(PipeProto.BytesToProtoMessage(buffer, PipeProto.OPCODE_FRAME));
            }
        }
    }
}
