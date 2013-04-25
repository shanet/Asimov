//------------------------------------------------------------------------------
// <copyright file="AsimovClient.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
// <summary>
//     The main class of the client.  Initializes Asimov and accepts command
//     line input.
// </summary>
//------------------------------------------------------------------------------

namespace AsimovClient
{
    using System;
    using System.Threading;

    using Logging;

    public class AsimovClient
    {
        private static Asimov asimov;

        public static ManualResetEvent EndEvent { get; private set; }

        public static void Main(string[] args)
        {
            try
            {
                EndEvent = new ManualResetEvent(false);

                // Start Asimov
                asimov = new Asimov();

                // Listen for console input in a separate thread
                new Thread(ListenForConsoleInput).Start();

                // Block until endEvent is fired
                EndEvent.WaitOne();

                // Dispose of Asimov and its resources
                if (asimov != null)
                {
                    asimov.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("FATAL ERROR: {0}", e);
                AsimovLog.WriteLine("FATAL ERROR: {0}", e);
            }
        }

        private static void ListenForConsoleInput()
        {
            string input;

            // Wait for the exit command
            do
            {
                input = Console.ReadLine();
            }
            while (string.Compare(input, Constants.ExitCommand, StringComparison.CurrentCultureIgnoreCase) != 0);

            // The exit command was recieved, so fire endEvent
            EndEvent.Set();
        }
    }
}
