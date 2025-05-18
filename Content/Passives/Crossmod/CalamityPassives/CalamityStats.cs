using StarlightRiver.Core.Systems.BarrierSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Crossmod.CalamityPassives
{
	internal class CalamityDamage : CalamityPassive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyDamage;
			difficulty = 2;
		}

		[JITWhenModsEnabled("CalamityMod")]
		public override void OnEnemySpawn(NPC npc)
		{
			if (npc.ModNPC.Mod.Name == "CalamityMod")
				npc.GetGlobalNPC<TreeNPC>().flatDamage += 1;
		}
	}
}
