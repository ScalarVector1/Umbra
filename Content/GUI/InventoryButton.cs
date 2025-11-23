using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using Umbra.Content.Configs;
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
			treeButton.Width.Set(92, 0);
			treeButton.Height.Set(34, 0);
			treeButton.OnLeftClick += OpenTree;
			treeButton.SetVisibility(1f, 1f);
			Append(treeButton);
		}

		public override void SafeUpdate(GameTime gameTime)
		{
			Vector2 configPos = ModContent.GetInstance<UIConfig>().UmbraIconPosition;

			if (treeButton.GetDimensions().Position() != configPos)
			{
				treeButton.Left.Set(configPos.X, 0);
				treeButton.Top.Set(configPos.Y, 0);
				treeButton.Width.Set(92, 0);
				treeButton.Height.Set(34, 0);
				Recalculate();
			}
		}

		private void OpenTree(UIMouseEvent evt, UIElement listeningElement)
		{
			Tree tree = UILoader.GetUIState<Tree>();
			tree.visible = true;

			if (tree.fullscreen)
				IngameFancyUI.OpenUIState(tree);

			tree.Recalculate();

			flashing = false;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			TreePlayer tp = Main.LocalPlayer.GetModPlayer<TreePlayer>();

			Texture2D back = Assets.GUI.GlowSoft.Value;

			Vector2 pos = treeButton.GetDimensions().Center();

			spriteBatch.Draw(back, pos + new Vector2(30, -6), null, Color.Black, 0, back.Size() / 2f, 1f, 0, 0);

			base.Draw(spriteBatch);

			Texture2D fill = Assets.GUI.BarFill.Value;
			int fillWidth = (int)(fill.Width * ((float)tp.partialPoints / tp.nextPoint));
			var fillSource = new Rectangle(0, 0, fillWidth, fill.Height);
			var fillTarget = new Rectangle((int)pos.X - 29, (int)pos.Y - 4, fillWidth, fill.Height);

			spriteBatch.Draw(fill, fillTarget, fillSource, Color.White);

			var color = new Color(200, 140, 255);
			float scale = 0.8f;

			if (flashing)
			{
				color = Color.Lerp(new Color(200, 140, 255), Color.Yellow, 0.5f + 0.5f * MathF.Sin(Main.GameUpdateCount * 0.2f));
				scale = 0.85f + 0.05f * MathF.Sin(Main.GameUpdateCount * 0.2f);
			}

			Vector2 pos2 = pos + new Vector2(30, 0);
			Utils.DrawBorderString(spriteBatch, $"{tp.UmbraPoints}", pos2, color, scale, 0.5f, 0.6f);

			if (treeButton.IsMouseHovering)
			{
				int unspent = tp.UmbraPoints;

				Tooltip.SetName(Language.GetText("Mods.Umbra.GUI.InventoryButton.Title").Format(unspent));
				Tooltip.SetTooltip(Language.GetText("Mods.Umbra.GUI.InventoryButton.Description").Format(tp.partialPoints, tp.nextPoint));

				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
