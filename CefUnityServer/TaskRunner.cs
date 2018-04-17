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

        protected BrowserHost host;
        protected PipeServer server;

        protected List<ITaskRunnable> taskList;

        public TaskRunner()
        {
            this.taskList = new List<ITaskRunnable>();
        }

        public void ClearTasks()
        {
            lock (taskList)
            {
                this.taskList.Clear();
            }
        }

        public void AddTask(ITaskRunnable task)
        {
            lock (taskList)
            {
                this.taskList.Add(task);
            }
        }

        public void Stop()
        {
            KeepAlive = false;
        }

        public void RunOnThread(BrowserHost host, PipeServer server)
        {
            this.host = host;
            this.server = server;

            try
            {
                Logr.Log("Started task runner.");
                
                while (KeepAlive)
                {
                    lock (taskList)
                    {
                        while (taskList.Count > 0)
                        {
                            var nextTask = taskList[0];
                            taskList.RemoveAt(0);

                            try
                            {
                                nextTask.Run(host, server);
                            }
                            catch (Exception ex)
                            {
                                Logr.Log("Exception while executing internal task:", ex.Message, ex);
                            }
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
