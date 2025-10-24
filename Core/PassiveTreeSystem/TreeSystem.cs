using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Umbra.Content.Configs;
using Umbra.Content.GUI;
using Umbra.Content.Passives;
using Umbra.Core.Loaders.UILoading;

namespace Umbra.Core.PassiveTreeSystem
{
    internal class TreeSystem : ModSystem
    {
        public const int TREE_VERSION = -9997;

        public static PassiveTree vanillaTree;
        public static PassiveTree tree;

        public static bool hasCustomTree;

        public override void Load()
        {
            LoadVanillaTree();
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
            if (UILoader.GetUIState<Tree>().fullscreen && UILoader.GetUIState<Tree>().visible)
                return;

            orig(self);
        }

        public override void ClearWorld()
        {
            hasCustomTree = false;
            tree = vanillaTree;
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
            tag["hasCustomTree"] = hasCustomTree;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tree.Save(tag);
            tag["hasCustomTree"] = hasCustomTree;

            if (hasCustomTree)
                tag["customTree"] = GetTreeJson();

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
            hasCustomTree = tag.GetBool("hasCustomTree");

            if (!hasCustomTree)
            {
                tree = vanillaTree;
                int lastVersion = tag.GetInt("lastVersion");

                if (lastVersion != TREE_VERSION)
                {
                    tag["activeIDs"] = new List<int>();
                    Main.LocalPlayer.GetModPlayer<TreePlayer>().UmbraPoints += tag.GetInt("lastSpent");

                    Notification.DisplayNotification(
                        Language.GetTextValue("Mods.Umbra.GUI.Notification.ResetMessageName"),
                        Language.GetTextValue("Mods.Umbra.GUI.Notification.ResetMessageBody"));
                }
            }
            else if (tag.TryGet("customTree", out string customTreeJson))
            {
                LoadFromString(customTreeJson);
            }
            else // Fallback base if we have the custom tree flag set but no custom tree saved
            {
                tree = vanillaTree;
                tag["activeIDs"] = new List<int>();
                Main.LocalPlayer.GetModPlayer<TreePlayer>().UmbraPoints += tag.GetInt("lastSpent");

                Notification.DisplayNotification(
                    Language.GetTextValue("Mods.Umbra.GUI.Notification.CustomErrorMessageName"),
                    Language.GetTextValue("Mods.Umbra.GUI.Notification.CustomErrorMessageBody"));
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

        public static string GetTreeJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new PassiveConverter() },
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(tree, options);
        }

        public static void SwitchToCustomTree()
        {
            string path = Path.Combine("Data", "Template.json");

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

            hasCustomTree = true;
        }

        public static void LoadFromString(string treeJson)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new PassiveConverter() },
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            tree = JsonSerializer.Deserialize<PassiveTree>(treeJson, options);

            tree.GenerateDicts();
            tree.RegenrateConnections();
            tree.RegenerateFlows();
        }

        public static void LoadVanillaTree()
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

                vanillaTree = JsonSerializer.Deserialize<PassiveTree>(stream, options);
                stream.Close();
            }

            vanillaTree.GenerateDicts();
            vanillaTree.RegenrateConnections();
            vanillaTree.RegenerateFlows();

            tree = vanillaTree;
        }

        public static void Export()
        {
            string path = ModLoader.ModPath.Replace("Mods", "UmbraTreeExports");
            string name = $"CustomUmbraTree_{Main.worldName}_{DateTime.Now.ToString("MM_dd_yy-HH_mm_ss_fff")}.json";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string thisPath = Path.Combine(path, name);
            Main.NewText(Language.GetText("Mods.Umbra.GUI.Tree.Export").Format(thisPath), Color.Yellow);

            File.WriteAllText(thisPath, GetTreeJson());
        }

		public override void PostWorldGen()
		{
            var defaultCustom = ModContent.GetInstance<CustomTreeConfig>().customTreeJson;

            if (!string.IsNullOrEmpty(defaultCustom))
            {
                try
                {
                    hasCustomTree = true;
                    LoadFromString(defaultCustom);
                }
                catch
                {
                    hasCustomTree = false;
                    throw new Exception("Your umbra default custom tree JSON in your configuration is invalid or malformed!");
                }
            }
		}
	}
}
