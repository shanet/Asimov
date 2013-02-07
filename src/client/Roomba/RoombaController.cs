namespace AsimovClient.Roomba
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class RoombaController
    {
        private TcpClient client;
        private IPAddress ip;
        private int port;

        public RoombaController(IPAddress ip, int port)
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

        public RoombaController(String ip, int port)
            : this(IPAddress.Parse(ip), port)
        {
        }

        ~RoombaController()
        {
            this.client.Close();
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
            catch (IOException e)
            {
                Console.Error.WriteLine("ERROR: Could not communicate with the server.");
            }
        }
    }
}

