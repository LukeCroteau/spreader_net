using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Spreader;

namespace Spreader_Test_Worker
{
    class Program
    {
        private static void LogMessage(string message)
        {
            Console.WriteLine(string.Format("{0} {1}", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), message));
        }

        static void Main(string[] args)
        {
            LogMessage("Starting Worker");
            Worker wrkr = new Worker(1);

            TaskAwaiter<bool> awaiter = wrkr.Start().GetAwaiter();

            LogMessage("Ready to wait");
            Task.Delay(3000).Wait();
            LogMessage("Stopping Main Loop");

            wrkr.Stop();

            bool success = awaiter.GetResult();
            LogMessage($"Got result {success}");

            LogMessage("Finished. Press any key to close.");
            Console.ReadKey();
        }
    }
}
