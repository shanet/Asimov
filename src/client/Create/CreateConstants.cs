//------------------------------------------------------------------------------
// <copyright file="CreateConstants.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Create
{
    public static class CreateConstants
    {
        /// <summary>
        /// Minimum time in milliseconds.
        /// </summary>
        public const double TimeMin = 0.0;

        /// <summary>
        /// Minimum velocity in meters/second.
        /// </summary>
        public const double VelocityMin = -0.5;

        /// <summary>
        /// Maximum velocity in meters/second.
        /// </summary>
        public const double VelocityMax = 0.5;

        /// <summary>
        /// Minimum turning radius in meters.
        /// </summary>
        public const double RadiusMin = -2.0;

        /// <summary>
        /// Maximum turning radius in meters.
        /// </summary>
        public const double RadiusMax = 2.0;

        /// <summary>
        /// Minimum drivable distance in meters.
        /// </summary>
        public const double DistanceMin = -2.0;

        /// <summary>
        /// Maximum drivable distance in meters.
        /// </summary>
        public const double DistanceMax = 2.0;

        /// <summary>
        /// Minimum Power LED color value.
        /// </summary>
        public const int PowerLedColorMin = 0;

        /// <summary>
        /// Maximum Power LED color value.
        /// </summary>
        public const int PowerLedColorMax = 255;

        /// <summary>
        /// Minimum Power LED intensity value.
        /// </summary>
        public const int PowerLedIntensityMin = 0;

        /// <summary>
        /// Maximum Power LED intensity value.
        /// </summary>
        public const int PowerLedIntensityMax = 255;
    }
}
