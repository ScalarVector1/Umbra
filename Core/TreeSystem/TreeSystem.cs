using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Umbra.Content.GUI;
using Umbra.Core.Loaders.UILoading;

namespace Umbra.Core.TreeSystem
{
	internal class TreePlayer : ModPlayer
	{
		public int UmbraPoints;

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
		public override void OnSpawn(NPC npc, IEntitySource source)
		{
			foreach (Passive passive in ModContent.GetInstance<TreeSystem>().tree.Nodes)
			{
				if (passive.active)
					passive.OnEnemySpawn(npc);
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
