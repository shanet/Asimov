//------------------------------------------------------------------------------
// <copyright file="ConsoleCreateController.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Create
{
    using System;
    using Helpers;

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
            Console.WriteLine("Mode set to {0}.", mode);
        }

        public void Turn(double velocity, double radius, int degrees)
        {
            Console.WriteLine("Turn {0} degrees at {1} mm/s with radius {2} mm.", degrees, (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(radius));
        }

        public void Stop()
        {
            Console.WriteLine("Stop.");
        }

        public void Drive(double velocity, double radius)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");
            Verify.ArgumentInRange(radius, CreateConstants.RadiusMin, CreateConstants.RadiusMax, "radius");

            Console.WriteLine("Drive at {0} mm/s with radius {1} mm.", (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(radius));
        }

        public void Drive(double velocity)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");

            Console.WriteLine("Drive straight at {0} mm/s.", (int)Units.BaseToMilli(velocity));
        }

        public void DriveDistance(double velocity, double radius, double distance)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");
            Verify.ArgumentInRange(radius, CreateConstants.RadiusMin, CreateConstants.RadiusMax, "radius");

            Console.WriteLine("Drive {0} mm at {1} mm/s with radius {2} mm.", distance, (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(radius));
        }

        public void DriveDistance(double velocity, double distance)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");

            Console.WriteLine("Drive straight {0} mm at {1} mm/s.", distance, (int)Units.BaseToMilli(velocity));
        }

        public void DriveTime(double velocity, double radius, double time)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");

            Console.WriteLine("Drive for {0} ms at {1} mm/s with radius {2} mm.", (int)Units.BaseToMilli(time), (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(radius));
        }

        public void DriveTime(double velocity, double time)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");

            Console.WriteLine("Drive straight for {0} ms at {1} mm/s.", (int)Units.BaseToMilli(time), (int)Units.BaseToMilli(velocity));
        }

        public void DriveDirect(double leftVelocity, double rightVelocity)
        {
            Verify.ArgumentInRange(leftVelocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "leftVelocity");
            Verify.ArgumentInRange(rightVelocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "rightVelocity");

            Console.WriteLine("Drive with a left wheel velocity of {0} mm/s and a right wheel velocity of {1} mm/s.", (int)Units.BaseToMilli(leftVelocity), (int)Units.BaseToMilli(rightVelocity));
        }

        public void Spin(double velocity)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");

            Console.WriteLine("Spin with a velocity of {0} mm/s {1}clockwise.", Math.Abs((int)Units.BaseToMilli(velocity)), velocity < 0 ? "counter" : string.Empty);
        }

        public void SpinDistance(double velocity, int degrees)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");

            Console.WriteLine("Spin {0} degrees with a velocity of {1} mm/s {2}clockwise.", degrees, Math.Abs((int)Units.BaseToMilli(velocity)), velocity < 0 ? "counter" : string.Empty);
        }

        public void SpinTime(double velocity, double time)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");

            Console.WriteLine("Spin for {0} ms with a velocity of {1} mm/s {2}clockwise.", (int)Units.BaseToMilli(time), Math.Abs((int)Units.BaseToMilli(velocity)), velocity < 0 ? "counter" : string.Empty);
        }

        public void SetLed(Led led, bool onOff)
        {
            Console.WriteLine("Turn the {0} LED {1}.", led.ToString(), onOff == true ? "on" : "off");
        }

        public void SetPowerLed(int color, int intensity)
        {
            Verify.ArgumentInRange(color, CreateConstants.PowerLedColorMin, CreateConstants.PowerLedColorMax, "color");
            Verify.ArgumentInRange(intensity, CreateConstants.PowerLedIntensityMin, CreateConstants.PowerLedIntensityMax, "intensity");

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
