namespace AsimovClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Create;

    public class AsimovClient
    {
        static void Main(string[] args)
        {
            ICreateController roomba = new ConsoleCreateController();

            roomba.PowerOn();
            roomba.Drive(1, 2, 3);
            roomba.PowerOff();
        }
    }
}
