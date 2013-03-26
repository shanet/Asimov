//------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient
{
    using System;

    public static class Constants
    {
        /// <summary>
        /// The angle in degrees around the center within which a person is considered centered.
        /// </summary>
        public const double CenteredTolerance = 3.0;

        /// <summary>
        /// The amount of time to wait before firing another gesture event of the same type.
        /// </summary>
        public static readonly TimeSpan GestureWaitTime = new TimeSpan(0, 0, 0, 1);
    }
}
