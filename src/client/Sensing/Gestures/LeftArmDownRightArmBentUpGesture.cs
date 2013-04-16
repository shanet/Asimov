//------------------------------------------------------------------------------
// <copyright file="LeftArmDownRightArmBentUpGesture.cs" company="Aaron Goodermuth">
//     Copyright (c) Aaron Goodermuth.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Sensing.Gestures
{
    using System;

    using Helpers;

    using Microsoft.Kinect;

    public class LeftArmDownRightArmBentUpGesture : IGesture
    {
        private const float DesiredLeftJointAngle = -90;
        private const float DesiredRightShoulderJointAngle = 0;
        private const float DesiredRightElbowJointAngle = 90;
        private const float JointToleranceAngle = 30;

        private const float DesiredLeftPlaneAngle = -180;
        private const float DesiredRightPlaneAngle = 0;
        private const float PlaneToleranceAngle = 45;

        private DateTime lastEventFireTime;

        public LeftArmDownRightArmBentUpGesture()
        {
            this.lastEventFireTime = DateTime.MinValue;
        }

        public event EventHandler LeftArmDownRightArmBentUpRecognized;

        public void UpdateGesture(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                if (this.CheckGesture(skeleton) && DateTime.Now.Subtract(this.lastEventFireTime) >= Constants.GestureWaitTime)
                {
                    this.lastEventFireTime = DateTime.Now;
                    this.LeftArmDownRightArmBentUpRecognized(this, null);
                }
            }
        }

        private bool CheckGesture(Skeleton skeleton)
        {
            double[] angles = new double[4];
            bool isValid = false;

            if (skeleton != null)
            {
                // Check if the skeleton's arms are extended sideways in the air
                angles[0] = SkeletonHelper.CalculateAngleXY(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                angles[1] = SkeletonHelper.CalculateAngleXY(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
                angles[2] = SkeletonHelper.CalculateAngleXY(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
                angles[3] = SkeletonHelper.CalculateAngleXY(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);

                isValid = MathHelper.AreEqualWithinTolerance(angles[0], DesiredLeftJointAngle, JointToleranceAngle)
                          && MathHelper.AreEqualWithinTolerance(angles[1], DesiredLeftJointAngle, JointToleranceAngle)
                          && MathHelper.AreEqualWithinTolerance(angles[2], DesiredRightElbowJointAngle, JointToleranceAngle)
                          && MathHelper.AreEqualWithinTolerance(angles[3], DesiredRightShoulderJointAngle, JointToleranceAngle);

                // Check if the skeleton's arms are in the YZ plane
                angles[0] = SkeletonHelper.CalculateAngleYZ(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                angles[1] = SkeletonHelper.CalculateAngleYZ(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
                angles[2] = SkeletonHelper.CalculateAngleYZ(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);

                isValid = isValid
                          && MathHelper.AreEqualWithinAngularTolerance(angles[0], DesiredLeftPlaneAngle, PlaneToleranceAngle)
                          && MathHelper.AreEqualWithinAngularTolerance(angles[1], DesiredLeftPlaneAngle, PlaneToleranceAngle)
                          && MathHelper.AreEqualWithinAngularTolerance(angles[2], DesiredRightPlaneAngle, PlaneToleranceAngle);
            }

            return isValid;
        }
    }
}