using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Umbra.Core.TreeSystem;

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

	internal class HardmodeEnemyPoision : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.HardmodeEnemyPoision;
			difficulty = 5;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<TreeNPC>().AddStatusChance(BuffID.Poisoned, 0.1f);
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
			npc.GetGlobalNPC<TreeNPC>().AddStatusChance(BuffID.Venom, 0.02f);
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
				npc.GetGlobalNPC<TreeNPC>().increasedDamage += 0.01f * (ModContent.GetInstance<TreeSystem>().tree.difficulty / 20);
				npc.GetGlobalNPC<TreeNPC>().flatLife += ModContent.GetInstance<TreeSystem>().tree.difficulty / 5;
			}
		}
	}

	internal class HardmodeDoubleChance: Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.HardmodeDoubleChance;
			difficulty = 6;
		}

		public override void Update()
		{
			TreeNPC.doubleSpawnChance += 0.05f;
		}
	}
}
