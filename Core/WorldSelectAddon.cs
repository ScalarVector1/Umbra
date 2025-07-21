using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Umbra.Core
{
	internal class WorldSelectAddon : ModSystem
	{
		public static Dictionary<string, int> difficulties = [];
		public static Dictionary<string, bool> customs = [];

		public override void Load()
		{
			On_UIWorldListItem.DrawSelf += DrawAddon;
			On_Main.LoadWorlds += RefreshWorldData;
		}

		private void RefreshWorldData(On_Main.orig_LoadWorlds orig)
		{
			orig();
			difficulties.Clear();
		}

		private void DrawAddon(On_UIWorldListItem.orig_DrawSelf orig, UIWorldListItem self, SpriteBatch spriteBatch)
		{
			orig(self, spriteBatch);

			string path = self._data.Path;

			Vector2 pos = self.GetDimensions().ToRectangle().TopLeft() + new Vector2(156, 32);

			var tex = Assets.Items.UmbraPickup.Value;
			var glow = Assets.GUI.GlowSoft.Value;
			spriteBatch.Draw(glow, pos + tex.Size() / 2f, null, Color.Black, 0, glow.Size() / 2f, 0.7f, 0, 0);
			spriteBatch.Draw(tex, pos, Color.White);

			if (difficulties.ContainsKey(path) && customs.ContainsKey(path))
			{
				if (!customs[path])
					Utils.DrawBorderString(spriteBatch, $"{difficulties[path]}", pos + tex.Size() / 2f, new Color(255, 100, 100), 0.7f, 0.5f, 0.5f);
				else
				{
					Utils.DrawBorderString(spriteBatch, "Custom", pos + tex.Size() / 2f + new Vector2(0, -12), new Color(255, 150, 100), 0.7f, 0.5f, 0.5f);
					Utils.DrawBorderString(spriteBatch, $"{difficulties[path]}", pos + tex.Size() / 2f, new Color(255, 150, 100), 0.7f, 0.5f, 0.5f);
				}
			}
			else
			{
				if(self._data.TryGetHeaderData<PassiveTreeSystem.TreeSystem>(out var tag))
				{
					difficulties.TryAdd(path, tag.GetInt("lastDifficulty"));
					customs.TryAdd(path, tag.GetBool("hasCustomTree"));
					return;
				}

				difficulties.TryAdd(path, 0);
				customs.TryAdd(path, false);
			}
		}
	}
}
