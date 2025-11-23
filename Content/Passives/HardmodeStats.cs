using Terraria.ID;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
	internal class HardmodeStrength : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.HardmodeNormalStrenght;
			difficulty = 4;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			if (!npc.boss)
			{
				npc.GetGlobalNPC<TreeNPC>().endurance += 0.02f;
				npc.GetGlobalNPC<TreeNPC>().increasedDamage += 0.05f;
			}
		}
	}

	internal class HardmodeEndurance : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.HardmodeEndurance;
			difficulty = 6;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<TreeNPC>().endurance += 0.06f;
		}
	}

	internal class HardmodeEnemyPoision : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.HardmodeEnemyPoision;
			difficulty = 5;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<StatusChanceNPC>().AddStatusChance(BuffID.Poisoned, 0.1f);
		}
	}

	internal class HardmodeEnemyVenom : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.HardmodeEnemyVenom;
			difficulty = 5;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<StatusChanceNPC>().AddStatusChance(BuffID.Venom, 0.02f);
		}
	}

	internal class HardmodeStrengthPerDoom : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.HardmodeStrengthPerDoom;
			difficulty = 8;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			if (npc.boss)
			{
				npc.GetGlobalNPC<TreeNPC>().increasedDamage += 0.01f * (TreeSystem.tree.difficulty / 20);
				npc.GetGlobalNPC<TreeNPC>().flatLife += TreeSystem.tree.difficulty / 10;
			}
		}
	}

	internal class HardmodeDoubleChance : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.HardmodeDoubleChance;
			difficulty = 6;
		}

		public override void Update()
		{
			TreeNPCGlobals.doubleSpawnChance += 0.05f;
		}
	}
}
