//------------------------------------------------------------------------------
// <copyright file="AsimovMode.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
// <summary>
//     Enumeration for the different operating modes that Asimov can be in.
// </summary>
//------------------------------------------------------------------------------

namespace AsimovClient.Modes
{
    public enum AsimovMode
    {
        None,
        Follow,
        Avoid,
        Obstacle,
        Drinking,
        Center
    }
}
