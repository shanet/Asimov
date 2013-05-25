//------------------------------------------------------------------------------
// <copyright file="IGesture.cs" company="Aaron Goodermuth">
//     Copyright (c) Aaron Goodermuth.  All rights reserved.
// </copyright>
// <summary>
//     Interface outlining properties of a generic gesture object.
// </summary>
//------------------------------------------------------------------------------

namespace AsimovClient.Sensing.Gestures
{
    using Microsoft.Kinect;

    public interface IGesture
    {
        /// <summary>
        /// Updates the gesture.
        /// </summary>
        /// <param name="data">The skeleton data.</param>
        void UpdateGesture(Skeleton data);
    }
}