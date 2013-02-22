//------------------------------------------------------------------------------
// <copyright file="ICreateController.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Create
{
    public enum CreateMode
    {
        /// <summary>
        /// In Passive mode, you can read sensors, perform built-in demos, and charge the battery.
        /// </summary>
        Passive,

        /// <summary>
        /// Safe mode enables automatic reaction to cliff sensors, wheel-drop sensors, and charging sensors.
        /// </summary>
        Safe,

        /// <summary>
        /// Full mode gives you complete control over Create.
        /// Full mode shuts off the cliff, wheel-drop, and internal charger safety features.
        /// </summary>
        Full
    }

    public enum Led
    {
        /// <summary>
        /// The Power LED
        /// </summary>
        Power,

        /// <summary>
        /// The Advance LED
        /// </summary>
        Advance,

        /// <summary>
        /// The Play LED
        /// </summary>
        Play
    }

    public interface ICreateController
    {
        #region Core
        /// <summary>
        /// Turns on the Create and puts it in safe mode.
        /// </summary>
        void PowerOn();

        /// <summary>
        /// Turns off the Create.
        /// </summary>
        void PowerOff();

        /// <summary>
        /// Sets the mode of the Create to <paramref name="mode"/>.
        /// </summary>
        /// <param name="mode">Mode in which to put the Create.</param>
        void SetMode(CreateMode mode);
        #endregion

        #region Drive
        /// <summary>
        /// Drive the Create at the given velocity and radius until another drive function is called.
        /// </summary>
        /// <param name="velocity">Velocity in millimeters/millisecond at which to drive.</param>
        /// <param name="radius">Radius in degrees at which to drive.</param>
        void Drive(int velocity, int radius);

        /// <summary>
        /// Drive the Create in a straight line at the given velocity until another drive function is called.
        /// </summary>
        /// <param name="velocity">Velocity at which to drive in millimeters/millisecond.</param>
        void Drive(int velocity);

        /// <summary>
        /// Drive the Create at the given velocity and radius for the given distance in millimeters.
        /// </summary>
        /// <param name="velocity">Velocity at which to drive in millimeters/millisecond.</param>
        /// <param name="radius">Radius at which to drive in degrees.</param>
        /// <param name="distance">Distance to drive in millimeters.</param>
        void DriveDistance(int velocity, int radius, int distance);

        /// <summary>
        /// Drive the Create in a straight line at the given velocity for the given distance in millimeters.
        /// </summary>
        /// <param name="velocity">Velocity at which to drive in millimeters/millisecond.</param>
        /// <param name="distance">Distance to drive in millimeters.</param>
        void DriveDistance(int velocity, int distance);

        /// <summary>
        /// Drive the Create at the given velocity and radius for the given time in milliseconds.
        /// </summary>
        /// <param name="velocity">Velocity at which to drive in millimeters/millisecond.</param>
        /// <param name="radius">Radius at which to drive in degrees.</param>
        /// <param name="time">Time to drive in milliseconds.</param>
        void DriveTime(int velocity, int radius, int time);

        /// <summary>
        /// Drive the Create in a straight line at the given velocity for the given time in milliseconds.
        /// </summary>
        /// <param name="velocity">Velocity at which to drive in millimeters/millisecond.</param>
        /// <param name="time">Time to drive in milliseconds.</param>
        void DriveTime(int velocity, int time);

        /// <summary>
        /// Drive the left and right wheels of the Create individually at the given right and left velocity.
        /// </summary>
        /// <param name="leftVelocity">Velocity at which to drive the left wheel in millimeters/millisecond.</param>
        /// <param name="rightVelocity">Velocity at which to drive the right wheel in millimeters/millisecond.</param>
        void DriveDirect(int leftVelocity, int rightVelocity);

        /// <summary>
        /// Turn the Create in place at the given velocity and radius (either clockwise or counter-clockwise)
        /// for the given number of degrees.
        /// </summary>
        /// <param name="velocity">Velocity at which to turn in millimeters/millisecond.</param>
        /// <param name="radius">Radius at which to turn in degrees.</param>
        /// <param name="degrees">Degrees to turn.</param>
        void Turn(int velocity, int radius, int degrees);

        /// <summary>
        /// Stop the Create.
        /// </summary>
        void Stop();
        #endregion

        #region LED
        /// <summary>
        /// Turn on or off the LED given by <paramref name="led"/>.
        /// </summary>
        /// <param name="led">The LED to turn off or on.</param>
        /// <param name="onOff">True to turn on, false to turn off.</param>
        void SetLed(Led led, bool onOff);

        /// <summary>
        /// Sets the Power LED to the given color and intensity.
        /// </summary>
        /// <param name="color">Value between 0 and 255, with 0 being green and 255 being red.</param>
        /// <param name="intensity">Value between 0 and 255, with 0 being off and 255 being full intensity.</param>
        void SetPowerLed(int color, int intensity);

        /// <summary>
        /// Flash the given LED the given number of times with a given flash duration.
        /// </summary>
        /// <param name="led">The LED to flash.</param>
        /// <param name="flashCount">The number of times to flash the LED.</param>
        /// <param name="flashDuration">The duration of each flash.</param>
        void FlashLed(Led led, int flashCount, int flashDuration);
        #endregion

        #region Song

        /// <summary>
        /// Emit a simple beep from the Create.
        /// </summary>
        void Beep();

        /// <summary>
        /// Define a song with the given song number.
        /// Notes of the song are specified in a parallel array with the note durations.
        /// A song may be a maximum of 16 notes.
        /// </summary>
        /// <param name="songNumber">Numeric identifier to give the song between 0 and 255.</param>
        /// <param name="notes">Up to 16 notes that make up the song, each between 0 and 255.</param>
        /// <param name="durations">Up to 16 durations (one for each note of the song), each between 0 and 255.</param>
        void DefineSong(int songNumber, int[] notes, int[] durations);

        /// <summary>
        /// Plays the song with the given song number.
        /// </summary>
        /// <param name="songNumber">Numeric identifier of the song to play between 0 and 255.</param>
        void PlaySong(int songNumber);

        #endregion
    }
}
