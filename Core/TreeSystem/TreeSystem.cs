using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Umbra.Content.GUI;
using Umbra.Core.Loaders.UILoading;

namespace Umbra.Core.TreeSystem
{
	internal class TreePlayer : ModPlayer
	{
		public int UmbraPoints;

		public int flatRegen;
		public float increasedRegen;

		public override void ResetEffects()
		{
			flatRegen = 0;
			increasedRegen = 0;
		}

		public override void UpdateEquips()
		{
			foreach (Passive passive in ModContent.GetInstance<TreeSystem>().tree.Nodes)
			{
				if (passive.active)
					passive.BuffPlayer(Player);
			}
		}

		public override void UpdateLifeRegen()
		{
			Player.lifeRegen += flatRegen;
			Player.lifeRegen += (int)(Player.lifeRegen * increasedRegen);
		}

		public override void SaveData(TagCompound tag)
		{
			tag["points"] = UmbraPoints;
		}

		public override void LoadData(TagCompound tag)
		{
			UmbraPoints = tag.GetInt("points");
		}
	}

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

		public override bool InstancePerEntity => true;

		public void AddStatusChance(int type, float chance)
		{
			if (statusChances.ContainsKey(type))
				statusChances[type] += chance;
			else
				statusChances.Add(type, chance);
		}

		public override void OnSpawn(NPC npc, IEntitySource source)
		{
			foreach (Passive passive in ModContent.GetInstance<TreeSystem>().tree.Nodes)
			{
				if (passive.active)
					passive.OnEnemySpawn(npc);
			}

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

	internal class TreeProjectile : GlobalProjectile
	{
		public TreeNPC spawner;

		public override bool InstancePerEntity => true;

		public override void OnSpawn(Projectile projectile, IEntitySource source)
		{
			if (source is EntitySource_Parent sourcez && sourcez.Entity is NPC npc)
			{
				spawner = npc.GetGlobalNPC<TreeNPC>();

				projectile.damage += spawner.flatDamage;
				projectile.damage += (int)(projectile.damage * spawner.increasedDamage);
				foreach (float more in spawner.moreDamage)
				{
					projectile.damage += (int)(projectile.damage * more);
				}
			}
		}

		public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
		{
			foreach (KeyValuePair<int, float> pair in spawner.statusChances)
			{
				if (Main.rand.NextFloat() < pair.Value)
					target.AddBuff(pair.Key, spawner.statusDuration);
			}
		}
	}

	internal class TreeSystem : ModSystem
	{
		public PassiveTree tree;

		public override void Load()
		{
			if (!Main.dedServ)
				LoadFromFile();
		}

		public override void OnWorldLoad()
		{
			UILoader.GetUIState<Tree>().RemoveAllChildren();
			Tree.Populated = false;
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag["activeIDs"] = tree.GetActiveIDs();
		}

		public override void LoadWorldData(TagCompound tag)
		{
			var activeIDs = (List<int>)tag.GetList<int>("activeIDs");
			tree.ApplyActiveIDs(activeIDs);
		}

		public override void PreUpdateEntities()
		{
			foreach (Passive passive in tree.Nodes)
			{
				if (passive.active)
					passive.Update();
			}
		}

		public void LoadFromFile()
		{
			string path = Path.Combine("Data", "Tree.json");

			if (Umbra.Instance.FileExists(path))
			{
				Stream stream = Umbra.Instance.GetFileStream(path);

				var options = new JsonSerializerOptions
				{
					Converters = { new PassiveConverter() },
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};

				tree = JsonSerializer.Deserialize<PassiveTree>(stream, options);
				stream.Close();
			}

			tree.GenerateDicts();
		}

		public void Export()
		{
			string downloadsPath = ModLoader.ModPath;
			string outputPath = Path.Combine(downloadsPath, "TreeExport.json");

			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				Converters = { new PassiveConverter() },
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			string json = JsonSerializer.Serialize(tree, options);

			File.WriteAllText(outputPath, json);
		}
	}
}
