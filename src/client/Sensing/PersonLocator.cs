//------------------------------------------------------------------------------
// <copyright file="PersonLocator.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Sensing
{
    using System;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;

    public class PersonLocator
    {
        public const double CenteredTolerance = 3.0;

        private KinectSensor kinect;

        private bool isPersonNotCenteredInvoked;

        public PersonLocator(KinectSensorChooser sensorChooser)
        {
            this.isPersonNotCenteredInvoked = false;

            // If the sensor changes, we need to set up the new sensor
            sensorChooser.KinectChanged += this.KinectSensorChanged;
        }

        public delegate void OnPersonCenteredHandler(object sender);

        public delegate void OnPersonNotCenteredHandler(object sender, double angle);

        public event OnPersonNotCenteredHandler OnPersonNotCentered;

        public event OnPersonCenteredHandler OnPersonCentered;

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
                        double angle = Math.Atan2(skeleton.Position.X, skeleton.Position.Z) * 180 / Math.PI;

                        // Determine if we need to center the skeleton we see
                        if (angle < -CenteredTolerance || angle > CenteredTolerance)
                        {
                            this.isPersonNotCenteredInvoked = true;
                            this.OnPersonNotCentered.Invoke(this, angle);
                        }
                        else if (this.isPersonNotCenteredInvoked == true)
                        {
                            this.OnPersonCentered.Invoke(this);
                            this.isPersonNotCenteredInvoked = false;
                        }
                    }
                    else if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        //TODO
                    }
                }
            }
        }

        private void KinectSensorChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.NewSensor != null)
            {
                this.kinect = args.NewSensor;

                // Add an event handler to be called whenever there is new color frame data
                this.kinect.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Turn on the skeleton stream to receive skeleton frames
                this.kinect.SkeletonStream.Enable();
            }

            if (args.OldSensor != null)
            {
                // Turn off the skelton sensor and unsubscribe from its events
                args.OldSensor.SkeletonFrameReady -= this.SensorSkeletonFrameReady;
                args.OldSensor.SkeletonStream.Disable();
            }
        }
    }
}
