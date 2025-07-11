using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Terraria.ModLoader.IO;
using Umbra.Content.GUI;
using Umbra.Content.Passives;
using Umbra.Core.Loaders.UILoading;

namespace Umbra.Core.PassiveTreeSystem
{
	internal class TreeSystem : ModSystem
	{
		public const int TREE_VERSION = -9997;

		public static PassiveTree tree;

		public override void Load()
		{
			LoadFromFile();
			On_Main.GUIBarsDraw += DontDrawInFullscreenTree;
			On_Main.CanPauseGame += PauseInFullscreen;
		}

		private bool PauseInFullscreen(On_Main.orig_CanPauseGame orig)
		{
			Tree tree = UILoader.GetUIState<Tree>();

			if (tree is null)
				return orig();

			return orig() || tree.visible && tree.fullscreen;
		}

		private void DontDrawInFullscreenTree(On_Main.orig_GUIBarsDraw orig, Main self)
		{
			if (!UILoader.GetUIState<Tree>().fullscreen)
				orig(self);
		}

		public override void OnWorldLoad()
		{
			if (!Main.dedServ)
			{
				UILoader.GetUIState<Tree>().RemoveAllChildren();
				Tree.Populated = false;
			}
		}

		public override void SaveWorldHeader(TagCompound tag)
		{
			tree.CalcDifficulty();
			tag["lastDifficulty"] = tree.difficulty;
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tree.Save(tag);

			int lastSpent = 0;

			foreach (Passive passive in tree.activeNodes)
			{
				lastSpent += passive.Cost;
			}

			tag["lastSpent"] = lastSpent;
			tag["lastVersion"] = TREE_VERSION;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			int lastVersion = tag.GetInt("lastVersion");

			if (lastVersion != TREE_VERSION)
			{
				tag["activeIDs"] = new List<int>();
				Main.LocalPlayer.GetModPlayer<TreePlayer>().UmbraPoints += tag.GetInt("lastSpent");

				Notification.DisplayNotification("[c/CC88FF:Tree Reset]", "The umbral tree has changed. As a consequence, all nodes need to be de-allocated. Your spent umbra has been refunded, so you can re-invest into the new tree as you wish!\n\n[c/bbbbbb:Click on this notification to close it.]");
			}

			tree.Load(tag);

			if (!Main.dedServ)
				UILoader.GetUIState<Tree>().Refresh();
		}

		public override void PreUpdateEntities()
		{
			foreach (Passive passive in tree.activeNodes)
			{
				passive.Update();
			}
		}

		public override void PostUpdateEverything()
		{
			if (!TreeSystem.tree.AnyActive<StartPoint>())
			{
				foreach (Passive node in TreeSystem.tree.Nodes)
				{
					if (node is StartPoint)
						TreeSystem.tree.Allocate(node.ID);
				}
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
