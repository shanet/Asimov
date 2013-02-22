//------------------------------------------------------------------------------
// <copyright file="AsimovClient.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient
{
    using Create;
    using Logging;
    using System;

    public class AsimovClient
    {
        public static void Main(string[] args)
        {
            try
            {
                ICreateController roomba = new ConsoleCreateController();

                roomba.PowerOn();
                roomba.Drive(1, 2);
                roomba.PowerOff();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("FATAL ERROR: {0}", e);
                AsimovLog.WriteLine("FATAL ERROR: {0}", e);
            }
        }
    }
}
