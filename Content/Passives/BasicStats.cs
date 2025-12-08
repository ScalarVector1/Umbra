using Terraria.ID;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
	internal class EnemyStrength : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.NormalStrenght;
			difficulty = 2;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			if (!npc.boss)
			{
				npc.GetGlobalNPC<TreeNPC>().increasedLife += 0.02f;
				npc.GetGlobalNPC<TreeNPC>().increasedDamage += 0.02f;
			}
		}
	}

	internal class EnemyDamage : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyDamage;
			difficulty = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<TreeNPC>().flatDamage += 1;
		}
	}

	internal class MoreEnemyDamage : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.MoreEnemyDamage;
			difficulty = 5;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<TreeNPC>().moreDamage.Add(0.05f);
		}
	}

	internal class EnemyBleed : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyBleed;
			difficulty = 2;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<StatusChanceNPC>().AddStatusChance(BuffID.Bleeding, 0.05f);
		}
	}

	internal class EnemyPoision : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyPoision;
			difficulty = 2;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<StatusChanceNPC>().AddStatusChance(BuffID.Poisoned, 0.05f);
		}
	}

	internal class EnemyFrostburn : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyFrostburn;
			difficulty = 4;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<StatusChanceNPC>().AddStatusChance(BuffID.Frostburn, 0.05f);
		}
	}

	internal class EnemySlow : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemySlow;
			difficulty = 6;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<StatusChanceNPC>().AddStatusChance(BuffID.Slow, 0.1f);
		}
	}

	internal class EnemyLife : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyHP;
			difficulty = 1;
		}
		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<TreeNPC>().increasedLife += 0.03f;
		}
	}

	internal class EnemyMoreLife : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyMoreLife;
			difficulty = 4;
		}
		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<TreeNPC>().moreLife.Add(0.1f);
		}
	}

	internal class EnemyRegen : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyRegen;
			difficulty = 2;
		}
		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<RegenerationNPC>().flatRegen += 2;
		}
	}

	internal class EnemyDefense : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyDefense;
			difficulty = 2;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<TreeNPC>().flatDefense += 1;
		}
	}

	internal class EnemySurvival : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemySurvival;
			difficulty = 5;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<TreeNPC>().flatLife += 25;
			npc.GetGlobalNPC<TreeNPC>().flatDefense += 1;
		}
	}

	internal class EnemyEndurance : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyEndurance;
			difficulty = 3;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<TreeNPC>().endurance += 0.02f;
		}
	}

	internal class EnemyDodge : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyDodge;
			difficulty = 4;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<DodgeNPC>().flatDodge += 0.02f;
		}
	}

	internal class BossStrength : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.BossStrength;
			difficulty = 3;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			if (npc.boss)
			{
				npc.GetGlobalNPC<TreeNPC>().endurance += 0.02f;
				npc.GetGlobalNPC<TreeNPC>().increasedDamage += 0.05f;
			}
		}
	}

	internal class StrengthPerDoom : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.StrengthPerDoom;
			difficulty = 8;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<ArmorPenetrationNPC>().penetrate += TreeSystem.tree.difficulty / 150;
			npc.GetGlobalNPC<TreeNPC>().flatLife += TreeSystem.tree.difficulty / 50;
		}
	}

	internal class SpawnRate : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.SpawnRate;
			difficulty = 1;
		}

		public override void Update()
		{
			TreeNPCGlobals.spawnRateModifier += 0.05f;
		}
	}

	internal class EnemyPenetrate : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyPenetrate;
			difficulty = 5;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<ArmorPenetrationNPC>().penetrate += 2;
		}
	}
}
