using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;
using StarlightRiver.Core.Systems.BarrierSystem;

namespace Umbra.Content.Passives.Crossmod.StarlightRiverPassives
{
    internal class StarlightEnemyBarrier : StarlightRiverPassive
    {
        public override void SetDefaults()
        {
            texture = Assets.Passives.StarlightEnemyBarrier;
            difficulty = 2;
        }

        [JITWhenModsEnabled("StarlightRiver")]
        public override void OnEnemySpawn(NPC npc)
        {
            npc.GetGlobalNPC<BarrierNPC>().maxBarrier += 20;
            npc.GetGlobalNPC<BarrierNPC>().barrier += 20;
        }
    }
}
