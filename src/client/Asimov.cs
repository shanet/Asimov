//------------------------------------------------------------------------------
// <copyright file="Asimov.cs" company="Gage Ames">
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

    using Create;
    using Logging;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;
    using Modes;
    using Sensing.Gestures;

    public class Asimov : IDisposable
    {
        private ICreateController create;

        private KinectSensor kinect;

        private ModeController modeController;

        private ICollection<IGesture> gestures;

        private SpeechRecognitionEngine speechEngine;

        private DateTime lastActionTime;

        public Asimov()
            : this(new AsimovController(new TcpCreateCommunicator()))
        { // Empty
        }

        public Asimov(ICreateController create)
            : this(create, null)
        {
            // Find the Kinect and subscribe to changes in the sensor
            KinectSensorChooser sensorChooser = new KinectSensorChooser();
            sensorChooser.KinectChanged += this.KinectSensorChanged;
            sensorChooser.Start();
        }

        public Asimov(ICreateController create, KinectSensor kinect)
        {
            this.create = create;
            this.kinect = kinect;

            this.modeController = new ModeController(this.create);
            this.gestures = this.InitGestures();
            this.lastActionTime = DateTime.MinValue;

            // Beep to indicate Asimov has started
            create.Beep();

            // Disable all safety features of the Create
            create.SetMode(CreateMode.Full);
        }

        public void KinectSensorChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.NewSensor != null)
            {
                this.kinect = args.NewSensor;

                // Add an event handler to be called whenever there is new color frame data
                this.kinect.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Turn on the skeleton stream to receive skeleton frames
                this.kinect.SkeletonStream.Enable();

                // Tilt the Kinect to allow for the best view of the skeletons
                this.kinect.ElevationAngle = 20;

                // Start speech recognition
                RecognizerInfo ri = this.GetKinectRecognizer();
                this.StartSpeechRecognition(ri);
            }

            if (args.OldSensor != null)
            {
                // Turn off the skelton sensor and unsubscribe from its events
                args.OldSensor.SkeletonFrameReady -= this.SensorSkeletonFrameReady;
                args.OldSensor.SkeletonStream.Disable();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void MoveForward()
        {
            // Verify we're not in a mode
            if (!this.modeController.IsInMode())
            {
                // Drive the Create forward a finite distance
                this.create.DriveDistance(Constants.DefaultVelocity, Constants.DefaultDriveStep);
                this.lastActionTime = DateTime.Now;
                AsimovLog.WriteLine("Drove the Create forward 0.5 m.");
            }
        }

        public void MoveBackward()
        {
            // Verify we're not in a mode
            if (!this.modeController.IsInMode())
            {
                // Drive the Create backward a finite distance
                this.create.DriveDistance(-Constants.DefaultVelocity, Constants.DefaultDriveStep);
                this.lastActionTime = DateTime.Now;
                AsimovLog.WriteLine("Drove the Create backward 0.5 m.");
            }
        }

        public void TurnLeft()
        {
            // Verify we're not in a mode
            if (!this.modeController.IsInMode())
            {
                // Turn the Create counterclockwise a finite number of degrees
                this.create.SpinAngle(-Constants.DefaultVelocity, Constants.DefaultSpinStep);
                this.lastActionTime = DateTime.Now;
                AsimovLog.WriteLine("Turned the Create counterclockwise 30 degrees.");
            }
        }

        public void TurnRight()
        {
            // Verify we're not in a mode
            if (!this.modeController.IsInMode())
            {
                // Turn the Create clockwise a finite number of degrees
                this.create.SpinAngle(Constants.DefaultVelocity, Constants.DefaultSpinStep);
                this.lastActionTime = DateTime.Now;
                AsimovLog.WriteLine("Turned the Create clockwise 30 degrees.");
            }
        }

        public void TurnAround()
        {
            // Verify we're not in a mode
            if (!this.modeController.IsInMode())
            {
                // Turn the Create 180 degrees.
                this.create.SpinAngle(Constants.DefaultVelocity, 180);
                this.lastActionTime = DateTime.Now;
                AsimovLog.WriteLine("Turned the Create clockwise 180 degrees.");
            }
        }

        public void EnterFollowMode()
        {
            // Verify we're not already in another mode
            if (!this.modeController.IsInMode())
            {
                // Set the mode to follow
                this.modeController.CurrentMode = AsimovMode.Follow;
                this.ConfirmModeChange();
                AsimovLog.WriteLine("Mode set to follow.");
            }
        }

        public void EnterCenterMode()
        {
            // Verify we're not already in another mode
            if (!this.modeController.IsInMode())
            {
                // Set the mode to center mode
                this.modeController.CurrentMode = AsimovMode.Center;
                this.ConfirmModeChange();
                AsimovLog.WriteLine("Mode set to center.");
            }
        }

        public void EnterAvoidMode()
        {
            // Verify we're not already in another mode
            if (!this.modeController.IsInMode())
            {
                // Set the mode to avoid
                this.modeController.CurrentMode = AsimovMode.Avoid;
                this.ConfirmModeChange();
                AsimovLog.WriteLine("Mode set to avoid.");
            }
        }

        public void EnterDrinkingMode()
        {
            // Verify we're not already in another mode
            if (!this.modeController.IsInMode())
            {
                // Set the mode to drinking mode
                this.modeController.CurrentMode = AsimovMode.Drinking;
                this.ConfirmModeChange();
                AsimovLog.WriteLine("Mode set to drinking mode.");
            }
        }

        public void ExitMode()
        {
            // Verify we're in a mode
            if (this.modeController.IsInMode())
            {
                // Set the mode to none in order to exit the current mode
                this.modeController.CurrentMode = AsimovMode.None;
                this.ConfirmModeChange();
                AsimovLog.WriteLine("Mode set to none.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.create != null)
                {
                    this.create.Dispose();
                    this.create = null;
                }

                if (this.kinect != null)
                {
                    this.kinect.Dispose();
                    this.kinect = null;
                }
            }
        }

        private void ConfirmModeChange()
        {
            this.create.Beep();
            this.create.Beep();
        }

        private ICollection<IGesture> InitGestures()
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
            armsUp.BothArmsUpRecognized += this.OnBothArmsUp;
            armsOut.BothArmsOutRecognized += this.OnBothArmsOut;
            elbowsBent.BothElbowsBentUpRecognized += this.OnBothElbowsBentUp;
            leftBentDown.LeftArmBentDownRightArmDownRecognized += this.OnLeftArmBentDownRightArmDown;
            rightBentDown.LeftArmDownRightArmBentDownRecognized += this.OnLeftArmDownRightArmBentDown;
            rightBentUp.LeftArmDownRightArmBentUpRecognized += this.OnLeftArmDownRightArmBentUp;
            rightOut.LeftArmDownRightArmOutRecognized += this.OnLeftArmDownRightArmOut;
            leftOut.LeftArmOutRightArmDownRecognized += this.OnLeftArmOutRightArmDown;
            leftOutRightUp.LeftArmOutRightArmUpRecognized += this.OnLeftArmOutRightArmUp;
            leftUpRightOut.LeftArmUpRightArmOutRecognized += this.OnLeftArmUpRightArmOut;

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

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
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
                        this.modeController.UpdateSkeleton(skeleton);

                        // Only send a new skeleton to gesture recognizers if we haven't acted recently
                        if (DateTime.Now.Subtract(this.lastActionTime) > Constants.ActionWaitTime)
                        {
                            // Send the new skeleton to gesture handlers
                            foreach (IGesture gesture in this.gestures)
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

        #region Speech Recognition

        private RecognizerInfo GetKinectRecognizer()
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

        private void StartSpeechRecognition(RecognizerInfo ri)
        {
            this.speechEngine = new SpeechRecognitionEngine(ri.Id);
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(File.ReadAllText("SpeechGrammar.xml"))))
            {
                var g = new Grammar(memoryStream);
                this.speechEngine.LoadGrammar(g);
            }

            this.speechEngine.SpeechRecognized += this.SpeechRecognized;
            this.speechEngine.SpeechRecognitionRejected += this.SpeechRejected;

            this.speechEngine.SetInputToAudioStream(
                 this.kinect.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            StringBuilder message = new StringBuilder("Speech input was rejected.");

            foreach (RecognizedPhrase phrase in e.Result.Alternates)
            {
                message.AppendFormat("\n  Rejected phrase: {0}\n  Confidence score: {1}\n  Grammar name:  {2}\n", phrase.Text, phrase.Confidence, phrase.Grammar.Name);
            }

            AsimovLog.WriteLine(message.ToString());

            this.create.Beep();
            this.create.Beep();
            this.create.Beep();
            this.create.Beep();
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= Constants.SpeechConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "FORWARD":
                        Console.WriteLine("Move Forward Speech Recognized");
                        AsimovLog.WriteLine("Move Forward Speech Recognized");

                        this.MoveForward();
                        break;
                    case "BACKWARD":
                        Console.WriteLine("Move Backward Speech Recognized");
                        AsimovLog.WriteLine("Move Backward Speech Recognized");

                        this.MoveBackward();
                        break;
                    case "LEFT":
                        Console.WriteLine("Turn Left Speech Recognized");
                        AsimovLog.WriteLine("Turn Left Speech Recognized");

                        this.TurnLeft();
                        break;
                    case "RIGHT":
                        Console.WriteLine("Turn Right Speech Recognized");
                        AsimovLog.WriteLine("Turn Right Speech Recognized");

                        this.TurnRight();
                        break;
                    case "TURNAROUND":
                        Console.WriteLine("Turn Around Speech Recognized");
                        AsimovLog.WriteLine("Turn Around Speech Recognized");

                        this.TurnAround();
                        break;
                    case "FOLLOW":
                        Console.WriteLine("Follow Mode Speech Recognized");
                        AsimovLog.WriteLine("Follow Mode Speech Recognized");

                        this.EnterFollowMode();
                        break;
                    case "AVOID":
                        Console.WriteLine("Avoid Mode Speech Recognized");
                        AsimovLog.WriteLine("Avoid Mode Speech Recognized");

                        this.EnterAvoidMode();
                        break;

                    case "CENTER":
                        Console.WriteLine("Center Mode Speech Recognized");
                        AsimovLog.WriteLine("Center Mode Speech Recognized");

                        this.EnterCenterMode();
                        break;
                    case "DRINK":
                        Console.WriteLine("Drinking Mode Speech Recognized");
                        AsimovLog.WriteLine("Drinking Mode Speech Recognized");

                        this.EnterDrinkingMode();
                        break;
                    case "EXIT":
                        Console.WriteLine("Exit Mode Speech Recognized");
                        AsimovLog.WriteLine("Exit Mode Speech Recognized");

                        this.ExitMode();
                        break;

                    case "SHUTDOWN":
                        Console.WriteLine("Shutdown Speech Recognized");
                        AsimovLog.WriteLine("Shutdown Speech Recognized");

                        for (int i = 0; i < 5; i++)
                        {
                            this.create.Beep();
                        }
                        AsimovClient.endEvent.Set();
                        break;
                }
            }
        }

        #endregion

        #region Gesture Recognition

        private void OnBothArmsOut(object sender, EventArgs e)
        {
            // Drinking Mode
            Console.WriteLine("Drinking Mode (BothArmsOut) Gesture Recognized");
            AsimovLog.WriteLine("Drinking Mode (BothArmsOut) Gesture Recognized");

            this.EnterDrinkingMode();
        }

        private void OnBothArmsUp(object sender, EventArgs e)
        {
            // Exit Mode
            Console.WriteLine("Exit Mode (BothArmsUp) Gesture Recognized");
            AsimovLog.WriteLine("Exit Mode (BothArmsUp) Gesture Recognized");

            this.ExitMode();
        }

        private void OnBothElbowsBentUp(object sender, EventArgs e)
        {
            // Follow Mode
            Console.WriteLine("Follow Mode (BothElbowsBentUp) Gesture Recognized");
            AsimovLog.WriteLine("Follow Mode (BothElbowsBentUp) Gesture Recognized");

            this.EnterFollowMode();
        }

        private void OnLeftArmBentDownRightArmDown(object sender, EventArgs e)
        {
            // Turn Around
            Console.WriteLine("Turn Around (LeftArmBentDownRightArmDown) Gesture Recognized");
            AsimovLog.WriteLine("Turn Around (LeftArmBentDownRightArmDown) Gesture Recognized");

            this.TurnAround();
        }

        private void OnLeftArmDownRightArmBentDown(object sender, EventArgs e)
        {
            // Move Backward
            Console.WriteLine("Move Backward (LeftArmDownRightArmBentDown) Gesture Recognized");
            AsimovLog.WriteLine("Move Backward (LeftArmDownRightArmBentDown) Gesture Recognized");

            this.MoveBackward();
        }

        private void OnLeftArmDownRightArmBentUp(object sender, EventArgs e)
        {
            // Move Forward
            Console.WriteLine("Move Forward (LeftArmDownRightArmBentUp) Gesture Recognized");
            AsimovLog.WriteLine("Move Forward (LeftArmDownRightArmBentUp) Gesture Recognized");

            this.MoveForward();
        }

        private void OnLeftArmDownRightArmOut(object sender, EventArgs e)
        {
            // Turn Left (Counterclockwise)
            Console.WriteLine("Turn Left (LeftArmDownRightArmOut) Gesture Recognized");
            AsimovLog.WriteLine("Turn Left (LeftArmDownRightArmOut) Gesture Recognized");

            this.TurnLeft();
        }

        private void OnLeftArmOutRightArmDown(object sender, EventArgs e)
        {
            // Turn Right (Clockwise)
            Console.WriteLine("Turn Right (LeftArmOutRightArmDown) Gesture Recognized");
            AsimovLog.WriteLine("Turn Right (LeftArmOutRightArmDown) Gesture Recognized");

            this.TurnRight();
        }

        private void OnLeftArmOutRightArmUp(object sender, EventArgs e)
        {
            // Center Mode
            Console.WriteLine("Center Mode (LeftArmOutRightArmUp) Gesture Recognized");
            AsimovLog.WriteLine("Center Mode (LeftArmOutRightArmUp) Gesture Recognized");

            this.EnterCenterMode();
        }

        private void OnLeftArmUpRightArmOut(object sender, EventArgs e)
        {
            // Avoid Mode
            Console.WriteLine("Avoid Mode (LeftArmUpRightArmOut) Gesture Recognized");
            AsimovLog.WriteLine("Avoid Mode (LeftArmUpRightArmOut) Gesture Recognized");

            this.EnterAvoidMode();
        }
        #endregion
    }
}
