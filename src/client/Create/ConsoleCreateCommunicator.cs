//------------------------------------------------------------------------------
// <copyright file="ConsoleCreateCommunicator.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Create
{
    using System;

    public class ConsoleCreateCommunicator : ICreateCommunicator
    {
        public bool ExecuteCommand(string commandFormat, params object[] args)
        {
            Console.WriteLine(commandFormat, args);

            return true;
        }

        public void Dispose()
        {
            // Not needed
        }
    }
}
