//------------------------------------------------------------------------------
// <copyright file="CreateMode.cs" company="Gage Ames">
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
}