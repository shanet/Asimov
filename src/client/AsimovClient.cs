//------------------------------------------------------------------------------
// <copyright file="AsimovClient.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient
{
    using System;
    using System.Threading;
    using Create;
    using Logging;
    using Microsoft.Kinect.Toolkit;
    using Sensing;

    public class AsimovClient
    {
        private static ManualResetEvent endEvent;

        private static ICreateController roomba;

        private static KinectSensorChooser sensorChooser;

        private static PersonLocator personLocator;

        public static void Main(string[] args)
        {
            try
            {
                roomba = new ConsoleCreateController();                
                endEvent = new ManualResetEvent(false);
                sensorChooser = new KinectSensorChooser();
                personLocator = new PersonLocator(sensorChooser);

                // Subscribe to events that we need to handle
                personLocator.OnPersonNotCentered += OnPersonNotCentered;
                personLocator.OnPersonCentered += OnPersonCentered;

                sensorChooser.Start();

                endEvent.WaitOne();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("FATAL ERROR: {0}", e);
                AsimovLog.WriteLine("FATAL ERROR: {0}", e);
            }
        }

        private static void OnPersonCentered(object sender)
        {
            roomba.Stop();
        }

        private static void OnPersonNotCentered(object sender, double angle)
        {
            //TODO: Turn a certain direction rather than a specific angle
            roomba.SpinAngle(Math.Sign(angle) * CreateConstants.VelocityMax, (int)angle);
        }
    }
}
