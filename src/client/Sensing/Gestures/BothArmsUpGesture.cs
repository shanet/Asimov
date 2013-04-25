//------------------------------------------------------------------------------
// <copyright file="BothArmsUpGesture.cs" company="Aaron Goodermuth">
//     Copyright (c) Aaron Goodermuth.  All rights reserved.
// </copyright>
// <summary>
//     Class that recognizes a gesture in which both arms of a skeleton are
//     straight up in the air.
// </summary>
//------------------------------------------------------------------------------

namespace AsimovClient.Sensing.Gestures
{
    using System;

    using Helpers;

    using Microsoft.Kinect;

    public class BothArmsUpGesture : IGesture
    {
        private const float DesiredJointAngle = 90;
        private const float JointToleranceAngle = 30;
        private const float DesiredPlaneAngle = 0;
        private const float PlaneToleranceAngle = 45;

        private DateTime lastEventFireTime;

        public BothArmsUpGesture()
        {
            this.lastEventFireTime = DateTime.MinValue;
        }

        public event EventHandler BothArmsUpRecognized;

        public void UpdateGesture(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                if (this.CheckGesture(skeleton) && DateTime.Now.Subtract(this.lastEventFireTime) >= Constants.GestureWaitTime)
                {
                    this.lastEventFireTime = DateTime.Now;
                    this.BothArmsUpRecognized(this, null);
                }
            }
        }

        private bool CheckGesture(Skeleton skeleton)
        {
            double[] angles = new double[4];
            bool isValid = false;

            if (skeleton != null)
            {
                // Check if the skeleton's arms are extended up in the air
                angles[0] = SkeletonHelper.CalculateAngleXY(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                angles[1] = SkeletonHelper.CalculateAngleXY(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
                angles[2] = SkeletonHelper.CalculateAngleXY(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
                angles[3] = SkeletonHelper.CalculateAngleXY(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);

                isValid = MathHelper.AreEqualWithinTolerance(angles[0], DesiredJointAngle, JointToleranceAngle)
                          && MathHelper.AreEqualWithinTolerance(angles[1], DesiredJointAngle, JointToleranceAngle)
                          && MathHelper.AreEqualWithinTolerance(angles[2], DesiredJointAngle, JointToleranceAngle)
                          && MathHelper.AreEqualWithinTolerance(angles[3], DesiredJointAngle, JointToleranceAngle);

                // Check if the skeleton's arms are in the XY plane
                angles[0] = SkeletonHelper.CalculateAngleYZ(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                angles[1] = SkeletonHelper.CalculateAngleYZ(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
                angles[2] = SkeletonHelper.CalculateAngleYZ(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
                angles[3] = SkeletonHelper.CalculateAngleYZ(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);

                isValid = isValid
                          && MathHelper.AreEqualWithinAngularTolerance(angles[0], DesiredPlaneAngle, PlaneToleranceAngle)
                          && MathHelper.AreEqualWithinAngularTolerance(angles[1], DesiredPlaneAngle, PlaneToleranceAngle)
                          && MathHelper.AreEqualWithinAngularTolerance(angles[2], DesiredPlaneAngle, PlaneToleranceAngle)
                          && MathHelper.AreEqualWithinAngularTolerance(angles[3], DesiredPlaneAngle, PlaneToleranceAngle);
            }

            return isValid;
        }
    }
}