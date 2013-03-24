
namespace AsimovClient.Modes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Kinect;

    public class ModeController
    {
        public ModeController()
        {
            this.CurrentMode = AsimovMode.None;
        }

        public AsimovMode CurrentMode { get; set; }

        public void UpdateSkeleton(Skeleton skeleton)
        {
            switch (this.CurrentMode)
            {
                case AsimovMode.Follow:
                    break;
                case AsimovMode.Avoid:
                    break;
                case AsimovMode.Obstacle:
                    break;
                case AsimovMode.Drinking:
                    break;
            }
        }
    }
}
