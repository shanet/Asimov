//------------------------------------------------------------------------------
// <copyright file="ConsoleCreateController.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Create
{
    using System;

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

        public void Turn(int velocity, int radius, int degrees)
        {
            Console.WriteLine("Turn {0} degrees at {1} mm/ms with radius {2} mm.", degrees, velocity, radius);
        }

        public void Stop()
        {
            Console.WriteLine("Stop.");
        }

        public void Drive(int velocity, int radius)
        {
            Console.WriteLine("Drive at {0} mm/ms with radius {1} mm.", velocity, radius);
        }

        public void Drive(int velocity)
        {
            Console.WriteLine("Drive straight at {0} mm/ms.", velocity);
        }

        public void DriveDistance(int velocity, int radius, int distance)
        {
            Console.WriteLine("Drive {0} mm at {1} mm/ms with radius {2} mm.", distance, velocity, radius);
        }

        public void DriveDistance(int velocity, int distance)
        {
            Console.WriteLine("Drive straight {0} mm at {1} mm/ms.", distance, velocity);
        }

        public void DriveTime(int velocity, int radius, int time)
        {
            Console.WriteLine("Drive for {0} ms at {1} mm/ms with radius {2} mm.", time, velocity, radius);
        }

        public void DriveTime(int velocity, int time)
        {
            Console.WriteLine("Drive straight for {0} ms at {1} mm/ms.", time, velocity);
        }

        public void DriveDirect(int leftVelocity, int rightVelocity)
        {
            Console.WriteLine("Drive with a left wheel velocity of {0} mm/ms and a right wheel velocity of {1} mm/ms.", leftVelocity, rightVelocity);
        }

        public void SetLed(Led led, bool onOff)
        {
            Console.WriteLine("Turn the {0} LED {1}.", led.ToString(), onOff == true ? "on" : "off");
        }

        public void SetPowerLed(int color, int intensity)
        {
            Console.WriteLine("Set the Power LED to a color of {0} and an intensity of {1}.", color, intensity);
        }

        public void FlashLed(Led led, int flashCount, int flashDuration)
        {
            Console.WriteLine("Flash the {0} LED {1} time(s) with a duration of {2} ms each.", led.ToString(), flashCount, flashDuration);
        }

        public void Beep()
        {
            Console.WriteLine("Beep.");
        }

        public void DefineSong(int songNumber, int[] notes, int[] durations)
        {
            Console.WriteLine("Set song #{0} to {{{1}}} with durations {{{2}}}.", songNumber, string.Join(",", notes), string.Join(",", durations));
        }

        public void PlaySong(int songNumber)
        {
            Console.WriteLine("Play song #{0}.", songNumber);
        }
    }
}
