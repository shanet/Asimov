//------------------------------------------------------------------------------
// <copyright file="BothElbowsBentUpGesture.cs" company="Aaron Goodermuth">
//     Copyright (c) Aaron Goodermuth.  All rights reserved.
// </copyright>
// <summary>
//     Class that recognizes a gesture in which both arms of a skeleton are
//     straight out to the side with its elbows bent up.
// </summary>
//------------------------------------------------------------------------------

namespace AsimovClient.Sensing.Gestures
{
    using System;

    using Helpers;

    using Microsoft.Kinect;

    public class BothElbowsBentUpGesture : IGesture
    {
        private const float DesiredLeftJointAngle = -180;
        private const float DesiredLeftElbowJointAngle = 90;
        private const float DesiredRightShoulderJointAngle = 0;
        private const float DesiredRightElbowJointAngle = 90;
        private const float JointToleranceAngle = 30;

        private const float DesiredLeftPlaneAngle = 0;
        private const float DesiredRightPlaneAngle = 0;
        private const float PlaneToleranceAngle = 45;

        private DateTime lastEventFireTime;

        public BothElbowsBentUpGesture()
        {
            this.lastEventFireTime = DateTime.MinValue;
        }

        public event EventHandler BothElbowsBentUpRecognized;

        public void UpdateGesture(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                if (this.CheckGesture(skeleton) && DateTime.Now.Subtract(this.lastEventFireTime) >= Constants.GestureWaitTime)
                {
                    this.lastEventFireTime = DateTime.Now;
                    this.BothElbowsBentUpRecognized(this, null);
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
                
                isValid = MathHelper.AreEqualWithinTolerance(angles[0], DesiredLeftElbowJointAngle, JointToleranceAngle)
                          && MathHelper.AreEqualWithinTolerance(angles[1], DesiredLeftJointAngle, JointToleranceAngle)
                          && MathHelper.AreEqualWithinTolerance(angles[2], DesiredRightElbowJointAngle, JointToleranceAngle)
                          && MathHelper.AreEqualWithinTolerance(angles[3], DesiredRightShoulderJointAngle, JointToleranceAngle);

                // Check if the skeleton's arms are in the XY plane
                angles[0] = SkeletonHelper.CalculateAngleYZ(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                angles[2] = SkeletonHelper.CalculateAngleYZ(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);

                isValid = isValid
                          && MathHelper.AreEqualWithinAngularTolerance(angles[0], DesiredLeftPlaneAngle, PlaneToleranceAngle)
                          && MathHelper.AreEqualWithinAngularTolerance(angles[2], DesiredRightPlaneAngle, PlaneToleranceAngle);
            }

            return isValid;
        }
    }
}