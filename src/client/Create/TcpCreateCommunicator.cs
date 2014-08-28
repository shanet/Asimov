//------------------------------------------------------------------------------
// <copyright file="TcpCreateCommunicator.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
// <summary>
//     Class that implements ICreateCommunicator to transmit all commands over
//     the network using TCP/IP.
// </summary>
//------------------------------------------------------------------------------

namespace AsimovClient.Create
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    using Logging;

    public class TcpCreateCommunicator : ICreateCommunicator
    {
        private const string DefaultIPAddress = "127.0.0.1";
        private const int DefaultPort = 4545;
        
        private const string HandshakeGreeting = "HELO";
        private const string ReadyResponse = "REDY";
        private const string AcknowledgementResponse = "ACK";
        private const string ErrorResponse = "ERR";
        private const string EndCommand = "END";

        private TcpClient client;
        private IPAddress ip;
        private int port;

        public TcpCreateCommunicator()
            : this(DefaultIPAddress, DefaultPort)
        { // Empty
        }

        public TcpCreateCommunicator(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;

            try
            {
                AsimovLog.WriteLine("Connecting to server at {0}:{1}...", ip.ToString(), port);

                this.client = new TcpClient();
                this.client.Connect(this.ip, this.port);

                AsimovLog.WriteLine("Connected!");

                AsimovLog.WriteLine("Attempting handshake...");

                this.Handshake();

                AsimovLog.WriteLine("Handshake completed! Ready to execute commands.");
            }
            catch (SocketException e)
            {
                Console.Error.WriteLine("ERROR: Could not connect to the Asimov server.");
                throw e;
            }
        }

        public TcpCreateCommunicator(string ip, int port)
            : this(IPAddress.Parse(ip), port)
        {
        }

        public bool ExecuteCommand(string commandFormat, params object[] args)
        {
            string command = string.Format(commandFormat, args);
            string response;

            // Send the command and get a response
            this.SendCommand(command);
            response = this.RecieveResponse();

            // Verify the command was acknowledged
            if (string.Compare(response, AcknowledgementResponse, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                AsimovLog.WriteLine("ERROR: Recieved response \"{0}\"; expected \"{1}\"", response, AcknowledgementResponse);
                Console.Error.WriteLine("ERROR: Recieved response \"{0}\"; expected \"{1}\"", response, AcknowledgementResponse);
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.client != null)
                {
                    // Send the end command and close the connection
                    this.SendCommand(EndCommand);
                    this.client.Close();
                    this.client = null;
                }
            }
        }

        private void Handshake()
        {
            string response;

            this.SendCommand(HandshakeGreeting);
            response = this.RecieveResponse();

            if (string.Compare(response, ReadyResponse, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                throw new Exception("The handshake was not successful.");
            }
        }

        private void SendCommand(string command)
        {
            NetworkStream stream = this.client.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(command + "\n");

            try
            {
                AsimovLog.WriteLine("Sending command \"{0}\"...", command);

                stream.Write(data, 0, data.Length);

                AsimovLog.WriteLine("Command sent!");
            }
            catch (IOException)
            {
                AsimovLog.WriteLine("ERROR: Could not communicate with the server.");
                Console.Error.WriteLine("ERROR: Could not communicate with the server.");
            }
        }

        private string RecieveResponse()
        {
            NetworkStream stream = this.client.GetStream();
            StringBuilder response = new StringBuilder();
            byte[] data = new byte[this.client.ReceiveBufferSize];
            string dataString = string.Empty;
            int readCount;

            try
            {
                AsimovLog.WriteLine("Checking for a response from the server...");

                // Keep reading as long as there is new line
                while (dataString.Contains("\n") == false)
                {
                    readCount = stream.Read(data, 0, this.client.ReceiveBufferSize);
                    dataString = Encoding.ASCII.GetString(data, 0, readCount);
                    response.Append(dataString);
                }

                // Remove the new line
                response.Replace("\r", string.Empty).Replace("\n", string.Empty);

                AsimovLog.WriteLine("Recieved response: \"{0}\"", response.ToString());
            }
            catch (IOException)
            {
                AsimovLog.WriteLine("ERROR: Could not communicate with the server.");
                Console.Error.WriteLine("ERROR: Could not communicate with the server.");
            }

            return response.ToString();
        }
    }
}
