using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using Umbra.Core.Loaders.UILoading;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.GUI
{
	internal class InventoryButton : SmartUIState
	{
		public UIImageButton treeButton = new(Assets.GUI.BarEmpty);
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
			treeButton.Width.Set(116, 0);
			treeButton.Height.Set(34, 0);
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

			var tp = Main.LocalPlayer.GetModPlayer<TreePlayer>();

			Texture2D back = Assets.GUI.GlowSoft.Value;

			Vector2 pos = treeButton.GetDimensions().Center();

			spriteBatch.Draw(back, pos + new Vector2(17, -6), null, Color.Black, 0, back.Size() / 2f, 1f, 0, 0);

			base.Draw(spriteBatch);

			var fill = Assets.GUI.BarFill.Value;
			var fillWidth = (int)(fill.Width * ((float)tp.partialPoints / TreePlayer.MAX_PARTIAL_POINTS));
			var fillSource = new Rectangle(0, 0, fillWidth, fill.Height);
			var fillTarget = new Rectangle((int)pos.X - 42, (int)pos.Y - 5, fillWidth, fill.Height);

			spriteBatch.Draw(fill, fillTarget, fillSource, Color.White);

			var color = new Color(200, 140, 255);
			float scale = 0.8f;

			if (flashing)
			{
				color = Color.Lerp(new Color(200, 140, 255), Color.Yellow, 0.5f + 0.5f * MathF.Sin(Main.GameUpdateCount * 0.2f));
				scale = 0.85f + 0.05f * MathF.Sin(Main.GameUpdateCount * 0.2f);
			}

			Vector2 pos2 = pos + new Vector2(17, 0);
			Utils.DrawBorderString(spriteBatch, $"{tp.UmbraPoints}", pos2, color, scale, 0.5f, 0.6f);

			if (treeButton.IsMouseHovering)
			{
				int unspent = tp.UmbraPoints;

				Tooltip.SetName(Language.GetText("Mods.Umbra.GUI.InventoryButton.Title").Format(unspent));
				Tooltip.SetTooltip(Language.GetText("Mods.Umbra.GUI.InventoryButton.Description").Value);

				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
