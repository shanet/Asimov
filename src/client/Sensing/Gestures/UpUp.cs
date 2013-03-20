using AsimovClient.Helpers;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsimovClient.Sensing.Gestures
{
    class UpUp : IGesture
    {
        public event EventHandler UpUpRecognized;
        private const float TOLERANCE_ANGLE = 30;
        

        public UpUp(string name)
        {
        }

        public void UpdateGesture(Skeleton skeleton)
        {
            if (skeleton != null)
            {

                bool recognized = true;

                // check gesture logic
                double[] angles = new double[4];

                angles[0] = CalcAngle(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                angles[1] = CalcAngle(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
                angles[2] = CalcAngle(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
                angles[3] = CalcAngle(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);

                recognized = recognized && WithinTolerance(angles[0], 90);
                recognized = recognized && WithinTolerance(angles[1], 90);
                recognized = recognized && WithinTolerance(angles[2], 90);
                recognized = recognized && WithinTolerance(angles[3], 90);


                if (recognized)
                {
                    this.UpUpRecognized(this, null);
                }
            }
        }


        private bool WithinTolerance(double found, double desired)
        {
            return Math.Abs(desired - found) < TOLERANCE_ANGLE;
        }

        private double CalcAngle(Joint j1, Joint j2)
        {
            double deltaX = j2.Position.X - j1.Position.X;
            double deltaY = j2.Position.Y - j1.Position.Y;

            double retval = Units.RadiansToDegrees( Math.Atan2(deltaY, deltaX) );
            return retval;
        }

        /*private bool CalcSamePlane(float joint1z, float joint2x)
        {
            return true; //todo
        }*/


    }
}
