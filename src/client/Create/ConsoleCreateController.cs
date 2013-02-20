using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsimovClient.Create
{
    public class ConsoleCreateController : ICreateController
    {
        public void PowerOn()
        {
            Console.WriteLine("Power on.");
        }

        public void PowerOff()
        {
            Console.WriteLine("Power off.");
        }

        public void SetMode(CreateMode mode)
        {
            Console.WriteLine("Mode set to {0}.", mode.ToString());
        }

        public void Drive(int velocity, int radius, int distance)
        {
            //TODO: Velocity units
            Console.WriteLine("Driving {0} mm at {1} ?? with radius {2}.", distance, velocity, radius);
        }

        public void Turn(int velocity, int radius, int degrees)
        {
            //TODO: Velocity units
            Console.WriteLine("Turning {0} degrees at {1} ?? with radius {2}.", degrees, velocity, radius);
        }

        public void Stop()
        {
            Console.WriteLine("Stop.");
        }
    }
}
