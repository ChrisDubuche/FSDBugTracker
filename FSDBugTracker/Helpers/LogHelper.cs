using System;
using System.IO;

namespace FSDBugTracker.Helpers
{
    public class LogHelper
    {
        public static string _file = "DebugLog.txt";
        public static object _locked = new object();

        public static void AppendToLog(string text)
        {
            lock (_locked)
            {
                string path = @"C:\DebugTemp\";
                File.AppendAllText(Path.Combine(path, _file), text + Environment.NewLine);
            }
        }
    }
}
