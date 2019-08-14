using CefUnityServer.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CefUnityServer
{
    public class TaskRunner
    {
        public bool KeepAlive = true;
        
        protected int _idleTicks;
        protected bool _enableForcedFps;
        protected int _autoRepaintTicks;

        protected Queue<ITaskRunnable> _taskList;

        public TaskRunner()
        {
            _taskList = new Queue<ITaskRunnable>();
        }

        public void AddTask(ITaskRunnable task)
        {
            lock (_taskList)
            {
                this._taskList.Enqueue(task);
            }
        }

        public void SetForcedFps(bool enableForcedFps, int autoRepaintTicks)
        {
            this._enableForcedFps = enableForcedFps;
            this._autoRepaintTicks = autoRepaintTicks;

            if (enableForcedFps)
            {
                Logr.Log($"Forced FPS mode: Repaint must happen every {autoRepaintTicks} ticks");
            }
        }

        public void Stop()
        {
            KeepAlive = false;
        }

        public void RunOnThread(BrowserHost host, PipeServer server)
        {
            SetForcedFps(true, 16);

            try
            {
                Logr.Log("Started task runner.");
                
                while (KeepAlive)
                {
                    lock (_taskList)
                    {
                        while (_taskList.Count > 0)
                        {
                            var nextTask = _taskList.Dequeue();

                            try
                            {
                                if (_enableForcedFps && (nextTask is SendFrameTask || nextTask is RepaintFrameTask))
                                {
                                    _idleTicks = 0;
                                }

                                nextTask.Run(host, server);
                            }
                            catch (Exception ex)
                            {
                                Logr.Log("Exception while executing internal task:", ex.Message, ex);
                            }
                        }

                        if (_enableForcedFps && _idleTicks++ >= _autoRepaintTicks)
                        {
                            host.Repaint();
                        }
                    }

                    Thread.Sleep(1); // we need to run more than 30 times per second for frame transfers
                }

                Logr.Log("Task runner shutting down.");
            }
            catch (ThreadInterruptedException) { }
            catch (ThreadAbortException) { }
        }
    }
}
