//------------------------------------------------------------------------------
// <copyright file="ModeController.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Modes
{
    using Microsoft.Kinect;

    public class ModeController
    {
        public ModeController()
        {
            this.CurrentMode = AsimovMode.None;
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
            }
        }

        private void FollowSkeleton(Skeleton skeleton)
        {
            //TODO
        }

        private void AvoidSkeleton(Skeleton skeleton)
        {
            //TODO
        }

        private void DrinkingMode(Skeleton skeleton)
        {
            //TODO
        }
    }
}
