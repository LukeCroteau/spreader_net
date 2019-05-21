using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace Spreader
{
    public class Worker
    {
        private bool running = false;
        private ConcurrentQueue<string> command_queue = new ConcurrentQueue<string>();
        public int Port = 21200;
        public bool DebugMode = false;
        private int _WorkerID = 0;
        public int WorkerID
        {
            get { return _WorkerID; }
        }
        private Socket _socket = null;

        public Worker(int ID)
        {
            _WorkerID = ID;
        }

        private async Task<bool> SendToSocket(string message)
        {
            if (_socket == null)
                return false;

            byte[] msg = Encoding.UTF8.GetBytes(message);
            ArraySegment<byte> seg = new ArraySegment<byte>(msg);
            await _socket.SendAsync(seg, SocketFlags.None);

            return true;
        }

        private async Task<bool> LogDebug(string message, bool sendToClient = false)
        {
            Debug.WriteLine(string.Format("{0} {1}", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), message));

            if (sendToClient)
                await SendToSocket(message);

            return true;
        }

        private void ProcessQueueItem(string Command)
        {
            string verb = Command.Split('|')[0];
            switch (verb)
            {
                case "QUIT":
                    running = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Starts the Worker thread.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Start()
        {
            if (running || _socket != null)
                return false;

            bool returnval = false;
            
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect("localhost", Port);
            }
            catch (Exception ex)
            {
                _socket = null;
                await LogDebug(string.Format("Connect Failed: {0}", ex.Message));
                return false;
            }

            if (_socket == null)
            {
                await LogDebug("Unable to connect to socket");
                return false;
            }

            running = true;

            try
            {
                while (running)
                {
                    string next_command = "";

                    while (command_queue.TryDequeue(out next_command))
                    {
                        ProcessQueueItem(next_command);
                    }

                    await LogDebug("Waiting for work...");
                    await Task.Delay(250);
                }

                returnval = true;
            }
            catch
            {
                returnval = false;
            }

            await LogDebug("Command Loop Stopped");

            _socket = null;

            return returnval;
        }

        /// <summary>
        /// Adds a QUIT to the command queue.
        /// This will only do something after the command queue has started.
        /// </summary>
        public void Stop()
        {
            command_queue.Enqueue("QUIT");
        }
    }
}
