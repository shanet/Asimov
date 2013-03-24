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
    using Modes;

    public class AsimovClient
    {
        private static ManualResetEvent endEvent;

        private static ICreateController roomba;

        private static KinectSensorChooser sensorChooser;

        private static PersonLocator personLocator;

        private static ModeController modeController;

        public static void Main(string[] args)
        {
            try
            {
                roomba = new AsimovController(new ConsoleCreateCommunicator());                
                endEvent = new ManualResetEvent(false);
                sensorChooser = new KinectSensorChooser();
                personLocator = new PersonLocator(sensorChooser, InitGestures());
                modeController = new ModeController();

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

        private static ICollection<IGesture> InitGestures()
        {
            ICollection<IGesture> retval = new Collection<IGesture>();

            // Create gestures
            BothArmsUpGesture armsUp = new BothArmsUpGesture();
            BothArmsDownGesture armsDown = new BothArmsDownGesture();
            BothArmsOutGesture armsOut = new BothArmsOutGesture();

            // Subscribe to gesture-related events
            armsUp.BothArmsUpRecognized += OnBothArmsUp;
            armsDown.BothArmsDownRecognized += OnBothArmsDown;
            armsOut.BothArmsOutRecognized += OnBothArmsOut;

            // Add gestures to the collection
            retval.Add(armsUp);
            retval.Add(armsDown);
            retval.Add(armsOut);

            return retval;
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

        public static void OnBothArmsUp(object sender, EventArgs e)
        {
            //TODO
            Console.WriteLine("BothArmsUp Gesture Recognized");
        }

        private static void OnBothArmsDown(object sender, EventArgs e)
        {
            //TODO
            Console.WriteLine("BothArmsDown Gesture Recognized");
        }

        private static void OnBothArmsOut(object sender, EventArgs e)
        {
            //TODO
            Console.WriteLine("BothArmsOut Gesture Recognized");
        }

        private static void ConfirmModeChange()
        {
            roomba.FlashLed(Led.Advance, 2, 500);
            roomba.Beep();
            roomba.Beep();
        }
    }
}
