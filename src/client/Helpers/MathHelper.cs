//------------------------------------------------------------------------------
// <copyright file="MathHelper.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
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
    }
}
