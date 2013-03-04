//------------------------------------------------------------------------------
// <copyright file="DriveProperties.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace ClientDebugger
{
    public class DriveProperties
    {
        public bool IsIndefinate { get; set; }

        public bool IsTime { get; set; }

        public bool IsDistance { get; set; }

        public bool IsAngle { get; set; }

        public bool IsRadius { get; set; }

        public bool IsStraight { get; set; }

        public bool IsSpin { get; set; }

        public double Velocity { get; set; }

        public double Time { get; set; }

        public double Distance { get; set; }

        public int Angle { get; set; }

        public double Radius { get; set; }
    }
}
