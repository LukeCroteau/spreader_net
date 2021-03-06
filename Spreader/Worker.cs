﻿using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using static Spreader.Utilities;

namespace Spreader
{
    public delegate void ScannerMethod(Worker ScanningWorker);

    public class Scanner
    {
        public int LoopEvery;
        public ScannerMethod Method;
        public DateTime LastRun;
    }

    public class Worker
    {
        private bool running = false;
        private bool ClientInitialized = false;
        private Socket _socket = null;
        private readonly object _transfer_lock = new object();
        private DateTime LastKeepAlive = DateTime.Today;
        private DateTime LastStartAttempt = DateTime.Today;
        private List<string> AccessCodes = new List<string>();
        private List<Scanner> Scanners = new List<Scanner>();

        public int Port = 21200;
        public bool DebugMode = false;
        public int WorkerID = 0;
        public int JobID = 0;
        public Dictionary<string, string> JobParameters = new Dictionary<string, string>();

        /// <summary>
        /// If DebugMode is enabled, this will output a copy of all log messages.
        /// Derived classes might want to override this to display messages on Console, in a log file, etc.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        protected virtual void WriteDebugLog(string message)
        {
            Debug.WriteLine(message);
        }

        /// <summary>
        /// This method handles the work of doing the assigned tasks.
        /// Derived classes should implement this method to perform the task work required.
        /// </summary>
        /// <param name="task_parameters">A tokenized list of parameters. Your implementation will define how you split the parameters.</param>
        /// <returns>True if the task was processed successfully.</returns>
        protected virtual bool DoTask(string task_parameters)
        {
            return false;
        }

        protected virtual void InitJobParameters(string job_parameters)
        {
            string[] InitJobParams = job_parameters.Split(CommandSeparator);

            foreach (string item in InitJobParams)
                if (!string.IsNullOrEmpty(item))
                    JobParameters[item.Split('=')[0]] = item.Split('=')[1];
        }

        private bool SendToSocket(string command, string message = "")
        {
            if (_socket == null)
                return false;

            if (DebugMode)
                WriteDebugLog(string.Format("DATA OUT - Command {0}, Message {1}", command, message));
            lock (_transfer_lock)
            {
                string outmsg = string.Format("{0}{1}{2}{1}{3}\r\n", command, CommandSeparator, WorkerID, message);
                byte[] msg = Encoding.UTF8.GetBytes(outmsg);
                _socket.Send(msg, SocketFlags.None);
            }

            return true;
        }

        private string ReadResponse()
        {
            string response = "";
            byte[] inmsg = new byte[4096];
            lock (_transfer_lock)
            {
                int recvcount = 0;
                DateTime runstarted = DateTime.Now;
                while (DateTime.Now < runstarted.AddSeconds(5))
                {
                    try
                    {
                        recvcount = _socket.Receive(inmsg);
                    }
                    catch (Exception excp)
                    {
                        if (excp is ObjectDisposedException)
                        {
                            LogDebug(string.Format("Socket Exception: {0}", excp.Message), false);
                            LogDebug("Closing Socket.", false);
                            _socket = null;
                            return "";
                        }
                    }

                    if (recvcount > 0)
                    {
                        LastKeepAlive = DateTime.Now;
                        char[] trimChars = { '\0' };
                        response = Encoding.UTF8.GetString(inmsg).Trim(trimChars);
                        if (DebugMode)
                            WriteDebugLog(string.Format("DATA IN  - Message {0}", response));

                        break;
                    }

                    Thread.Sleep(25);
                }
            }
            return response;
        }

        private bool ReceiveFromSocket(out string data)
        {
            data = "";
            byte[] inmsg = new byte[4096];
            lock (_transfer_lock)
            {
                int recvcount = 0;
                try
                {
                    recvcount = _socket.Receive(inmsg);
                }
                catch (Exception excp)
                {
                    if (excp is ObjectDisposedException)
                    {
                        LogDebug(string.Format("Socket Exception: {0}", excp.Message), false);
                        LogDebug("Closing Socket.", false);
                        _socket = null;
                        return false;
                    }
                }

                if (recvcount > 0)
                {
                    LastKeepAlive = DateTime.Now;
                    char[] trimChars = { '\0' };
                    data = Encoding.UTF8.GetString(inmsg).Trim(trimChars);
                    if (DebugMode)
                        WriteDebugLog(string.Format("DATA IN  - Message {0}", data));
                    return true;
                }
                else
                    return false;
            }
        }

        private void SendLog(Utilities.SpreaderLogLevel level, string message)
        {
            SendToSocket("WKRLOG", string.Format("{0}{1}{2}", (int)level, CommandSeparator, message));
        }

        /// <summary>
        /// Writes a Debug Log Entry
        /// </summary>
        /// <param name="message">Message to Log</param>
        /// <param name="sendToClient">Determines whether we send this to the Client for permanent logging, Defaults to False</param>
        public void LogDebug(string message, bool sendToClient = true)
        {
            if (DebugMode)
                WriteDebugLog(message);

            if (sendToClient)
                SendLog(Utilities.SpreaderLogLevel.LOG_DEBUG, message);
        }

        /// <summary>
        /// Writes a Message Log Entry
        /// </summary>
        /// <param name="message">Message to Log</param>
        public void LogMessage(string message)
        {
            SendLog(Utilities.SpreaderLogLevel.LOG_MESSAGE, message);
        }

        /// <summary>
        /// Writes a Warning Log Entry
        /// </summary>
        /// <param name="message">Message to Log</param>
        public void LogWarning(string message)
        {
            SendLog(Utilities.SpreaderLogLevel.LOG_WARNING, message);
        }

        /// <summary>
        /// Writes an Error Log Entry
        /// </summary>
        /// <param name="message">Message to Log</param>
        public void LogError(string message)
        {
            SendLog(Utilities.SpreaderLogLevel.LOG_ERROR, message);
        }

        /// <summary>
        /// Writes a Fatal Log Entry
        /// </summary>
        /// <param name="message">Message to Log</param>
        public void LogFatal(string message)
        {
            SendLog(Utilities.SpreaderLogLevel.LOG_FATAL, message);
        }

        /// <summary>
        /// Adds a new task for the Job
        /// </summary>
        /// <param name="taskKey">A unique key for this task</param>
        /// <param name="taskParams">Any parameters this task might need.
        /// These parameters will be Encoded for protection.</param>
        /// <param name="accessCode">The access code required to process this task.</param>
        /// <returns>The ID of the new Task, or 0 if task adding failed.</returns>
        public int AddNewTask(string taskKey, string taskParams, string accessCode)
        {
            SendToSocket("WKRTASKADD", JobID.ToString() + CommandSeparator +
                Utilities.EncodeParameters(taskKey) + CommandSeparator +
                Utilities.EncodeParameters(taskParams) + CommandSeparator +
                Utilities.EncodeParameters(accessCode));

            string resp = ReadResponse();
            string result = resp.Split(CommandSeparator)[0];
            int responseVal = 0;
            if (result != "0")
            {
                int.TryParse(resp.Split(CommandSeparator)[1], out responseVal);
            }
            
            return responseVal;
        }

        private void CheckKeepAlive()
        {
            DateTime current = DateTime.Now;
            double diff = (current - LastKeepAlive).TotalMilliseconds;
            if (diff > Utilities.WorkerTimeoutLimit)
            {
                LogDebug("Lost Connection to Client", false);
            }
            else if (diff > Utilities.WorkerPingLimit)
            {
                SendToSocket("WKRPING");
            }
        }

        private bool HasAccess(string access_code)
        {
            return AccessCodes.Contains(access_code);
        }

        private void DoScan()
        {
            foreach (Scanner scan in Scanners)
            {
                if (DateTime.Now >= scan.LastRun.AddMilliseconds(scan.LoopEvery))
                {
                    scan.Method(this);
                    scan.LastRun = DateTime.Now;
                }
            }
        }

        private void SendWorkerStart()
        {
            if (ClientInitialized)
                return;

            if ((DateTime.Now - LastStartAttempt).TotalMilliseconds > 10000)
            {
                LastStartAttempt = DateTime.Now;
                LogDebug("Client has not yet sent Initialization. Attempting to start.", false);
                SendToSocket("WKRSTARTED");
            }
        }

        private void ProcessClientData(string data)
        {
            string command = "";
            string parms = "";
            int xpos = data.IndexOf(CommandSeparator);
            if (xpos > -1)
            {
                command = data.Substring(0, xpos);
                parms = data.Substring(xpos + 1);
            }
            else
            {
                command = data;
            }

            switch (command)
            {
                case "WKRINIT":
                    HandleClientInit(parms);
                    break;
                case "WKRPING":
                    HandleClientPing();
                    break;
                case "WKRPONG":
                    HandleClientPong();
                    break;
                case "WKRSTOP":
                    LogDebug("Received Stop, Quitting.", false);
                    running = false;
                    break;
                case "WKRTASK":
                    LogDebug(string.Format("Received a Task! Params {0}", parms), false);
                    HandleClientTask(parms);
                    break;
                case "WKREVENT":
                    LogDebug(string.Format("Received Event: {0}", parms), false);
                    break;
                default:
                    LogDebug(string.Format("Unknown Command {0}, Quitting.", command), false);
                    running = false;
                    break;
            }
        }

        private void HandleClientInit(string parameters)
        {
            string[] parms = parameters.Split(CommandSeparator);

            string codes = parms[0];
            foreach (string code in codes.Split(InitSeparator))
            {
                if (!AccessCodes.Contains(code))
                    AccessCodes.Add(code);
            }

            if (parms.Length > 1)
                int.TryParse(parms[1], out JobID);

            if (parms.Length > 2)
                InitJobParameters(Utilities.DecodeParameters(parms[2]));

            ClientInitialized = true;

            SendToSocket("WKRINITIALIZED");
            SendToSocket("WKREVENTSUBSCRIBE", string.Format("NEW_TASK_{0}", JobID));
        }

        private void HandleClientPing()
        {
            SendToSocket("WKRPONG");
        }

        private void HandleClientPong()
        {
            LastKeepAlive = DateTime.Now;
        }

        private void HandleClientTask(string task_data)
        {
            string[] data = task_data.Split(CommandSeparator);
            string taskid = data[0];
            string parms = data[1];

            bool success = DoTask(DecodeParameters(parms));
            SendToSocket("WKRTASKDONE", string.Format("{0}{1}{2}", taskid, CommandSeparator, success ? "1" : "0"));
        }

        /// <summary>
        /// Starts the Worker thread.
        /// </summary>
        /// <returns>
        /// True if the task was execute successfully.
        /// False, if an exception or other error occurred.
        /// </returns>
        public async Task<bool> Start()
        {
            if (running || _socket != null)
                return false;

            bool returnval = false;

            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.ReceiveTimeout = 25;
                _socket.SendTimeout = 100;
                _socket.ReceiveBufferSize = 4096;
                _socket.Connect("localhost", Port);
            }
            catch (Exception ex)
            {
                _socket = null;
                LogDebug(string.Format("Connect Failed: {0}", ex.Message), false);
                return false;
            }

            if (_socket == null)
            {
                LogDebug("Unable to connect to socket", false);
                return false;
            }

            running = true;
            LastKeepAlive = DateTime.Now;

            try
            {
                while (running && _socket != null)
                {
                    if (!ClientInitialized)
                        SendWorkerStart();

                    if (ReceiveFromSocket(out string client_data))
                        foreach (string line in client_data.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                            ProcessClientData(line);

                    CheckKeepAlive();

                    if (HasAccess("SCAN"))
                        DoScan();

                    await Task.Delay(25);
                }

                returnval = true;
            }
            catch
            {
                returnval = false;
            }

            LogDebug("Command Loop Stopped", false);

            _socket = null;

            return returnval;
        }

        /// <summary>
        /// Stops the Worker from executing.
        /// </summary>
        public void Stop()
        {
            running = false;
        }

        /// <summary>
        /// Registers a Simple Scanner.
        /// The supplied method will scan at a regular interval, and execute synchronously.
        /// </summary>
        /// <param name="method">The scanner method to execute.</param>
        /// <param name="loop_every">How often to execute the scan method, in Milliseconds. (5,000 by default)</param>
        public void RegisterSimpleScanner(ScannerMethod method, int loop_every = 5000)
        {
            if (method != null && loop_every > 100)
            {
                Scanner tmpScan = new Scanner()
                {
                    Method = method,
                    LastRun = DateTime.Now,
                    LoopEvery = loop_every
                };
                Scanners.Add(tmpScan);
            }
        }
    }
}
