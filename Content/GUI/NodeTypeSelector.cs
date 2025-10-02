using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using Umbra.Content.Passives.Crossmod;
using Umbra.Core.PassiveTreeSystem;
using Umbra.Core;
using Umbra.Core.Loaders.UILoading;
using Umbra.Content.GUI.FieldEditors;
using Umbra.Content.Passives;

namespace Umbra.Content.GUI
{
	internal class NodeTypeSelector : SmartUIElement
	{
		UIGrid choices;
		Terraria.GameContent.UI.Elements.FixedUIScrollbar scroll;
		public TextField search;

		int recalcTime;

		public void Initialize(UserInterface ui)
		{
			var panel = new UIPanel();
			panel.Left.Set(0, 0);
			panel.Top.Set(0, 0);
			panel.Width.Set(0, 1f);
			panel.Height.Set(0, 1f);
			panel.BackgroundColor = new Color(0f, 0f, 0f, 0.4f);
			Append(panel);

			search = new TextField();
			search.Left.Set(12, 0);
			search.Top.Set(12, 0);
			search.Width.Set(-24, 1f);
			search.Height.Set(32, 0);
			Append(search);

			scroll = new(ui);
			scroll.Left.Set(-24, 0);
			scroll.Top.Set(60, 0);
			scroll.Width.Set(32, 0);
			scroll.Height.Set(-70, 1f);
			Append(scroll);

			choices = new();
			choices.Left.Set(12, 0);
			choices.Top.Set(50, 0);
			choices.Width.Set(0, 1f);
			choices.Height.Set(-24 - 32, 1f);
			choices.ListPadding = 0;
			choices.SetScrollbar(scroll);
			Append(choices);

			foreach (Passive passive in ModContent.GetContent<Passive>())
			{
				if (passive is not UnloadedPassive)
					choices.Add(new NodeChoice(passive, this));
			}

			choices.UpdateOrder();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			GUIHelper.DrawFancyFrame(spriteBatch, GetDimensions().ToRectangle());

			if (search.updated)
				Recalculate();

			if (recalcTime > 0)
			{
				Recalculate();
				recalcTime--;
			}
		}

		public override void SafeScrollWheel(UIScrollWheelEvent evt)
		{
			recalcTime = 2;
		}
	}

	internal class NodeChoice : SmartUIElement
	{
		public Passive passive;
		public bool filtered;
		public NodeTypeSelector parent;

		public NodeChoice(Passive passive, NodeTypeSelector parent)
		{
			this.passive = passive;

			Width.Set(40, 0);
			Height.Set(40, 0);
			this.parent = parent;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Tree.editing && Tree.toPlace == passive)
			{
				Texture2D glow = Assets.GUI.GlowAlpha.Value;
				Texture2D star = Assets.GUI.StarAlpha.Value;

				var glowColor = new Color(120, 255, 120)
				{
					A = 0
				};

				spriteBatch.Draw(glow, GetDimensions().Center(), null, glowColor * 0.5f, 0, glow.Size() / 2f, 1f, 0, 0);
				spriteBatch.Draw(star, GetDimensions().Center(), null, glowColor * 0.25f, 0, star.Size() / 2f, 1f, 0, 0);
			}

			if (Tree.editing && IsMouseHovering)
			{
				Texture2D glow = Assets.GUI.GlowAlpha.Value;
				Texture2D star = Assets.GUI.StarAlpha.Value;

				var glowColor = new Color(160, 160, 60)
				{
					A = 0
				};

				spriteBatch.Draw(glow, GetDimensions().Center(), null, glowColor * 0.5f, 0, glow.Size() / 2f, 1f, 0, 0);
				spriteBatch.Draw(star, GetDimensions().Center(), null, glowColor * 0.25f, 0, star.Size() / 2f, 1f, 0, 0);
			}

			spriteBatch.Draw(passive.texture.Value, GetDimensions().ToRectangle(), null, Color.White);

			if (IsMouseHovering)
			{
				string cost = "\n" + Language.GetText("Mods.Umbra.GUI.Node.Doom").Format(passive.difficulty);
				string max = passive.AllowDuplicates ? "" : $"\n{Language.GetText("Mods.Umbra.GUI.Tree.MaxOne").Value}";

				Tooltip.SetName(passive.DisplayName);
				Tooltip.SetTooltip(passive.Tooltip + cost + max);
			}

			base.Draw(spriteBatch);
		}

		public void ShrinkIfFiltered()
		{
			if (!passive.DisplayName.ToLower().Contains(parent.search.currentValue))
			{
				Width.Set(0, 0);
				Height.Set(0, 0);

				MarginLeft = 0;
				MarginRight = 0;
				MarginTop = 0;
				MarginBottom = 0;

				filtered = true;
			}
			else
			{
				Width.Set(40, 0);
				Height.Set(40, 0);

				MarginLeft = 2;
				MarginRight = 2;
				MarginTop = 2;
				MarginBottom = 2;
				filtered = false;
			}
		}

		public override void Recalculate()
		{
			ShrinkIfFiltered();
			base.Recalculate();
		}

		public override void SafeClick(UIMouseEvent evt)
		{
			Tree.toPlace = passive;
		}

		public override void SafeRightClick(UIMouseEvent evt)
		{
			if (Tree.selected != null && TreeSystem.tree.CanInsert(passive))
			{
				int id = Tree.selected.ID;
				int x = Tree.selected.X;
				int y = Tree.selected.Y;

				int cost = Tree.selected.Cost;
				List<Passive> connections = Tree.selected.connections;

				TreeSystem.tree.Nodes.Remove(Tree.selected);
				Tree.selected = passive.Clone();
				Tree.selected.ID = id;
				Tree.selected.X = x;
				Tree.selected.Y = y;
				Tree.selected.Cost = cost;
				Tree.selected.connections = connections;
				TreeSystem.tree.Nodes.Add(Tree.selected);

				UILoader.GetUIState<Tree>().Refresh();
			}
			else
			{
				SoundEngine.PlaySound(SoundID.Unlock);
			}
		}

		public override int CompareTo(object obj)
		{
			if (obj is NodeChoice choice)
			{
				if (passive.size != choice.passive.size)
					return passive.size.CompareTo(choice.passive.size);

				if ((passive is ModGate) != (choice.passive is ModGate))
					return (passive is ModGate).CompareTo(choice.passive is ModGate);

				if ((passive is CrossmodPassive) != (choice.passive is CrossmodPassive))
					return (passive is CrossmodPassive).CompareTo(choice.passive is CrossmodPassive);

				return passive.DisplayName.CompareTo(choice.passive.DisplayName);
			}

			return base.CompareTo(obj);
		}
	}
}
