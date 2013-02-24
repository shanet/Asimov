//------------------------------------------------------------------------------
// <copyright file="Units.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Helpers
{
    using System;

    public static class Units
    {
        public static double BaseToMilli(double value)
        {
            return value * 1000;
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }
    }
}
