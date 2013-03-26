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

    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;

    using Modes;
    using Sensing.Gestures;

    public class AsimovClient
    {
        private static ManualResetEvent endEvent;

        private static ICreateController roomba;

        private static KinectSensor kinect;

        private static ModeController modeController;

        private static ICollection<IGesture> gestures;  

        public static void Main(string[] args)
        {
            try
            {
                KinectSensorChooser sensorChooser = new KinectSensorChooser();

                endEvent = new ManualResetEvent(false);
                roomba = new AsimovController(new ConsoleCreateCommunicator());
                modeController = new ModeController();
                gestures = InitGestures();

                // Find the Kinect and subscribe to changes in the sensor
                sensorChooser.KinectChanged += KinectSensorChanged;
                sensorChooser.Start();

                // Do not exit until endEvent is fired
                endEvent.WaitOne();
                roomba.Dispose();
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
            BothArmsOutGesture armsOut = new BothArmsOutGesture();
            BothArmsUpGesture armsUp = new BothArmsUpGesture();
            BothElbowsBentUpGesture elbowsBent = new BothElbowsBentUpGesture();
            LeftArmBentDownRightArmDownGesture leftBentDown = new LeftArmBentDownRightArmDownGesture();
            LeftArmDownRightArmBentDownGesture rightBentDown = new LeftArmDownRightArmBentDownGesture();
            LeftArmDownRightArmBentUpGesture rightBentUp = new LeftArmDownRightArmBentUpGesture();
            LeftArmDownRightArmOutGesture rightOut = new LeftArmDownRightArmOutGesture();
            LeftArmOutRightArmDownGesture leftOut = new LeftArmOutRightArmDownGesture();
            LeftArmOutRightArmUpGesture leftOutRightUp = new LeftArmOutRightArmUpGesture();
            LeftArmUpRightArmOutGesture leftUpRightOut = new LeftArmUpRightArmOutGesture();

            // Subscribe to gesture-related events
            armsUp.BothArmsUpRecognized += OnBothArmsUp;
            armsOut.BothArmsOutRecognized += OnBothArmsOut;
            elbowsBent.BothElbowsBentUpRecognized += OnBothElbowsBentUp;
            leftBentDown.LeftArmBentDownRightArmDownRecognized += OnLeftArmBentDownRightArmDown;
            rightBentDown.LeftArmDownRightArmBentDownRecognized += OnLeftArmDownRightArmBentDown;
            rightBentUp.LeftArmDownRightArmBentUpRecognized += OnLeftArmDownRightArmBentUp;
            rightOut.LeftArmDownRightArmOutRecognized += OnLeftArmDownRightArmOut;
            leftOut.LeftArmOutRightArmDownRecognized += OnLeftArmOutRightArmDown;
            leftOutRightUp.LeftArmOutRightArmUpRecognized += OnLeftArmOutRightArmUp;
            leftUpRightOut.LeftArmUpRightArmOutRecognized += OnLeftArmUpRightArmOut;

            // Add gestures to the collection
            retval.Add(armsUp);
            retval.Add(armsOut);
            retval.Add(elbowsBent);
            retval.Add(leftBentDown);
            retval.Add(rightBentDown);
            retval.Add(rightBentUp);
            retval.Add(rightOut);
            retval.Add(leftOut);
            retval.Add(leftOutRightUp);
            retval.Add(leftUpRightOut);

            return retval;
        }

        private static void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            // Save all of the skeletons
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            // If we have skeletons to process
            if (skeletons.Length != 0)
            {
                // Process each skeleton
                foreach (Skeleton skeleton in skeletons)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        // Send the new skeleton to the mode controller
                        modeController.UpdateSkeleton(skeleton);

                        // Send the new skeleton to gesture handlers
                        foreach (IGesture gesture in gestures)
                        {
                            gesture.UpdateGesture(skeleton);
                        }
                    }
                    else if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        //TODO
                    }
                }
            }
        }

        private static void KinectSensorChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.NewSensor != null)
            {
                kinect = args.NewSensor;

                // Add an event handler to be called whenever there is new color frame data
                kinect.SkeletonFrameReady += SensorSkeletonFrameReady;
                
                // Turn on the skeleton stream to receive skeleton frames
                kinect.SkeletonStream.Enable();

                // Tilt the Kinect to allow for the best view of the skeletons
                kinect.ElevationAngle = 0;
            }

            if (args.OldSensor != null)
            {
                // Turn off the skelton sensor and unsubscribe from its events
                args.OldSensor.SkeletonFrameReady -= SensorSkeletonFrameReady;
                args.OldSensor.SkeletonStream.Disable();
            }
        }

        private static void OnBothArmsOut(object sender, EventArgs e)
        {
            // Drinking Mode
            Console.WriteLine("BothArmsOut Gesture Recognized");
            AsimovLog.WriteLine("BothArmsOut Gesture Recognized");

            // Set the mode to drinking mode
            modeController.CurrentMode = AsimovMode.Drinking;
            AsimovLog.WriteLine("Mode set to drinking mode.");
        }

        private static void OnBothArmsUp(object sender, EventArgs e)
        {
            // Exit Mode
            Console.WriteLine("BothArmsUp Gesture Recognized");
            AsimovLog.WriteLine("BothArmsUp Gesture Recognized");

            // Set the mode to none in order to exit the current mode
            modeController.CurrentMode = AsimovMode.None;
            AsimovLog.WriteLine("Mode set to none.");
        }

        private static void OnBothElbowsBentUp(object sender, EventArgs e)
        {
            // Exit Mode
            Console.WriteLine("BothElbowsBentUp Gesture Recognized");
            AsimovLog.WriteLine("BothElbowsBentUp Gesture Recognized");

            // Set the mode to none in order to exit the current mode
            modeController.CurrentMode = AsimovMode.None;
            AsimovLog.WriteLine("Mode set to follow.");
        }

        private static void OnLeftArmBentDownRightArmDown(object sender, EventArgs e)
        {
            // Exit Mode
            Console.WriteLine("LeftArmBentDownRightArmDown Gesture Recognized");
            AsimovLog.WriteLine("LeftArmBentDownRightArmDown Gesture Recognized");

            // Set the mode to none in order to exit the current mode
            modeController.CurrentMode = AsimovMode.None;
            AsimovLog.WriteLine("Mode set to turn around.");
        }

        private static void OnLeftArmDownRightArmBentDown(object sender, EventArgs e)
        {
            // Exit Mode
            Console.WriteLine("LeftArmDownRightArmBentDown Gesture Recognized");
            AsimovLog.WriteLine("LeftArmDownRightArmBentDown Gesture Recognized");

            // Set the mode to none in order to exit the current mode
            modeController.CurrentMode = AsimovMode.None;
            AsimovLog.WriteLine("Mode set to move away.");
        }

        private static void OnLeftArmDownRightArmBentUp(object sender, EventArgs e)
        {
            // Exit Mode
            Console.WriteLine("LeftArmDownRightArmBentUp Gesture Recognized");
            AsimovLog.WriteLine("LeftArmDownRightArmBentUp Gesture Recognized");

            // Set the mode to none in order to exit the current mode
            modeController.CurrentMode = AsimovMode.None;
            AsimovLog.WriteLine("Mode set to move forward.");
        }

        private static void OnLeftArmDownRightArmOut(object sender, EventArgs e)
        {
            // Exit Mode
            Console.WriteLine("LeftArmDownRightArmOut Gesture Recognized");
            AsimovLog.WriteLine("LeftArmDownRightArmOut Gesture Recognized");

            // Set the mode to none in order to exit the current mode
            modeController.CurrentMode = AsimovMode.None;
            AsimovLog.WriteLine("Mode set to turn right.");
        }

        private static void OnLeftArmOutRightArmDown(object sender, EventArgs e)
        {
            // Exit Mode
            Console.WriteLine("LeftArmOutRightArmDown Gesture Recognized");
            AsimovLog.WriteLine("LeftArmOutRightArmDown Gesture Recognized");

            // Set the mode to none in order to exit the current mode
            modeController.CurrentMode = AsimovMode.None;
            AsimovLog.WriteLine("Mode set to turn left.");
        }

        private static void OnLeftArmOutRightArmUp(object sender, EventArgs e)
        {
            // Exit Mode
            Console.WriteLine("LeftArmOutRightArmUp Gesture Recognized");
            AsimovLog.WriteLine("LeftArmOutRightArmUp Gesture Recognized");

            // Set the mode to none in order to exit the current mode
            modeController.CurrentMode = AsimovMode.None;
            AsimovLog.WriteLine("Mode set to center.");
        }

        private static void OnLeftArmUpRightArmOut(object sender, EventArgs e)
        {
            // Exit Mode
            Console.WriteLine("LeftArmUpRightArmOut Gesture Recognized");
            AsimovLog.WriteLine("LeftArmUpRightArmOut Gesture Recognized");

            // Set the mode to none in order to exit the current mode
            modeController.CurrentMode = AsimovMode.None;
            AsimovLog.WriteLine("Mode set to none.");
        }

        private static void ConfirmModeChange()
        {
            roomba.FlashLed(Led.Advance, 2, 500);
            roomba.Beep();
            roomba.Beep();
        }
    }
}
