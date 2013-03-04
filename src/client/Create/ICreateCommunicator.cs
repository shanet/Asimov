//------------------------------------------------------------------------------
// <copyright file="ICreateCommunicator.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Create
{
    public interface ICreateCommunicator
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
