//------------------------------------------------------------------------------
// <copyright file="Units.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
// <summary>
//     Static class that contains helper functions to perform unit-related
//     mathematics, such as converting radians to degrees.
// </summary>
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
