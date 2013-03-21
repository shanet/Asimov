//------------------------------------------------------------------------------
// <copyright file="SkeletonHelper.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Helpers
{
    using System;
    using Microsoft.Kinect;

    public static class SkeletonHelper
    {
        /// <summary>
        /// Calculates the angle between the provided joints in the XY plane.
        /// </summary>
        /// <param name="joint1">The first joint.</param>
        /// <param name="joint2">The second joint.</param>
        /// <returns>The angle (in degrees) between the joints.</returns>
        public static double CalculateAngleXY(Joint joint1, Joint joint2)
        {
            double deltaX = joint2.Position.X - joint1.Position.X;
            double deltaY = joint2.Position.Y - joint1.Position.Y;
            
            return Units.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
        }
    }
}
