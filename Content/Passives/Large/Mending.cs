using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
    internal class Mending : Passive
    {
        public override void SetDefaults()
        {
            texture = Assets.Passives.Mending;
            difficulty = 25;
            size = 1;
        }

        public override void OnEnemySpawn(NPC npc)
        {
            npc.GetGlobalNPC<RegenerationNPC>().increasedRegen += 0.5f;
        }
    }
}
