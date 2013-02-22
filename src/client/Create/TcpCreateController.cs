//------------------------------------------------------------------------------
// <copyright file="TcpCreateController.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Create
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Logging;

    public class TcpCreateController : ICreateController
    {
        private const string HandshakeGreeting = "HELO";
        private const string ReadyResponse = "REDY";
        private const string AcknowledgementResponse = "ACK";
        private const string ErrorResponse = "ERR";

        private TcpClient client;
        private IPAddress ip;
        private int port;

        public TcpCreateController(IPAddress ip, int port)
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

        public TcpCreateController(string ip, int port)
            : this(IPAddress.Parse(ip), port)
        {
        }

        ~TcpCreateController()
        {
            this.client.Close();
        }

        public void PowerOn()
        {
            //TODO: This needs defined in the protocol
            throw new NotImplementedException();
        }

        public void PowerOff()
        {
            //TODO: This needs defined in the protocol
            throw new NotImplementedException();
        }

        public void SetMode(CreateMode mode)
        {
            this.ExecuteCommand("MODE {0}", mode.ToString().ToUpper());
        }

        public void Drive(int velocity, int radius)
        {
            this.ExecuteCommand("DRIVE NORMAL {0} {1}", velocity, radius);
        }

        public void Drive(int velocity)
        {
            this.ExecuteCommand("DRIVE STRAIGHT {0}", velocity);
        }

        public void DriveDistance(int velocity, int radius, int distance)
        {
            this.ExecuteCommand("DRIVE DISTANCE {0} {1} {2}", velocity, radius, distance);
        }

        public void DriveDistance(int velocity, int distance)
        {
            this.ExecuteCommand("DRIVE STRAIGHT DISTANCE {0} {1}", velocity, distance);
        }

        public void DriveTime(int velocity, int radius, int time)
        {
            this.ExecuteCommand("DRIVE TIME {0} {1} {2}", velocity, radius, time);
        }

        public void DriveTime(int velocity, int time)
        {
            this.ExecuteCommand("DRIVE STRAIGHT TIME {0} {1}", velocity, time);
        }

        public void DriveDirect(int leftVelocity, int rightVelocity)
        {
            this.ExecuteCommand("DRIVE DIRECT {0} {1}", leftVelocity, rightVelocity);
        }

        public void Turn(int velocity, int radius, int degrees)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            this.ExecuteCommand("DRIVE STOP");
        }

        public void SetLed(Led led, bool onOff)
        {
            if (led == Led.Power && onOff == false)
            {
                this.SetPowerLed(0, 0);
            }
            else
            {
                this.ExecuteCommand("LED {0} {1}", led.ToString().ToUpper(), onOff == true ? "ON" : "OFF");
            }
        }

        public void SetPowerLed(int color, int intensity)
        {
            // Check that the arguments are valid
            if (color < 0 || color > 255)
            {
                throw new ArgumentException("color must be between 0 and 255", "color");
            }

            if (intensity < 0 || intensity > 255)
            {
                throw new ArgumentException("intensity must be between 0 and 255", "intensity");
            }

            this.ExecuteCommand("LED POWER {0} {1}", color, intensity);
        }

        public void FlashLed(Led led, int flashCount, int flashDuration)
        {
            this.ExecuteCommand("LED FLASH {0} {1} {2}", led.ToString().ToUpper(), flashCount, flashDuration);
        }

        public void Beep()
        {
            this.ExecuteCommand("BEEP");
        }

        public void DefineSong(int songNumber, int[] notes, int[] durations)
        {
            //TODO: In what format do we pass notes and duration?
            this.ExecuteCommand("SONG DEFINE {0} {1} {2}", songNumber, string.Join(",", notes), string.Join(",", durations));
        }

        public void PlaySong(int songNumber)
        {
            this.ExecuteCommand("SONG PLAY {0}", songNumber);
        }

        private void ExecuteCommand(string commandFormat, params object[] args)
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