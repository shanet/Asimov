//------------------------------------------------------------------------------
// <copyright file="ModeController.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Modes
{
    using System;

    using Create;
    using Helpers;

    using Microsoft.Kinect;

    public class ModeController
    {
        private ICreateController create;

        private DateTime lastActionTime;

        public ModeController(ICreateController roomba)
        {
            this.CurrentMode = AsimovMode.None;
            this.create = roomba;
            this.lastActionTime = DateTime.MinValue;
        }

        public AsimovMode CurrentMode { get; set; }

        public void UpdateSkeleton(Skeleton skeleton)
        {
            switch (this.CurrentMode)
            {
                case AsimovMode.Follow:
                    this.FollowSkeleton(skeleton);
                    break;
                case AsimovMode.Avoid:
                    this.AvoidSkeleton(skeleton);
                    break;
                case AsimovMode.Drinking:
                    this.DrinkingMode(skeleton);
                    break;
                case AsimovMode.Center:
                    this.CenterSkeleton(skeleton);
                    break;
            }
        }

        public bool IsInMode()
        {
            return AsimovMode.None != this.CurrentMode;
        }

        private void FollowSkeleton(Skeleton skeleton)
        {
            double distanceFromSkeleton = skeleton.Position.Z;

            // Center the skeleton
            this.CenterSkeleton(skeleton);

            if (DateTime.Now.Subtract(this.lastActionTime) > Constants.ActionWaitTime)
            {
                if (Constants.DesiredDistanceFromSkelton + Constants.DistanceFromSkeletonTolerance < distanceFromSkeleton)
                {
                    // Drive toward the skeleton one step
                    this.create.DriveDistance(Constants.DefaultVelocity, Math.Min(distanceFromSkeleton - Constants.DesiredDistanceFromSkelton, Constants.DefaultDriveStep));
                    this.lastActionTime = DateTime.Now;
                }
                else if (Constants.DesiredDistanceFromSkelton - Constants.DistanceFromSkeletonTolerance > distanceFromSkeleton)
                {
                    // Drive away from the skeleton one step
                    this.create.DriveDistance(-Constants.DefaultVelocity, Math.Min(Constants.DesiredDistanceFromSkelton - distanceFromSkeleton, Constants.DefaultDriveStep));
                    this.lastActionTime = DateTime.Now;
                }
            }
        }

        private void AvoidSkeleton(Skeleton skeleton)
        {
            double angle = Units.RadiansToDegrees(Math.Atan2(skeleton.Position.X, skeleton.Position.Z));

            // Turn away from the skeleton
            if (DateTime.Now.Subtract(this.lastActionTime) > Constants.ActionWaitTime)
            {
                this.create.SpinAngle(Math.Sign(angle) * Constants.DefaultVelocity, 180 - (int)Math.Abs(angle));
                this.lastActionTime = DateTime.Now;
            }
        }

        private void DrinkingMode(Skeleton skeleton)
        {
            //TODO
        }

        private void CenterSkeleton(Skeleton skeleton)
        {
            double angle = Units.RadiansToDegrees(Math.Atan2(skeleton.Position.X, skeleton.Position.Z));

            // Determine if we need to center the skeleton we see
            if (angle < -Constants.CenteredTolerance || angle > Constants.CenteredTolerance)
            {
                if (DateTime.Now.Subtract(this.lastActionTime) > Constants.CenterWaitTime)
                {
                    this.create.SpinAngle(-Math.Sign(angle) * Constants.DefaultVelocity, (int)angle / 2);
                    this.lastActionTime = DateTime.Now;
                }
            }
        }
    }
}
