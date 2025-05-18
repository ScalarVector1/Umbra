using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Umbra.Content.GUI;
using Umbra.Core.Loaders.UILoading;

namespace Umbra.Core.PassiveTreeSystem
{
	internal class TreeSystem : ModSystem
	{
		public static PassiveTree tree;

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

		public override void SaveWorldHeader(TagCompound tag)
		{
			tree.CalcDifficulty();
			tag["lastDifficulty"] = tree.difficulty;
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tree.Save(tag);
		}

		public override void LoadWorldData(TagCompound tag)
		{
			tree.Load(tag);

			UILoader.GetUIState<Tree>().Refresh();
		}

		public override void PreUpdateEntities()
		{
			foreach (Passive passive in tree.activeNodes)
			{
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
			tree.RegenrateConnections();
			tree.RegenerateFlows();
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
