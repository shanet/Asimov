//------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient
{
    using System;

    using Create;

    public static class Constants
    {
        /// <summary>
        /// The angle in degrees around the center within which a person is considered centered.
        /// </summary>
        public const double CenteredTolerance = 3.0;

        /// <summary>
        /// Default number of degrees to spin for gesture and voice commands.
        /// </summary>
        public const int DefaultSpinStep = 30;

        /// <summary>
        /// Default number of meters to drive for gesture and voice commands.
        /// </summary>
        public const double DefaultDriveStep = 0.5;

        /// <summary>
        /// Default velocity in meters/second for gesture and voice commands.
        /// </summary>
        public const double DefaultVelocity = CreateConstants.VelocityMax / 2;

        /// <summary>
        /// The amount of time to wait before firing another gesture event of the same type.
        /// </summary>
        public static readonly TimeSpan GestureWaitTime = new TimeSpan(0, 0, 0, 1);
    }
}
