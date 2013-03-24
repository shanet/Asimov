//------------------------------------------------------------------------------
// <copyright file="AsimovController.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Create
{
    using System;

    using Helpers;
    using Logging;

    public class AsimovController : ICreateController
    {
        private ICreateCommunicator communicator;

        public AsimovController(ICreateCommunicator communicator)
        {
            Verify.ArgumentNotNull(communicator, "communicator");

            this.communicator = communicator;
        }

        public void SetMode(CreateMode mode)
        {
            AsimovLog.WriteLine("Setting mode to to {0}.", mode);

            this.communicator.ExecuteCommand("MODE {0}", mode.ToString().ToUpper());
        }

        public void Drive(double velocity, double radius)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");
            Verify.ArgumentInRange(radius, CreateConstants.RadiusMin, CreateConstants.RadiusMax, "radius");

            AsimovLog.WriteLine("Driving at {0} mm/s with radius {1} mm.", (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(radius));

            this.communicator.ExecuteCommand("DRIVE NORMAL {0} {1}", (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(radius));
        }

        public void Drive(double velocity)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");

            AsimovLog.WriteLine("Driving straight at {0} mm/s.", (int)Units.BaseToMilli(velocity));

            this.communicator.ExecuteCommand("DRIVE STRAIGHT NORMAL {0}", (int)Units.BaseToMilli(velocity));
        }

        public void DriveDistance(double velocity, double radius, double distance)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");
            Verify.ArgumentInRange(radius, CreateConstants.RadiusMin, CreateConstants.RadiusMax, "radius");

            AsimovLog.WriteLine("Driving for {0} mm at {1} mm/s with radius {2} mm.", distance, (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(radius));

            // Ensure that the sign of the distance argument matches the sign of the velocity.  Otherwise, we'll drive forever.
            distance = Math.Sign(velocity) * Math.Abs(distance);

            this.communicator.ExecuteCommand("DRIVE DISTANCE {0} {1} {2}", (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(radius), (int)Units.BaseToMilli(distance));
        }

        public void DriveDistance(double velocity, double distance)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");

            AsimovLog.WriteLine("Driving straight for {0} mm at {1} mm/s.", distance, (int)Units.BaseToMilli(velocity));

            // Ensure that the sign of the distance argument matches the sign of the velocity.  Otherwise, we'll drive forever.
            distance = Math.Sign(velocity) * Math.Abs(distance);

            this.communicator.ExecuteCommand("DRIVE STRAIGHT DISTANCE {0} {1}", (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(distance));
        }

        public void DriveTime(double velocity, double radius, double time)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");
            Verify.ArgumentInRange(radius, CreateConstants.RadiusMin, CreateConstants.RadiusMax, "radius");
            Verify.ArgumentAtLeast(time, CreateConstants.TimeMin, "time");

            AsimovLog.WriteLine("Driving for {0} ms at {1} mm/s with radius {2} mm.", (int)Units.BaseToMilli(time), (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(radius));

            this.communicator.ExecuteCommand("DRIVE TIME {0} {1} {2}", (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(radius), (int)Units.BaseToMilli(time));
        }

        public void DriveTime(double velocity, double time)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");
            Verify.ArgumentAtLeast(time, CreateConstants.TimeMin, "time");

            AsimovLog.WriteLine("Driving straight for {0} ms at {1} mm/s.", (int)Units.BaseToMilli(time), (int)Units.BaseToMilli(velocity));

            this.communicator.ExecuteCommand("DRIVE STRAIGHT TIME {0} {1}", (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(time));
        }

        public void DriveDirect(double leftVelocity, double rightVelocity)
        {
            Verify.ArgumentInRange(leftVelocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "leftVelocity");
            Verify.ArgumentInRange(rightVelocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "rightVelocity");

            AsimovLog.WriteLine("Driving with a left wheel velocity of {0} mm/s and a right wheel velocity of {1} mm/s.", (int)Units.BaseToMilli(leftVelocity), (int)Units.BaseToMilli(rightVelocity));

            this.communicator.ExecuteCommand("DRIVE DIRECT {0} {1}", (int)Units.BaseToMilli(leftVelocity), (int)Units.BaseToMilli(rightVelocity));
        }

        public void Spin(double velocity)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");

            AsimovLog.WriteLine("Spinning with a velocity of {0} mm/s {1}clockwise.", Math.Abs((int)Units.BaseToMilli(velocity)), velocity < 0 ? "counter" : string.Empty);

            this.communicator.ExecuteCommand("DRIVE SPIN NORMAL {0}", (int)Units.BaseToMilli(velocity));
        }

        public void SpinAngle(double velocity, int degrees)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");

            AsimovLog.WriteLine("Spinning for {0} degrees with a velocity of {1} mm/s {2}clockwise.", degrees, Math.Abs((int)Units.BaseToMilli(velocity)), velocity < 0 ? "counter" : string.Empty);

            // Ensure that the sign of the degrees argument is opposite the the sign of the velocity.  Otherwise, we'll spin forever.
            degrees = Math.Sign(velocity) * -Math.Abs(degrees);

            this.communicator.ExecuteCommand("DRIVE SPIN ANGLE {0} {1}", (int)Units.BaseToMilli(velocity), degrees);
        }

        public void SpinTime(double velocity, double time)
        {
            Verify.ArgumentInRange(velocity, CreateConstants.VelocityMin, CreateConstants.VelocityMax, "velocity");
            Verify.ArgumentAtLeast(time, CreateConstants.TimeMin, "time");

            AsimovLog.WriteLine("Spinning for {0} ms with a velocity of {1} mm/s {2}clockwise.", (int)Units.BaseToMilli(time), Math.Abs((int)Units.BaseToMilli(velocity)), velocity < 0 ? "counter" : string.Empty);

            this.communicator.ExecuteCommand("DRIVE SPIN TIME {0} {1}", (int)Units.BaseToMilli(velocity), (int)Units.BaseToMilli(time));
        }

        public void Stop()
        {
            AsimovLog.WriteLine("Stopping.");

            this.communicator.ExecuteCommand("DRIVE STOP");
        }

        public void SetLed(Led led, bool onOff)
        {
            AsimovLog.WriteLine("Turnning the {0} LED {1}.", led.ToString(), onOff == true ? "on" : "off");

            if (led == Led.Power && onOff == false)
            {
                this.SetPowerLed(0, 0);
            }
            else
            {
                this.communicator.ExecuteCommand("LED {0} {1}", led.ToString().ToUpper(), onOff == true ? "ON" : "OFF");
            }
        }

        public void SetPowerLed(int color, int intensity)
        {
            Verify.ArgumentInRange(color, CreateConstants.PowerLedColorMin, CreateConstants.PowerLedColorMax, "color");
            Verify.ArgumentInRange(intensity, CreateConstants.PowerLedIntensityMin, CreateConstants.PowerLedIntensityMax, "intensity");

            AsimovLog.WriteLine("Setting the Power LED to a color of {0} and an intensity of {1}.", color, intensity);

            this.communicator.ExecuteCommand("LED POWER {0} {1}", color, intensity);
        }

        public void FlashLed(Led led, int flashCount, int flashDuration)
        {
            AsimovLog.WriteLine("Flashing the {0} LED {1} time(s) with a duration of {2} ms each.", led.ToString(), flashCount, flashDuration);

            this.communicator.ExecuteCommand("LED FLASH {0} {1} {2}", led.ToString().ToUpper(), flashCount, flashDuration);
        }

        public void Beep()
        {
            AsimovLog.WriteLine("Beeping.");

            this.communicator.ExecuteCommand("BEEP");

            // Let the beep be heard before doing anything else
            System.Threading.Thread.Sleep(40);
        }

        public void DefineSong(int songNumber, int[] notes, int[] durations)
        {
            AsimovLog.WriteLine("Setting song #{0} to {{{1}}} with durations {{{2}}}.", songNumber, string.Join(",", notes), string.Join(",", durations));

            this.communicator.ExecuteCommand("SONG DEFINE {0} {1} {2}", songNumber, string.Join(",", notes), string.Join(",", durations));
        }

        public void PlaySong(int songNumber)
        {
            AsimovLog.WriteLine("Playing song #{0}.", songNumber);

            this.communicator.ExecuteCommand("SONG PLAY {0}", songNumber);
        }

        public void WaitTime(double time)
        {
            Verify.ArgumentAtLeast(time, CreateConstants.TimeMin, "time");

            AsimovLog.WriteLine("Waiting for {0} ms.", (int)Units.BaseToMilli(time));

            this.communicator.ExecuteCommand("WAIT TIME {0}", (int)Units.BaseToMilli(time));
        }

        public void WaitDistance(double distance)
        {
            AsimovLog.WriteLine("Waiting until we travel {0} mm.", (int)Units.BaseToMilli(distance));

            this.communicator.ExecuteCommand("WAIT DISTANCE {0}", (int)Units.BaseToMilli(distance));
        }

        public void WaitAngle(int angle)
        {
            AsimovLog.WriteLine("Waiting until we rotate {0} degrees.", angle);

            // The Create uses positive for counterclockwise, so we need to negate the argument.
            angle = -angle;

            this.communicator.ExecuteCommand("WAIT ANGLE {0}", angle);
        }

        public void WaitEvent(WaitEvent waitEvent)
        {
            AsimovLog.WriteLine("Waiting until the event \"{0}\" occurs.", waitEvent);

            this.communicator.ExecuteCommand("WAIT EVENT {0}", (int)waitEvent);
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
                if (this.communicator != null)
                {
                    this.communicator.Dispose();
                    this.communicator = null;
                }
            }
        }
    }
}
