﻿//------------------------------------------------------------------------------
// <copyright file="ICreateCommunicator.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
// <summary>
//     Interface outlining methods used to issue Asimov commands.
// </summary>
//------------------------------------------------------------------------------

namespace AsimovClient.Create
{
    using System;

    public interface ICreateCommunicator : IDisposable
    {
        /// <summary>
        /// Executes the given command using the classes communication method.
        /// </summary>
        /// <param name="commandFormat">The command with placeholder values for variables.</param>
        /// <param name="args">Any variables that will replace the placeholders in the command.</param>
        /// <returns>True if the command was executed successfully, false otherwise.</returns>
        bool ExecuteCommand(string commandFormat, params object[] args);
    }
}
