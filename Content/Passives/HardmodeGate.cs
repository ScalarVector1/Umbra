using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
    internal class HardmodeGate : Passive
    {
        public override bool AllowDuplicates => true;

        public override void SetDefaults()
        {
            texture = Assets.Passives.HardmodeGate;
            difficulty = 0;
            size = 2;
        }

        public override bool CanAllocate(Player player)
        {
            return base.CanAllocate(player) && Main.hardMode;
        }
    }
}
