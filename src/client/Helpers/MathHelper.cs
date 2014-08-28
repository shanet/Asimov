﻿//------------------------------------------------------------------------------
// <copyright file="MathHelper.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
// <summary>
//     Static class that contains helper function to perform common math
//     operations, such as checking equality within a tolerance.
// </summary>
//------------------------------------------------------------------------------

namespace AsimovClient.Helpers
{
    using System;

    public static class MathHelper
    {
        public static bool AreEqualWithinTolerance(double value1, double value2, double tolerance)
        {
            return Math.Abs(value1 - value2) <= tolerance;
        }

        public static bool AreEqualWithinAngularTolerance(double value1, double value2, double tolerance)
        {
            return Math.Abs(value1 - value2) % 180 <= tolerance;
        }
    }
}
