using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefUnityServer
{
    public static class Logr
    {
        private static Object syncLock = new object();
        private static DateTime? firstLog;

        private static string MakeTimeStamp()
        {
            if (!firstLog.HasValue)
            {
                firstLog = DateTime.Now;
            }

            var secondsSinceStart = (DateTime.Now - firstLog).Value.TotalSeconds;
            var secondsFormatted = secondsSinceStart.ToString("N3").Replace(',', '.');

            var padToLength = 10;

            var sb = new StringBuilder();
            sb.Append("[");
            while (sb.Length < (padToLength - 2 - secondsFormatted.Length))
                sb.Append(" ");
            sb.Append(secondsFormatted);
            sb.Append("]");

            return sb.ToString();
        }

        public static void Log(params object[] things)
        {
            lock (syncLock)
            {
                var jointMessage =
                (
                    MakeTimeStamp() + " " +
                    String.Join(" ", things)
                );

                if (Console.Out != null)
                {
                    Console.WriteLine(jointMessage);
                }

                Debug.WriteLine(jointMessage);
            }
        }
    }
}