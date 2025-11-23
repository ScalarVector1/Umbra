using Terraria.ID;

namespace Umbra.Core
{
	internal class RegenerationNPC : GlobalNPC
	{
		public int flatRegen;
		public float increasedRegen;

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
		{
			return !entity.friendly && entity.lifeMax > 5 && !NPCID.Sets.CountsAsCritter[entity.type] && entity.damage > 0;
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			if (npc.realLife == -1)
			{
				npc.lifeRegen += flatRegen;
				npc.lifeRegen += (int)(npc.lifeRegen * increasedRegen);
			}
		}
	}

	internal class RegenerationPlayer : ModPlayer
	{
		public int flatRegen;
		public float increasedRegen;

		public override void ResetEffects()
		{
			flatRegen = 0;
			increasedRegen = 0;
		}

		public override void UpdateLifeRegen()
		{
			Player.lifeRegen += flatRegen;
			Player.lifeRegen += (int)(Player.lifeRegen * increasedRegen);
		}
	}
}
