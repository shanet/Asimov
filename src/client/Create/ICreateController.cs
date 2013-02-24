//------------------------------------------------------------------------------
// <copyright file="ICreateController.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Create
{
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
        /// <param name="velocity">Velocity at which to drive in meters/second. [-0.5, 0.5]</param>
        /// <param name="radius">Radius at which to drive in meters. [-2.0, 2.0]</param>
        void Drive(double velocity, double radius);

        /// <summary>
        /// Drive the Create in a straight line at the given velocity until another drive function is called.
        /// </summary>
        /// <param name="velocity">Velocity at which to drive in meters/second. [-0.5, 0.5]</param>
        void Drive(double velocity);

        /// <summary>
        /// Drive the Create at the given velocity and radius for the given distance.
        /// </summary>
        /// <param name="velocity">Velocity at which to drive in meters/second. [-0.5, 0.5]</param>
        /// <param name="radius">Radius at which to drive in meters. [-2.0, 2.0]</param>
        /// <param name="distance">Distance to drive in meters. [-2.0, 2.0]</param>
        void DriveDistance(double velocity, double radius, double distance);

        /// <summary>
        /// Drive the Create in a straight line at the given velocity for the given distance.
        /// </summary>
        /// <param name="velocity">Velocity at which to drive in meters/second. [-0.5, 0.5]</param>
        /// <param name="distance">Distance to drive in meters. [-2.0, 2.0]</param>
        void DriveDistance(double velocity, double distance);

        /// <summary>
        /// Drive the Create at the given velocity and radius for the given time.
        /// </summary>
        /// <param name="velocity">Velocity at which to drive in meters/second. [-0.5, 0.5]</param>
        /// <param name="radius">Radius at which to drive in meters. [-2.0, 2.0]</param>
        /// <param name="time">Time to drive in seconds.</param>
        void DriveTime(double velocity, double radius, double time);

        /// <summary>
        /// Drive the Create in a straight line at the given velocity for the given time.
        /// </summary>
        /// <param name="velocity">Velocity at which to drive in meters/second. [-0.5, 0.5]</param>
        /// <param name="time">Time to drive in seconds.</param>
        void DriveTime(double velocity, double time);

        /// <summary>
        /// Drive the left and right wheels of the Create individually at the given right and left velocity.
        /// </summary>
        /// <param name="leftVelocity">Velocity at which to drive the left wheel in meters/second. [-0.5, 0.5]</param>
        /// <param name="rightVelocity">Velocity at which to drive the right wheel in meters/second. [-0.5, 0.5]</param>
        void DriveDirect(double leftVelocity, double rightVelocity);

        /// <summary>
        /// Spin the Create in place clockwise at the given velocity.
        /// </summary>
        /// <param name="velocity">Velocity at which to turn in meters/second (use a negative velocity to spin counterclockwise). [-0.5, 0.5]</param>
        void Spin(double velocity);

        /// <summary>
        /// Spin the Create in place clockwise at the given velocity for the given number of degrees.
        /// </summary>
        /// <param name="velocity">Velocity at which to turn in meters/second (use a negative velocity to spin counterclockwise). [-0.5, 0.5]</param>
        /// <param name="degrees">Degrees to turn.</param>
        void SpinDistance(double velocity, int degrees);

        /// <summary>
        /// Spin the Create in place clockwise at the given velocity for the given number of seconds.
        /// </summary>
        /// <param name="velocity">Velocity at which to turn in meters/second (use a negative velocity to spin counterclockwise). [-0.5, 0.5]</param>
        /// <param name="time">Time to spin in seconds.</param>
        void SpinTime(double velocity, double time);

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
