using Microsoft.Kinect;
using System;

namespace AsimovClient.Sensing.Gestures
{
    public interface IGesture
    {
        /// <summary>
        /// Updates the gesture.
        /// </summary>
        /// <param name="data">The skeleton data.</param>
        void UpdateGesture(Skeleton data);
    }
}