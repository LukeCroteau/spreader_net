using System;
using Spreader;

namespace Spreader_Test_Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Worker");
            Worker wrkr = new Worker();

            Console.WriteLine("Finished. Press any key to close.");
            Console.ReadKey();
        }
    }
}
