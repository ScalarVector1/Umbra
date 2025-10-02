using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
    internal class BloodMoonStrength : Passive
    {
        public override void SetDefaults()
        {
            texture = Assets.Passives.BloodMoonStrength;
            difficulty = 2;
        }

        public override void OnEnemySpawn(NPC npc)
        {
            if (Main.bloodMoon)
            {
                npc.GetGlobalNPC<TreeNPC>().flatDamage += 1;
                npc.GetGlobalNPC<TreeNPC>().flatDefense += 1;
            }
        }
    }

    internal class BloodMoonBleed : Passive
    {
        public override void SetDefaults()
        {
            texture = Assets.Passives.BloodMoonBleed;
            difficulty = 2;
        }

        public override void OnEnemySpawn(NPC npc)
        {
            if (Main.bloodMoon)
            {
                npc.GetGlobalNPC<StatusChanceNPC>().AddStatusChance(BuffID.Bleeding, 0.1f);
            }
        }
    }
}
