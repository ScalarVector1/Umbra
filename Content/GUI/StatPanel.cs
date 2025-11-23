using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;
using Umbra.Content.GUI.FieldEditors;
using Umbra.Core;
using Umbra.Core.Loaders.UILoading;
using Umbra.Core.PassiveTreeSystem;
using static AssGen.Assets;

namespace Umbra.Content.GUI
{
	internal class StatPanel : SmartUIElement
	{
		public UIPanel panel;
		public UIList list;
		Terraria.GameContent.UI.Elements.FixedUIScrollbar scroll;

		public TextField search;

		public StatPanel(UserInterface ui)
		{
			panel = new UIPanel();
			list = new UIList();
			scroll = new(ui);

			Width.Set(366, 0);
			Height.Set(800, 0);

			panel.Left.Set(0, 0);
			panel.Top.Set(0, 0);
			panel.Width.Set(0, 1);
			panel.Height.Set(0, 1);
			panel.BackgroundColor = new Color(20, 15, 40) * 0.85f;
			Append(panel);

			search = new TextField();
			search.Left.Set(12, 0);
			search.Top.Set(12, 0);
			search.Width.Set(-24 - 24, 1f);
			search.Height.Set(32, 0);
			search.color = new Color(100, 80, 160) * 0.85f;
			Append(search);

			list.Left.Set(0, 0);
			list.Top.Set(52, 0);
			list.Width.Set(-32, 1);
			list.Height.Set(-52, 1);
			list.ListPadding = 0;
			list.SetScrollbar(scroll);
			Append(list);

			scroll.Left.Set(-26, 1);
			scroll.Top.Set(16, 0);
			scroll.Width.Set(22, 0);
			scroll.Height.Set(-32, 1);
			Append(scroll);
		}

		public void Populate()
		{
			list.Clear();

			foreach(var line in TreeSystem.tree.tooltips.ordered)
			{
				list.Add(new StatPanelLine(line, this));
			}

			Recalculate();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			var frame = GetDimensions().ToRectangle();
			frame.Inflate(4, 4);
			GUIHelper.DrawFancyFrame(spriteBatch, frame);

			if (search.updated)
				Recalculate();
		}
	}

	internal class StatPanelLine : SmartUIElement
	{
		TreeTooltipLine line;
		private TextSnippet[] text;
		private StatPanel parent;

		public StatPanelLine(TreeTooltipLine line, StatPanel parent)
		{
			this.line = line;
			this.parent = parent;

			text = ChatManager.ParseMessage(line.displayedText, Color.White).ToArray();

			ReLogic.Graphics.DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;
			var size = ChatManager.GetStringSize(font, text, Vector2.One * 0.8f, 300);

			Width.Set(0, 1);
			Height.Set(size.Y + 6, 0);
			this.parent = parent;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			var bullet = Assets.Passives.UnloadedPassive.Value;
			var line = Assets.MagicPixel.Value;

			ReLogic.Graphics.DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;

			Vector2 pos = GetDimensions().ToRectangle().TopLeft();

			spriteBatch.Draw(bullet, pos + Vector2.One * 8, Color.White);
			spriteBatch.Draw(line, new Rectangle((int)pos.X, (int)pos.Y, 334, 4), Color.Black * 0.25f);

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, text, pos + new Vector2(34, 10), 0, Vector2.Zero, Vector2.One * 0.8f, out int hovered, 300);
		
			base.Draw(spriteBatch);
		}

		public void ShrinkIfFiltered()
		{
			if (!line.displayedText.ToLower().Contains(parent.search.currentValue))
			{
				Width.Set(0, 0);
				Height.Set(0, 0);

				MarginLeft = 0;
				MarginRight = 0;
				MarginTop = 0;
				MarginBottom = 0;
			}
			else
			{
				ReLogic.Graphics.DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;
				var size = ChatManager.GetStringSize(font, text, Vector2.One * 0.8f, 300);

				Width.Set(0, 1);
				Height.Set(size.Y + 6, 0);
			}
		}

		public override void Recalculate()
		{
			ShrinkIfFiltered();
			base.Recalculate();
		}

		public override int CompareTo(object obj)
		{
			if (obj is StatPanelLine other)
				return line.CompareTo(other.line);

			return base.CompareTo(obj);
		}
	}

}
