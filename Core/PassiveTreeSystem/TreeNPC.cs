using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace Umbra.Core.PassiveTreeSystem
{
	internal class TreeNPCGlobals : ModSystem
	{
		public static float spawnRateModifier = 1;
		public static float doubleSpawnChance = 0;

		public override void PostUpdateEverything()
		{
			spawnRateModifier = 1;
			doubleSpawnChance = 0;
		}
	}

	internal class TreeNPC : GlobalNPC
	{
		public int flatLife;
		public float increasedLife;
		public List<float> moreLife = [];

		public int flatDamage;
		public float increasedDamage;
		public List<float> moreDamage = [];

		public int flatDefense;
		public float increasedDefense;
		public List<float> moreDefense = [];

		public float endurance;

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
		{
			return !entity.friendly && entity.lifeMax > 5 && !NPCID.Sets.CountsAsCritter[entity.type] && entity.damage > 0;
		}

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			if (spawnRate == 0)
			{
				spawnRate = int.MaxValue;
				maxSpawns = 0;
				return;
			}

			spawnRate = (int)(spawnRate / TreeNPCGlobals.spawnRateModifier);
			maxSpawns = (int)(maxSpawns * TreeNPCGlobals.spawnRateModifier);
		}

		public override void SetDefaults(NPC npc)
		{
			foreach (Passive passive in TreeSystem.tree.activeNodes)
			{
				passive.OnEnemySpawn(npc);
			}

			if (!Main.expertMode)
			{
				ApplyStats(npc);
			}
		}

		public override void SetDefaultsFromNetId(NPC npc)
		{
			if (!Main.expertMode)
			{
				ApplyStats(npc);
			}
		}

		public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
		{
			ApplyStats(npc);
		}

		public void ApplyStats(NPC npc)
		{
			// Apply modifers after
			npc.lifeMax += flatLife;
			npc.lifeMax += (int)(npc.lifeMax * increasedLife);
			foreach (float more in moreLife)
			{
				npc.lifeMax += (int)(npc.lifeMax * more);
			}

			npc.life = npc.lifeMax;

			// damage
			npc.damage += flatDamage;
			npc.damage += (int)(npc.damage * increasedDamage);
			foreach (float more in moreDamage)
			{
				npc.damage += (int)(npc.damage * more);
			}

			// defense
			npc.defense += flatDefense;
			npc.defense += (int)(npc.defense * increasedDefense);
			foreach (float more in moreDefense)
			{
				npc.defense += (int)(npc.defense * more);
			}
		}

		public override void OnSpawn(NPC npc, IEntitySource source)
		{
			if (!npc.boss && Main.rand.NextFloat() <= TreeNPCGlobals.doubleSpawnChance)
			{
				NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, npc.type);
			}
		}

		public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
		{
			modifiers.FinalDamage *= 1f - endurance;
		}
	}
}
