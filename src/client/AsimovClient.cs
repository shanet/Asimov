﻿//------------------------------------------------------------------------------
// <copyright file="AsimovClient.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Threading;

    using Create;
    using Logging;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;
    using Modes;
    using Sensing.Gestures;

    public class AsimovClient
    {
        private static ManualResetEvent endEvent;

        private static ICreateController roomba;

        private static KinectSensor kinect;

        private static ModeController modeController;

        private static ICollection<IGesture> gestures;

        private static SpeechRecognitionEngine speechEngine;

        private static DateTime lastActionTime;

        public static void Main(string[] args)
        {
            try
            {
                KinectSensorChooser sensorChooser = new KinectSensorChooser();

                endEvent = new ManualResetEvent(false);
                roomba = new AsimovController(new ConsoleCreateCommunicator());
                modeController = new ModeController(roomba);
                gestures = InitGestures();
                lastActionTime = DateTime.MinValue;

                //TEMP - Maybe?
                roomba.Beep();
                roomba.SetMode(CreateMode.Full);
                //TEMP

                // Find the Kinect and subscribe to changes in the sensor
                sensorChooser.KinectChanged += KinectSensorChanged;
                sensorChooser.Start();

                // Listen for console input in a separate thread
                new Thread(ListenForConsoleInput).Start();

                // Block until endEvent is fired
                endEvent.WaitOne();

                // Dispose of the Create and end the connection with the server
                if (roomba != null)
                {
                    roomba.Dispose();
                }

                // Dispose of the Kinect
                if (kinect != null)
                {
                    kinect.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("FATAL ERROR: {0}", e);
                AsimovLog.WriteLine("FATAL ERROR: {0}", e);
            }
        }

        private static void ListenForConsoleInput()
        {
            string input;

            // Wait for the exit command
            do
            {
                input = Console.ReadLine();
            }
            while (string.Compare(input, Constants.ExitCommand, StringComparison.CurrentCultureIgnoreCase) != 0);

            // The exit command was recieved, so fire endEvent
            endEvent.Set();
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

                        // Only send a new skeleton to gesture recognizers if we haven't acted recently
                        if (DateTime.Now.Subtract(lastActionTime) > Constants.ActionWaitTime)
                        {
                            // Send the new skeleton to gesture handlers
                            foreach (IGesture gesture in gestures)
                            {
                                gesture.UpdateGesture(skeleton);
                            }
                        }
                    }
                    else if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        //TODO
                    }
                }
            }
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        private static void StartSpeechRecognition(RecognizerInfo ri)
        {
            speechEngine = new SpeechRecognitionEngine(ri.Id);
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(File.ReadAllText("SpeechGrammar.xml"))))
            {
                var g = new Grammar(memoryStream);
                speechEngine.LoadGrammar(g);
            }

            speechEngine.SpeechRecognized += SpeechRecognized;
            speechEngine.SpeechRecognitionRejected += SpeechRejected;

            speechEngine.SetInputToAudioStream(
                 kinect.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        private static void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            StringBuilder message = new StringBuilder("Speech input was rejected.");

            foreach (RecognizedPhrase phrase in e.Result.Alternates)
            {
                message.AppendFormat("\n  Rejected phrase: {0}\n  Confidence score: {1}\n  Grammar name:  {2}\n", phrase.Text, phrase.Confidence, phrase.Grammar.Name);
            }

            AsimovLog.WriteLine(message.ToString());

            roomba.Beep();
            roomba.Beep();
            roomba.Beep();
            roomba.Beep();
        }

        private static void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= Constants.SpeechConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "FORWARD":
                        Console.WriteLine("Move Forward Speech Recognized");
                        AsimovLog.WriteLine("Move Forward Speech Recognized");
            
                        // Verify we're not in a mode
                        if (AsimovMode.None == modeController.CurrentMode)
                        {
                            // Drive the Create forward a finite distance
                            roomba.DriveDistance(Constants.DefaultVelocity, Constants.DefaultDriveStep);
                            lastActionTime = DateTime.Now;
                            AsimovLog.WriteLine("Drove the Create forward 0.5 m.");
                        }

                        break;
                    case "BACKWARD":
                        Console.WriteLine("Move Backward Speech Recognized");
                        AsimovLog.WriteLine("Move Backward Speech Recognized");

                        // Verify we're not in a mode
                        if (AsimovMode.None == modeController.CurrentMode)
                        {
                            // Drive the Create backward a finite distance
                            roomba.DriveDistance(-Constants.DefaultVelocity, Constants.DefaultDriveStep);
                            lastActionTime = DateTime.Now;
                            AsimovLog.WriteLine("Drove the Create backward 0.5 m.");
                        }

                        break;
                    case "LEFT":
                        Console.WriteLine("Turn Left Speech Recognized");
                        AsimovLog.WriteLine("Turn Left Speech Recognized");

                        // Verify we're not in a mode
                        if (AsimovMode.None == modeController.CurrentMode)
                        {
                            // Turn the Create counterclockwise a finite number of degrees
                            roomba.SpinAngle(-Constants.DefaultVelocity, Constants.DefaultSpinStep);
                            lastActionTime = DateTime.Now;
                            AsimovLog.WriteLine("Turned the Create counterclockwise 30 degrees.");
                        }

                        break;
                    case "RIGHT":
                        Console.WriteLine("Turn Right Speech Recognized");
                        AsimovLog.WriteLine("Turn Right Speech Recognized");

                        // Verify we're not in a mode
                        if (AsimovMode.None == modeController.CurrentMode)
                        {
                            // Turn the Create clockwise a finite number of degrees
                            roomba.SpinAngle(Constants.DefaultVelocity, Constants.DefaultSpinStep);
                            lastActionTime = DateTime.Now;
                            AsimovLog.WriteLine("Turned the Create clockwise 30 degrees.");
                        }

                        break;
                    case "TURNAROUND":
                        Console.WriteLine("Turn Around Speech Recognized");
                        AsimovLog.WriteLine("Turn Around Speech Recognized");

                        // Verify we're not in a mode
                        if (AsimovMode.None == modeController.CurrentMode)
                        {
                            // Turn the Create 180 degrees.
                            roomba.SpinAngle(Constants.DefaultVelocity, 180);
                            lastActionTime = DateTime.Now;
                            AsimovLog.WriteLine("Turned the Create clockwise 180 degrees.");
                        }

                        break;
                    case "FOLLOW":
                        Console.WriteLine("Follow Mode Speech Recognized");
                        AsimovLog.WriteLine("Follow Mode Speech Recognized");

                        // Verify we're not already in another mode
                        if (AsimovMode.None == modeController.CurrentMode)
                        {
                            // Set the mode to follow
                            modeController.CurrentMode = AsimovMode.Follow;
                            ConfirmModeChange();
                            AsimovLog.WriteLine("Mode set to follow.");
                        }

                        break;
                    case "AVOID":
                        Console.WriteLine("Avoid Mode Speech Recognized");
                        AsimovLog.WriteLine("Avoid Mode Speech Recognized");

                        // Verify we're not already in another mode
                        if (AsimovMode.None == modeController.CurrentMode)
                        {
                            // Set the mode to avoid
                            modeController.CurrentMode = AsimovMode.Avoid;
                            ConfirmModeChange();
                            AsimovLog.WriteLine("Mode set to avoid.");
                        }

                        break;

                    case "CENTER":
                        Console.WriteLine("Center Mode Speech Recognized");
                        AsimovLog.WriteLine("Center Mode Speech Recognized");

                        // Verify we're not already in another mode
                        if (AsimovMode.None == modeController.CurrentMode)
                        {
                            // Set the mode to center mode
                            modeController.CurrentMode = AsimovMode.Center;
                            ConfirmModeChange();
                            AsimovLog.WriteLine("Mode set to center.");
                        }

                        break;
                    case "DRINK":
                        Console.WriteLine("Drinking Mode Speech Recognized");
                        AsimovLog.WriteLine("Drinking Mode Speech Recognized");

                        // Verify we're not already in another mode
                        if (AsimovMode.None == modeController.CurrentMode)
                        {
                            // Set the mode to drinking mode
                            modeController.CurrentMode = AsimovMode.Drinking;
                            ConfirmModeChange();
                            AsimovLog.WriteLine("Mode set to drinking mode.");
                        }

                        break;
                    case "EXIT":
                        Console.WriteLine("Exit Mode Speech Recognized");
                        AsimovLog.WriteLine("Exit Mode Speech Recognized");

                        // Verify we're in a mode
                        if (AsimovMode.None != modeController.CurrentMode)
                        {
                            // Set the mode to none in order to exit the current mode
                            modeController.CurrentMode = AsimovMode.None;
                            ConfirmModeChange();
                            AsimovLog.WriteLine("Mode set to none.");
                        }

                        break;
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
                kinect.ElevationAngle = 20;

                // Start speech recognition
                RecognizerInfo ri = GetKinectRecognizer();
                StartSpeechRecognition(ri);
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
            Console.WriteLine("Drinking Mode (BothArmsOut) Gesture Recognized");
            AsimovLog.WriteLine("Drinking Mode (BothArmsOut) Gesture Recognized");

            // Verify we're not already in another mode
            if (AsimovMode.None == modeController.CurrentMode)
            {
                // Set the mode to drinking mode
                modeController.CurrentMode = AsimovMode.Drinking;
                ConfirmModeChange();
                AsimovLog.WriteLine("Mode set to drinking mode.");
            }
        }

        private static void OnBothArmsUp(object sender, EventArgs e)
        {
            // Exit Mode
            Console.WriteLine("Exit Mode (BothArmsUp) Gesture Recognized");
            AsimovLog.WriteLine("Exit Mode (BothArmsUp) Gesture Recognized");

            // Verify we're in a mode
            if (AsimovMode.None != modeController.CurrentMode)
            {
                // Set the mode to none in order to exit the current mode
                modeController.CurrentMode = AsimovMode.None;
                ConfirmModeChange();
                AsimovLog.WriteLine("Mode set to none.");
            }
        }

        private static void OnBothElbowsBentUp(object sender, EventArgs e)
        {
            // Follow Mode
            Console.WriteLine("Follow Mode (BothElbowsBentUp) Gesture Recognized");
            AsimovLog.WriteLine("Follow Mode (BothElbowsBentUp) Gesture Recognized");

            // Verify we're not already in another mode
            if (AsimovMode.None == modeController.CurrentMode)
            {
                // Set the mode to follow
                modeController.CurrentMode = AsimovMode.Follow;
                ConfirmModeChange();
                AsimovLog.WriteLine("Mode set to follow.");
            }
        }

        private static void OnLeftArmBentDownRightArmDown(object sender, EventArgs e)
        {
            // Turn Around
            Console.WriteLine("Turn Around (LeftArmBentDownRightArmDown) Gesture Recognized");
            AsimovLog.WriteLine("Turn Around (LeftArmBentDownRightArmDown) Gesture Recognized");

            // Verify we're not in a mode
            if (AsimovMode.None == modeController.CurrentMode)
            {
                // Turn the Create 180 degrees.
                roomba.SpinAngle(Constants.DefaultVelocity, 180);
                lastActionTime = DateTime.Now;
                AsimovLog.WriteLine("Turned the Create clockwise 180 degrees.");
            }
        }

        private static void OnLeftArmDownRightArmBentDown(object sender, EventArgs e)
        {
            // Move Backward
            Console.WriteLine("Move Backward (LeftArmDownRightArmBentDown) Gesture Recognized");
            AsimovLog.WriteLine("Move Backward (LeftArmDownRightArmBentDown) Gesture Recognized");

            // Verify we're not in a mode
            if (AsimovMode.None == modeController.CurrentMode)
            {
                // Drive the Create backward a finite distance
                roomba.DriveDistance(-Constants.DefaultVelocity, Constants.DefaultDriveStep);
                lastActionTime = DateTime.Now;
                AsimovLog.WriteLine("Drove the Create backward 0.5 m.");
            }
        }

        private static void OnLeftArmDownRightArmBentUp(object sender, EventArgs e)
        {
            // Move Forward
            Console.WriteLine("Move Forward (LeftArmDownRightArmBentUp) Gesture Recognized");
            AsimovLog.WriteLine("Move Forward (LeftArmDownRightArmBentUp) Gesture Recognized");
            
            // Verify we're not in a mode
            if (AsimovMode.None == modeController.CurrentMode)
            {
                // Drive the Create forward a finite distance
                roomba.DriveDistance(Constants.DefaultVelocity, Constants.DefaultDriveStep);
                lastActionTime = DateTime.Now;
                AsimovLog.WriteLine("Drove the Create forward 0.5 m.");
            }
        }

        private static void OnLeftArmDownRightArmOut(object sender, EventArgs e)
        {
            // Turn Left (Counterclockwise)
            Console.WriteLine("Turn Left (LeftArmDownRightArmOut) Gesture Recognized");
            AsimovLog.WriteLine("Turn Left (LeftArmDownRightArmOut) Gesture Recognized");

            // Verify we're not in a mode
            if (AsimovMode.None == modeController.CurrentMode)
            {
                // Turn the Create counterclockwise a finite number of degrees
                roomba.SpinAngle(-Constants.DefaultVelocity, Constants.DefaultSpinStep);
                lastActionTime = DateTime.Now;
                AsimovLog.WriteLine("Turned the Create counterclockwise 30 degrees.");
            }
        }

        private static void OnLeftArmOutRightArmDown(object sender, EventArgs e)
        {
            // Turn Right (Clockwise)
            Console.WriteLine("Turn Right (LeftArmOutRightArmDown) Gesture Recognized");
            AsimovLog.WriteLine("Turn Right (LeftArmOutRightArmDown) Gesture Recognized");

            // Verify we're not in a mode
            if (AsimovMode.None == modeController.CurrentMode)
            {
                // Turn the Create clockwise a finite number of degrees
                roomba.SpinAngle(Constants.DefaultVelocity, Constants.DefaultSpinStep);
                lastActionTime = DateTime.Now;
                AsimovLog.WriteLine("Turned the Create clockwise 30 degrees.");
            }
        }

        private static void OnLeftArmOutRightArmUp(object sender, EventArgs e)
        {
            // Center Mode
            Console.WriteLine("Center Mode (LeftArmOutRightArmUp) Gesture Recognized");
            AsimovLog.WriteLine("Center Mode (LeftArmOutRightArmUp) Gesture Recognized");

            // Verify we're not already in another mode
            if (AsimovMode.None == modeController.CurrentMode)
            {
                // Set the mode to center mode
                modeController.CurrentMode = AsimovMode.Center;
                ConfirmModeChange();
                AsimovLog.WriteLine("Mode set to center.");
            }
        }

        private static void OnLeftArmUpRightArmOut(object sender, EventArgs e)
        {
            // Avoid Mode
            Console.WriteLine("Avoid Mode (LeftArmUpRightArmOut) Gesture Recognized");
            AsimovLog.WriteLine("Avoid Mode (LeftArmUpRightArmOut) Gesture Recognized");

            // Verify we're not already in another mode
            if (AsimovMode.None == modeController.CurrentMode)
            {
                // Set the mode to avoid
                modeController.CurrentMode = AsimovMode.Avoid;
                ConfirmModeChange();
                AsimovLog.WriteLine("Mode set to avoid.");
            }
        }

        private static void ConfirmModeChange()
        {
            //roomba.FlashLed(Led.Advance, 2, 500);
            roomba.Beep();
            roomba.Beep();
        }
    }
}
