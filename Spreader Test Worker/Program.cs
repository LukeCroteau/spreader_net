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
            LogMessage("Starting Worker");
            MyWorker wrkr = new MyWorker()
            {
                WorkerID = 1,
                DebugMode = true
            };

            TaskAwaiter<bool> awaiter = wrkr.Start().GetAwaiter();

            LogMessage("Press any key to send a message");
            Console.ReadKey();
            wrkr.LogMessage("Local Worker Starting!");

            LogMessage("Press any key to Stop Worker.");
            Console.ReadKey();

            wrkr.Stop();

            bool success = awaiter.GetResult();
            LogMessage($"Got result {success}");

            LogMessage("Finished. Press any key to close.");
            Console.ReadKey();
        }
    }
}
