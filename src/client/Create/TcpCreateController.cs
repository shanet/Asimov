namespace AsimovClient.Create
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class TcpCreateController : ICreateController
    {
        private TcpClient client;
        private IPAddress ip;
        private int port;

        public TcpCreateController(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;

            try
            {
                Debug.WriteLine("Connecting to server at {0}:{1}...", ip.ToString(), port);

                this.client = new TcpClient();
                this.client.Connect(this.ip, this.port);

                Debug.WriteLine("Connected!");
            }
            catch (SocketException e)
            {
                Console.Error.WriteLine("ERROR: Could not connect to the Asimov server.");
                throw e;
            }
        }

        public TcpCreateController(String ip, int port)
            : this(IPAddress.Parse(ip), port)
        {
        }

        ~TcpCreateController()
        {
            this.client.Close();
        }

        public void PowerOn()
        {
            throw new NotImplementedException();
        }

        public void PowerOff()
        {
            throw new NotImplementedException();
        }

        public void SetMode(CreateMode mode)
        {
            throw new NotImplementedException();
        }

        public void Drive(int velocity, int radius, int distance)
        {
            throw new NotImplementedException();
        }

        public void Turn(int velocity, int radius, int degrees)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        private void SendCommand(String command)
        {
            NetworkStream stream = client.GetStream();
            Byte[] data = Encoding.ASCII.GetBytes(command);

            try
            {
                Debug.WriteLine("Sending command \"{0}\"...", command);

                stream.Write(data, 0, data.Length);

                Debug.WriteLine("Command sent!");
            }
            catch (IOException)
            {
                Console.Error.WriteLine("ERROR: Could not communicate with the server.");
            }
        }
    }
}

