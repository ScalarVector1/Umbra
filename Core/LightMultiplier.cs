using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbra.Core
{
    internal class LightMultiplier : ModSystem
    {
        public static float lightMultiplier;

        public override void ModifyLightingBrightness(ref float scale)
        {
            scale += lightMultiplier;
            lightMultiplier = 0f;
        }

        public override void PostUpdateEverything()
        {
            //lightMultiplier = 0f;
        }
    }
}
