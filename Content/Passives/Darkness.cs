using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
    internal class Darkness : Passive
    {
        public override void SetDefaults()
        {
            texture = Assets.Passives.Darkness;
            difficulty = 1;
        }

        public override void Update()
        {
            LightMultiplier.lightMultiplier -= 0.005f;
        }
    }
}
