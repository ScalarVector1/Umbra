using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace Umbra.Core.TreeSystem
{
	internal class TreeNPC : GlobalNPC
	{
		public int flatLife;
		public float increasedLife;
		public List<float> moreLife = [];

		public int flatRegen;
		public float increasedRegen;

		public int flatDamage;
		public float increasedDamage;
		public List<float> moreDamage = [];

		public int flatDefense;
		public float increasedDefense;
		public List<float> moreDefense = [];

		public float endurance;

		public Dictionary<int, float> statusChances = [];
		public int statusDuration = 300;

		public static float spawnRateModifier = 1;

		public static float doubleSpawnChance = 0;

		public override bool InstancePerEntity => true;

		public override void ResetEffects(NPC npc)
		{
			spawnRateModifier = 1;
			doubleSpawnChance = 0;
		}

		public void AddStatusChance(int type, float chance)
		{
			if (statusChances.ContainsKey(type))
				statusChances[type] += chance;
			else
				statusChances.Add(type, chance);
		}

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			if (spawnRate == 0)
			{
				spawnRate = int.MaxValue;
				maxSpawns = 0;
				return;
			}

			spawnRate = (int)(spawnRate / spawnRateModifier);
			maxSpawns = (int)(maxSpawns * spawnRateModifier);
		}

		public override void SetDefaults(NPC npc)
		{
			foreach (Passive passive in ModContent.GetInstance<TreeSystem>().tree.Nodes)
			{
				if (passive.active)
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
			if (!npc.boss && Main.rand.NextFloat() <= doubleSpawnChance)
			{
				NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, npc.type);
			}
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			npc.lifeRegen += flatRegen;
			npc.lifeRegen += (int)(npc.lifeRegen * increasedRegen);
		}

		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			foreach (KeyValuePair<int, float> pair in statusChances)
			{
				if (Main.rand.NextFloat() < pair.Value)
					target.AddBuff(pair.Key, statusDuration);
			}
		}

		public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
		{
			modifiers.FinalDamage *= 1f - endurance;
		}
	}
}
