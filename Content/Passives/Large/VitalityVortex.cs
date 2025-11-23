using System.Linq;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class VitalityVortex : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.VitalityVortex;
			difficulty = 20;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<VitalityVortexNPC>().active = true;
		}

		public override void BuffPlayer(Player player)
		{
			if (Main.npc.Any(n => n.active && Vector2.Distance(n.Center, player.Center) < 160))
				player.GetModPlayer<RegenerationPlayer>().increasedRegen -= 0.25f;
		}
	}

	internal class VitalityVortexNPC : GlobalNPC
	{
		public bool active;

		public override bool InstancePerEntity => true;

		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			if (active && Main.player.Any(n => n.active && !n.dead && Vector2.Distance(n.Center, npc.Center) < 160))
				npc.lifeRegen += 40;
		}
	}
}
