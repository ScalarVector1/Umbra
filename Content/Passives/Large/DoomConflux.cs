using Umbra.Content.Buffs;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class DoomConflux : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.DoomConflux;
			difficulty = 40;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<StatusChanceNPC>().AddStatusChance(ModContent.BuffType<Doomed>(), 0.01f * (TreeSystem.tree.difficulty / 10));
		}
	}
}
