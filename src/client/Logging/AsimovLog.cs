//------------------------------------------------------------------------------
// <copyright file="AsimovLog.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace AsimovClient.Logging
{
    using System;
    using System.IO;

    public static class AsimovLog
    {
        public const bool IsLoggingEnabled = true;

        public const string LogFileName = "AsimovClientLog.txt";
        public const string OldLogFileName = "AsimovClientLog_old.txt";

        private static StreamWriter log;

        static AsimovLog()
        {
            if (File.Exists(LogFileName))
            {
                File.Delete(OldLogFileName);
                File.Move(LogFileName, OldLogFileName);
            }

            log = File.CreateText(LogFileName);
        }

        public static void WriteLine(string format, params object[] args)
        {
            if (IsLoggingEnabled)
            {
                log.WriteLine("[{0}] {1}", DateTime.Now.TimeOfDay.ToString(), string.Format(format, args));
                log.Flush();
            }
        }
    }
}
