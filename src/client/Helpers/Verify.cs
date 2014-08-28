//------------------------------------------------------------------------------
// <copyright file="Verify.cs" company="Gage Ames">
//     Copyright (c) Gage Ames.  All rights reserved.
// </copyright>
// <summary>
//     Static class that contains helper functions to verify Boolean conditions
//     are satisfied, and react accordingly.  Mostly used to verify function
//     arguments and throw appropriate exceptions.
// </summary>
//------------------------------------------------------------------------------

namespace AsimovClient.Helpers
{
    using System;

    public static class Verify
    {
        public const string DefaultParamName = "Parameter";

        public static void ArgumentNotNull(object argument, string paramName = DefaultParamName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(string.Format("{0} cannot be null.", paramName), paramName);
            }
        }

        public static void ArgumentAtLeast(IComparable argument, IComparable minimum, string paramName = DefaultParamName)
        {
            if (argument.CompareTo(minimum) < 0)
            {
                throw new ArgumentException(string.Format("{0} must be at least {1}.", paramName, minimum), paramName);
            }
        }

        public static void ArgumentAtMost(IComparable argument, IComparable maximum, string paramName = DefaultParamName)
        {
            if (argument.CompareTo(maximum) > 0)
            {
                throw new ArgumentException(string.Format("{0} must be at most {1}.", paramName, maximum), paramName);
            }
        }

        public static void ArgumentInRange(IComparable argument, IComparable minimum, IComparable maximum, string paramName = DefaultParamName)
        {
            if (argument.CompareTo(minimum) < 0 || argument.CompareTo(maximum) > 0)
            {
                throw new ArgumentException(string.Format("{0} must be between {1} and {2}.", paramName, minimum, maximum), paramName);
            }
        }
    }
}
