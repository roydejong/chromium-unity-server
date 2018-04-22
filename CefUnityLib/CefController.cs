using CefUnityLib.Helpers;
using CefUnityLib.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefUnityLib
{
    public class CefController
    {
        protected string pipeName;
        protected NamedPipeClientStream stream;
        protected StreamReader reader;
        protected StreamWriter writer;

        public EventHandler<PipeProtoMessage> MessageReceived;

        public string PipeName
        {
            get
            {
                return pipeName;
            }
        }

        public bool Connected
        {
            get
            {
                return stream != null && stream.IsConnected;
            }
        }

        public CefController(string pipeName = "default")
        {
            // Auto prefix pipe name
            if (!pipeName.StartsWith(Consts.NAMED_PIPE_PREFIX))
            {
                pipeName = Consts.NAMED_PIPE_PREFIX + pipeName;
            }

            this.pipeName = pipeName;
        }

        public bool SendMessage(PipeProtoMessage msg)
        {
            if (Connected)
            {
                msg.WriteToClientStream(stream, true);
                return true;
            }

            return false;
        }

        public void SendMouseEvent(byte eventType, Int32 coordX, Int32 coordY, MouseButtons mouseButtons)
        {
            SendMessage(new MouseEventPipeMessage(eventType, coordX, coordY, mouseButtons));
        }

#pragma warning disable CS4014
        public void Connect()
        {
            if (Connected)
            {
                return;
            }

            stream = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

            stream.Connect(3000); // may error, might want to implement custom exception type wrapper

            Task.Run(new Action(() =>
            {
                while (Connected)
                {
                    // Send ping message
                    var pingMessage = new PipeProtoMessage
                    {
                        Opcode = PipeProto.OPCODE_PING
                    };

                    pingMessage.WriteToClientStream(stream, true);

                    // Read next message
                    PipeProtoMessage incomingMessage = null;

                    try
                    {
                        incomingMessage = PipeProtoMessage.ReadFromStream(stream);
                    }
                    catch (Exception ex) { }

                    if (incomingMessage != null)
                    {
                        try
                        {
                            if (MessageReceived != null)
                            {
                                MessageReceived.Invoke(this, incomingMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Debug.WriteLine(ex.Message + " @ " + ex.StackTrace);
                        }
                    }
                }
            }));
        }
#pragma warning restore CS4014
    }
}