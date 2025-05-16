using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Umbra.Core.Loaders.UILoading;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.GUI
{
	internal class InventoryButton : SmartUIState
	{
		public UIImageButton treeButton = new(Assets.Items.UmbraPickup);
		public bool flashing;

		public override bool Visible => Main.playerInventory;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		}

		public override void OnInitialize()
		{
			treeButton.Left.Set(574, 0);
			treeButton.Top.Set(274, 0);
			treeButton.Width.Set(32, 0);
			treeButton.Height.Set(32, 0);
			treeButton.OnLeftClick += openTree;
			treeButton.SetVisibility(1f, 1f);
			Append(treeButton);
		}

		private void openTree(UIMouseEvent evt, UIElement listeningElement)
		{
			UILoader.GetUIState<Tree>().visible = true;
			flashing = false;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			treeButton.Top.Set(110, 0);
			Recalculate();

			Texture2D back = Assets.GUI.GlowSoft.Value;

			Vector2 pos = treeButton.GetDimensions().Center();

			spriteBatch.Draw(back, pos + new Vector2(0, -6), null, Color.Black, 0, back.Size() / 2f, 1f, 0, 0);

			base.Draw(spriteBatch);

			var color = new Color(180, 120, 255);
			float scale = 0.8f;

			if (flashing)
			{
				color = Color.Lerp(new Color(180, 120, 255), Color.Yellow, 0.5f + 0.5f * MathF.Sin(Main.GameUpdateCount * 0.2f));
				scale = 0.85f + 0.05f * MathF.Sin(Main.GameUpdateCount * 0.2f);
			}

			Utils.DrawBorderString(spriteBatch, $"{Main.LocalPlayer.GetModPlayer<TreePlayer>().UmbraPoints}", pos, color, scale, 0.5f, 0.6f);

			if (treeButton.IsMouseHovering)
			{
				Tooltip.SetName($"Umbral Tree ([c/CC88FF:{Main.LocalPlayer.GetModPlayer<TreePlayer>().UmbraPoints}] unspent umbra)");
				Tooltip.SetTooltip("Click this to open the umbral tree, where you can spend Umbra to increase the difficulty of the game.");

				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
