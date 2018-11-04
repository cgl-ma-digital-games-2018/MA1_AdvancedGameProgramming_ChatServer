using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TestConsoleClient
{
    internal class Client
    {
        public string IpAddress { get; }
        public int Port { get; }
        public bool IsConnected { get; private set; } = false;

        private TcpClient _tcpClient = null;

        #region Constructor
        public Client(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }
        #endregion

        public void Connect()
        {
            try
            {
                _tcpClient = new TcpClient(IpAddress, Port);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            // Creates new thread for user input.
            //var userInputThread = new Thread(HandleInput);
            //userInputThread.Start();

            using (var sReader = new StreamReader(_tcpClient.GetStream()))
            using (var sWriter = new StreamWriter(_tcpClient.GetStream()))
            {
                Console.Clear();
                //sWriter.AutoFlush = true;
                IsConnected = true;
                Console.WriteLine($"Connected to server at IP Address: {IpAddress}, Port: {Port} \n");

                // Handles incoming communication.
                while (true)
                {
                    var incomingMessage = sReader.ReadLine();
                    Console.WriteLine(incomingMessage);

                    Console.Write(">> ");
                    var outgoingMessage = Console.ReadLine() ?? string.Empty;

                    if (outgoingMessage != string.Empty)
                    {
                        sWriter.WriteLine(outgoingMessage);
                    }
                }
            }
        }

        private void HandleInput()
        {
            using (var sWriter = new StreamWriter(_tcpClient.GetStream()))
            {
                var outgoingMessage = string.Empty;

                // Loops for non-empty user input.
                while (!outgoingMessage.StartsWith("exit"))
                {
                    Console.WriteLine("writer stream id: " + _tcpClient.GetStream().GetHashCode());
                    Console.Write(">> ");
                    outgoingMessage = Console.ReadLine() ?? string.Empty;

                    sWriter.WriteLine(outgoingMessage);
                }
            }
        }
    }
}