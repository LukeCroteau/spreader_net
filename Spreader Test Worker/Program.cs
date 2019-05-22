using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Spreader;

namespace Spreader_Test_Worker
{
    class MyWorker : Worker
    {
        protected override void WriteDebugLog(string message)
        {
            Console.WriteLine(string.Format("{0} DL {1}", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), message));
        }

        protected override bool DoTask(string task_parameters)
        {
            LogError("Could not execute Task!!!");
            // TODO
            return false;
        }
    }

    class Program
    {
        private static void LogMessage(string message)
        {
            Console.WriteLine(string.Format("{0} LM {1}", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), message));
        }

        static void Main(string[] args)
        {
            int workerId = 0;
            bool debugMode = false;

            foreach (string arg in args)
            {
                string[] argsplit = arg.Split('=');
                string xarg = argsplit[0];
                switch (xarg)
                {
                    case "/ID":
                        int.TryParse(xarg, out workerId);
                        break;
                    case "/DEBUG":
                        debugMode = true;
                        break;
                    default:
                        break;
                }
            }

            LogMessage("Starting Worker");
            MyWorker wrkr = new MyWorker()
            {
                WorkerID = workerId,
                DebugMode = debugMode
            };

            TaskAwaiter<bool> awaiter = wrkr.Start().GetAwaiter();

            bool success = awaiter.GetResult();
            LogMessage($"Got result {success}");
        }
    }
}
