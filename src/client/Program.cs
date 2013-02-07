namespace AsimovClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Roomba;

    public class Program
    {
        static void Main(string[] args)
        {
            RoombaController roomba = new RoombaController("127.0.0.1", 5000);
        }
    }
}
