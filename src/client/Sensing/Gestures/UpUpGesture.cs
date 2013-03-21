//------------------------------------------------------------------------------
// <copyright file="UpUpGesture.cs" company="Aaron Goodermuth">
//     Copyright (c) Aaron Goodermuth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Sensing.Gestures
{
    using System;

    using Helpers;

    using Microsoft.Kinect;

    public class UpUpGesture : IGesture
    {
        private const float DesiredJointAngle = 90;
        private const float ToleranceAngle = 30;

        public event EventHandler UpUpRecognized;

        public void UpdateGesture(Skeleton skeleton)
        {
            bool recognized;
            double[] angles = new double[4];

            if (skeleton != null)
            {
                // Check if the skeleton meets the requirements of the gesture (two arms straight up in the air)
                angles[0] = SkeletonHelper.CalculateAngleXY(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                angles[1] = SkeletonHelper.CalculateAngleXY(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
                angles[2] = SkeletonHelper.CalculateAngleXY(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
                angles[3] = SkeletonHelper.CalculateAngleXY(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);

                recognized = MathHelper.AreEqualWithinTolerance(angles[0], DesiredJointAngle, ToleranceAngle)
                             && MathHelper.AreEqualWithinTolerance(angles[1], DesiredJointAngle, ToleranceAngle)
                             && MathHelper.AreEqualWithinTolerance(angles[2], DesiredJointAngle, ToleranceAngle)
                             && MathHelper.AreEqualWithinTolerance(angles[3], DesiredJointAngle, ToleranceAngle);

                if (recognized)
                {
                    this.UpUpRecognized(this, null);
                }
            }
        }

        /*private bool CalcSamePlane(float joint1z, float joint2x)
        {
            return true; //TODO
        }*/
    }
}