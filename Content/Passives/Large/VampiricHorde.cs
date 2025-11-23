using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class VampiricHorde : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.VampiricHorde;
			difficulty = 20;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<VampiricHordeNPC>().active = true;
		}
	}

	internal class VampiricHordeNPC : GlobalNPC
	{
		public bool active;

		public override bool InstancePerEntity => true;

		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			if (active)
			{
				int amount = Math.Min(npc.lifeMax - npc.life, 100);
				npc.life += amount;
				npc.HealEffect(amount);
			}
		}
	}
}
