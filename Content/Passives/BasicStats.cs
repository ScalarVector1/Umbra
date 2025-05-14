using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Terraria.ID;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Passives
{
	internal class EnemyDamage : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyDamage;
			difficulty = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<TreeNPC>().flatDamage += 2;
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
			npc.GetGlobalNPC<TreeNPC>().moreDamage.Add(0.1f);
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
			npc.GetGlobalNPC<TreeNPC>().AddStatusChance(BuffID.Bleeding, 0.05f);
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
			npc.GetGlobalNPC<TreeNPC>().AddStatusChance(BuffID.Poisoned, 0.05f);
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
			npc.GetGlobalNPC<TreeNPC>().increasedLife += 0.1f;
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
			npc.GetGlobalNPC<TreeNPC>().flatRegen += 4;
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
			difficulty = 5;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<TreeNPC>().endurance += 0.05f;
		}
	}
}
