//------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
// <summary>
//     Defines common constant and default values for Asimov.
// </summary>
//------------------------------------------------------------------------------

namespace AsimovClient
{
    using System;

    using Create;

    public static class Constants
    {
        /// <summary>
        /// Command used to exit the client cleanly
        /// </summary>
        public const string ExitCommand = "exit";

        /// <summary>
        /// Speech utterance confidence below which we treat speech as if it hadn't been heard
        /// </summary>
        public const double SpeechConfidenceThreshold = 0.3;

        /// <summary>
        /// The angle in degrees around the center within which a person is considered centered.
        /// </summary>
        public const double CenteredTolerance = 10.0;

        /// <summary>
        /// The desired number of meters to be from from the skeleton in follow mode.
        /// </summary>
        public const double DesiredDistanceFromSkelton = 1.5;

        /// <summary>
        /// The number of meters within DesiredDistanceFromSkeleton a skeleton can be without taking action
        /// </summary>
        public const double DistanceFromSkeletonTolerance = 0.25;

        /// <summary>
        /// Default number of degrees to spin for gesture and voice commands.
        /// </summary>
        public const int DefaultSpinStep = 30;

        /// <summary>
        /// Default number of meters to drive for gesture and voice commands.
        /// </summary>
        public const double DefaultDriveStep = 0.1;

        /// <summary>
        /// Default velocity in meters/second for gesture and voice commands.
        /// </summary>
        public const double DefaultVelocity = CreateConstants.VelocityMax / 2;

        /// <summary>
        /// The amount of time to wait before firing another gesture event of the same type.
        /// </summary>
        public static readonly TimeSpan GestureWaitTime = new TimeSpan(0, 0, 0, 1);

        /// <summary>
        /// The amount of time to wait before firing another action event.
        /// </summary>
        public static readonly TimeSpan ActionWaitTime = new TimeSpan(0, 0, 0, 0, 500);

        /// <summary>
        /// The amount of time to wait before firing another center command.
        /// </summary>
        public static readonly TimeSpan CenterWaitTime = new TimeSpan(0, 0, 0, 1);
    }
}
