using CefUnityLib;
using CefUnityServer.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CefUnityServer
{
    public class PipeServer
    {
        public const int NO_COMM_AUTO_SHUTDOWN_AFTER_SECS = 60;

        protected string pipeName;
        protected NamedPipeServerStream stream;
        protected bool KeepAlive = true;
        protected object syncLock = new object();
        protected TaskRunner runner;
        protected bool GotRemotePing = false;
        protected DateTime LastActivity;

        public PipeServer(string pipeName, TaskRunner runner)
        {
            // Auto prefix pipe name
            if (!pipeName.StartsWith(Consts.NAMED_PIPE_PREFIX))
            {
                pipeName = Consts.NAMED_PIPE_PREFIX + pipeName;
            }

            this.pipeName = pipeName;
            this.runner = runner;

            LastActivity = DateTime.Now;
        }

        protected void DoAutoShutdownCheck()
        {
            var now = DateTime.Now;

            if (KeepAlive && stream != null && stream.CanRead)
            {
                // Stream is still open, update activity and move on
                LastActivity = now;
                return;
            }

            var then = LastActivity;
            var diff = now - then;

            if (Math.Abs(diff.TotalSeconds) >= NO_COMM_AUTO_SHUTDOWN_AFTER_SECS)
            {
                Logr.Log(String.Format("Named pipe server: No network activity! Starting automatic shutdown. (Triggered after {0} seconds of inactivity.)", NO_COMM_AUTO_SHUTDOWN_AFTER_SECS));
                Program.ShutDown();
                return;
            }
        }

        public void StartAsNewTask()
        {
            // Background task thread: Auto shutdown after inactivity
            Task.Run(new Action(() =>
            {
                while (KeepAlive)
                {
                    DoAutoShutdownCheck();
                    Thread.Sleep(1000);
                }
            }));

            // Background task thread: Accept connections and receive incoming data
            Task.Run(new Action(() =>
            {
                LastActivity = DateTime.Now;

                while (KeepAlive)
                {
                    DoAutoShutdownCheck();

                    if (stream == null)
                    {
                        stream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                        Logr.Log(String.Format("Started named pipe server [{0}].", pipeName));
                    }

                    try
                    {
                        GotRemotePing = false;
                        stream.WaitForConnection();
                    }
                    catch (Exception e)
                    {
                        Logr.Log("Named pipe: Connection is failing.", e.Message);
                        KillStream();
                        continue;
                    }

                    try
                    {
                        Logr.Log("Named pipe: New connection established.");

                        // Force a repaint to cause frame to be re-sent to the connecting client
                        runner.AddTask(new RepaintFrameTask());

                        while (stream.CanRead && KeepAlive)
                        {
                            var incomingMessage = PipeProtoMessage.ReadFromStream(stream);

                            if (incomingMessage != null)
                            {
                                Task.Run(new Action(() =>
                                {
                                    HandleIncomingMessage(incomingMessage);
                                }));
                            }
                        }

                        Logr.Log("Named pipe: Connection is closing.");
                    }
                    catch (Exception ex)
                    {
                        Logr.Log("Named pipe connection had an unexpected failure:", ex.Message, ex);
                    }
                }
                
                Logr.Log("Named pipe server has shut down.");

                KillStream();
            }));
        }

        public void HandleIncomingMessage(PipeProtoMessage incomingMessage)
        {
            LastActivity = DateTime.Now;

            switch (incomingMessage.Opcode)
            {
                case PipeProto.OPCODE_PING:

                    GotRemotePing = true;
                    break;

                case PipeProto.OPCODE_FRAME:
                    
                    runner.AddTask(new RepaintFrameTask());
                    break;

                case PipeProto.OPCODE_MOUSE_EVENT:

                    runner.AddTask(new SetMouseTask(new CefUnityLib.Messages.MouseEventPipeMessage(incomingMessage.Payload)));
                    break;

                case PipeProto.OPCODE_MOUSE_WHEEL_EVENT:

                    runner.AddTask(new SendMouseWheelEventTask(new CefUnityLib.Messages.MouseWheelEventPipeMessage(incomingMessage.Payload)));
                    break;

                case PipeProto.OPCODE_KEY_EVENT:

                    runner.AddTask(new SendKeyEventTask(new CefUnityLib.Messages.KeyEventPipeMessage(incomingMessage.Payload)));
                    break;

                case PipeProto.OPCODE_SHUTDOWN:

                    runner.AddTask(new ShutdownTask());
                    break;

                default:

                    Logr.Log("Pipe server error. Could not route message due to unrecognized packet opcode:", incomingMessage.Opcode);
                    break;
            }
        }

        public void SendData(byte[] data)
        {
            if (stream != null && stream.IsConnected && GotRemotePing)
            {
                lock (stream)
                {
                    try
                    {
                        stream.Write(data, 0, data.Length);
                        stream.Flush();
                    }
                    catch (Exception ex)
                    {
                        Logr.Log("Pipe send error:", ex);
                    }
                }
            }
        }

        public void Stop()
        {
            KeepAlive = false;
            KillStream();
        }

        private void KillStream()
        {
            if (stream != null)
            {
                try
                {
                    stream.Close();
                    stream.Dispose();
                }
                catch (Exception) { }
            }

            stream = null;
        }
    }
}
