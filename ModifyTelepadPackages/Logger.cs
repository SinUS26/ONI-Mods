using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace ModifyTelepadPackages
{
    public class Logger
    {
        private static bool toLog = true;
        private static string Prefix => string.Format("[{0}][{1}]", "ModiFyTelepadPackages", Thread.CurrentThread.ManagedThreadId);

        public static void Print(string message)
        {
            if (!toLog)
                return;
            Debug.Log("" + Logger.Prefix + ": " + message + "");
        }

        public static void Print(Component component)
        {
            if (!toLog)
                return;
            Debug.Log(string.Format("{0}: {1}: {2}", Logger.Prefix, component.name, component.GetType()));
        }

        public static void Header(string message)
        {
            if (!toLog)
                return;
            Debug.Log("" + Logger.Prefix + ": " + message + "");
        }

        /* public static void Start(string message)
        {
            if (Console.Out.GetType().ToString() == "UnityEngine.UnityLogWriter")
                Debug.Log(Logger.Prefix + ": " + message);
            else
                Debug.Log("\u001B[48;5;21m  " + Logger.Prefix + ": " + message + " \u001B[48;5;0m");
        } */

        public static void Print(System.Exception message)
        {
            if (!toLog)
                return;
            Debug.Log(string.Format("{0}: Exception : {1}", (object)Logger.Prefix, (object)message));
        }

        public static void PrintError(string msg, string stackTrace)
        {
            if (!toLog)
                return;
            Debug.Log("" + Logger.Prefix + ": ERROR: " + msg + "\n" + stackTrace + "");
        }

        public static void PrintError(string msg)
        {
            if (!toLog)
                return;
            string str = string.Join("\n", ((IEnumerable<string>)Environment.StackTrace.Split('\n')).Skip<string>(1).ToArray<string>());
            Debug.Log("" + Logger.Prefix + ": " + msg + "\n" + str + "");
        }

        public static void PrintAction(string msg)
        {
            if (!toLog)
                return;
            Debug.Log("" + Logger.Prefix + ": ACTION: " + msg + "");
        }
    }
}
