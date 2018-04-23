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
        public EventHandler<bool> ConnectionStateChanged;

        protected ulong statMsgsReceived = 0;
        protected ulong statMsgsSent = 0;

        public ulong MessagesReceivedCount
        {
            get
            {
                return statMsgsReceived;
            }
        }

        public ulong MessagesSentCount
        {
            get
            {
                return statMsgsSent;
            }
        }

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
                try
                {
                    msg.WriteToClientStream(stream, true);
                    statMsgsSent++;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Send message failure: " + ex.Message + " " + ex.StackTrace.ToString());

                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }

                    return false;
                }

                return true;
            }

            return false;
        }

        public void SendMouseEvent(byte eventType, Int32 coordX, Int32 coordY, MouseButtons mouseButtons)
        {
            SendMessage(new MouseEventPipeMessage(eventType, coordX, coordY, mouseButtons));
        }

        public void SendShutdownMessage()
        {
            SendMessage(new PipeProtoMessage
            {
                Opcode = PipeProto.OPCODE_SHUTDOWN
            });
        }

        public void Disconnect()
        {
            if (stream != null)
            {
                try
                {
                    stream.Dispose();
                }
                catch (Exception e) { }
            }

            stream = null;
            
            if (ConnectionStateChanged != null)
            {
                ConnectionStateChanged.Invoke(this, Connected);
            }
        }

#pragma warning disable CS4014
        public void Connect()
        {
            if (Connected)
            {
                return;
            }

            stream = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

            try
            {
                stream.Connect(3000);
            }
            catch (Exception ex)
            {
                Disconnect();

                if (ConnectionStateChanged != null)
                {
                    ConnectionStateChanged.Invoke(this, Connected);
                }

                throw ex;
            }

            Task.Run(new Action(() =>
            {
                if (ConnectionStateChanged != null)
                {
                    ConnectionStateChanged.Invoke(this, Connected);
                }
                
                // Send ping message
                var pingMessage = new PipeProtoMessage
                {
                    Opcode = PipeProto.OPCODE_PING
                };

                try
                {
                    pingMessage.WriteToClientStream(stream, true);
                    statMsgsSent++;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Send message failure: " + ex.Message + " " + ex.StackTrace.ToString());

                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }

                    Disconnect();
                }

                while (Connected)
                {
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
                                statMsgsReceived++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Debug.WriteLine(ex.Message + " @ " + ex.StackTrace);

                            if (Debugger.IsAttached)
                            {
                                Debugger.Break();
                            }
                        }
                    }
                }
            }));
        }
#pragma warning restore CS4014
    }
}