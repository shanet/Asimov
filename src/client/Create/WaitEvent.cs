//------------------------------------------------------------------------------
// <copyright file="Event.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Create
{
    public enum WaitEvent
    {
        /// <summary>
        /// Any wheel drops.
        /// </summary>
        WheelDrop,

        /// <summary>
        /// The front wheel drops.
        /// </summary>
        FrontWheelDrop,

        /// <summary>
        /// The left wheel drops.
        /// </summary>
        LeftWheelDrop,

        /// <summary>
        /// The right wheel drops.
        /// </summary>
        RightWheelDrop,

        /// <summary>
        /// The bump sensor is activated.
        /// </summary>
        Bump,

        /// <summary>
        /// The left bump sensor is activated.
        /// </summary>
        LeftBump,

        /// <summary>
        /// The right bump sensor is activated.
        /// </summary>
        RightBump,

        /// <summary>
        /// A virtual wall is encountered.
        /// </summary>
        VirtualWall,

        /// <summary>
        /// Any wall is encountered.
        /// </summary>
        Wall,

        /// <summary>
        /// Any cliff sensor is activated.
        /// </summary>
        Cliff,

        /// <summary>
        /// The left cliff sensor is activated.
        /// </summary>
        LeftCliff,

        /// <summary>
        /// The front left cliff sensor is activated.
        /// </summary>
        FrontLeftCliff,

        /// <summary>
        /// The front right cliff sensor is activated.
        /// </summary>
        FrontRightCliff,

        /// <summary>
        /// The right cliff sensor is activated.
        /// </summary>
        RightCliff,

        /// <summary>
        /// The Create is in its home base.
        /// </summary>
        HomeBase,

        /// <summary>
        /// The advance button is pressed.
        /// </summary>
        AdvanceButton,

        /// <summary>
        /// The play button is pressed.
        /// </summary>
        PlayButton,

        /// <summary>
        /// Digital input 0.
        /// </summary>
        DigitalInput0,

        /// <summary>
        /// The digital input 1.
        /// </summary>
        DigitalInput1,

        /// <summary>
        /// Digital input 2.
        /// </summary>
        DigitalInput2,

        /// <summary>
        /// Digital input 3.
        /// </summary>
        DigitalInput3,

        /// <summary>
        /// Passive mode is activated.
        /// </summary>
        PassiveMode
    }
}
