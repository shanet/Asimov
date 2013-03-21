//------------------------------------------------------------------------------
// <copyright file="AsimovClient.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;
    using Create;
    using Logging;
    using Microsoft.Kinect.Toolkit;
    using Sensing;
    using Sensing.Gestures;

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
                roomba = new AsimovController(new ConsoleCreateCommunicator());                
                endEvent = new ManualResetEvent(false);
                sensorChooser = new KinectSensorChooser();
                personLocator = new PersonLocator(sensorChooser, InitGestures());

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

        public static void TestGestureReaction(object sender, object shouldBeNull)
        {
            // Todo
            Console.WriteLine("Gesture Recognized");
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

        private static ICollection<IGesture> InitGestures()
        {
            ICollection<IGesture> retval = new Collection<IGesture>();

            // Create gestures
            UpUpGesture upup = new UpUpGesture();

            // Subscribe to gesture-related events
            upup.UpUpRecognized += TestGestureReaction;

            // Add gestures to the collection
            retval.Add(upup);

            return retval;
        }
    }
}
