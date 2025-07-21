using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using Umbra.Content.GUI.FieldEditors;
using Umbra.Content.Passives.Crossmod;
using Umbra.Core;
using Umbra.Core.Loaders.UILoading;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.GUI
{
	internal class Tree : SmartUIState
	{
		public static bool Populated = false;

		private UIPanel panel;
		private InnerPanel inner;
		private UIImageButton closeButton;
		private UIImageButton exportButton;
		private UIImageButton editButton;
		private UIImageButton fullscreenButton;

		private NodeTypeSelector selector;
		private IntEditor costEditor;

		public bool visible;
		public static bool editing;
		public bool fullscreen;

		//Editing vars
		public static Passive toPlace;
		public static Passive selected;

		private static TreePlayer TreePlayer => Main.LocalPlayer.GetModPlayer<TreePlayer>();
		private static TreeSystem TreeSystem => ModContent.GetInstance<TreeSystem>();

		private const int TopPadding = -400;
		private const int LeftPadding = -400;
		private const int PanelWidth = 800;
		private const int PanelHeight = 800;

		public override bool Visible => visible;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		}

		public void StartEdit()
		{
			selector = new();
			selector.Left.Set(LeftPadding - 220, 0.5f);
			selector.Top.Set(TopPadding, 0.5f);
			selector.Width.Set(200, 0);
			selector.Height.Set(700, 0);
			selector.Initialize(UserInterface);
			Append(selector);

			costEditor = new("Cost", (a) =>
			{
				if (selected != null)
					selected.Cost = a;
			}, 0, () => selected?.Cost ?? 0, "Cost for the selected node");

			costEditor.Left.Set(LeftPadding - 220, 0.5f);
			costEditor.Top.Set(TopPadding + selector.Height.Pixels + 12, 0.5f);
			costEditor.OnInitialize();
			Append(costEditor);

			exportButton = new UIImageButton(Assets.GUI.ExportButton);
			exportButton.Left.Set(LeftPadding - 220 + costEditor.Width.Pixels + 12, 0.5f);
			exportButton.Top.Set(TopPadding + selector.Height.Pixels + 12, 0.5f);
			exportButton.Width.Set(38, 0);
			exportButton.Height.Set(38, 0);
			exportButton.OnLeftClick += (a, b) => TreeSystem.Export();
			exportButton.SetVisibility(1, 1);
			Append(exportButton);

			if (fullscreen)
			{
				selector.Left.Set(32, 0f);
				selector.Top.Set(124, 0f);

				costEditor.Left.Set(32, 0f);
				costEditor.Top.Set(124 + selector.Height.Pixels + 12, 0f);

				exportButton.Left.Set(32 + costEditor.Width.Pixels + 12, 0f);
				exportButton.Top.Set(124 + selector.Height.Pixels + 12, 0f);
			}
		}

		public void Refresh()
		{
			if (inner != null)
			{
				inner.RemoveAllChildren();
				TreeSystem.tree.Nodes.ForEach(n => inner.Append(new PassiveElement(n) { }));

				Recalculate();
			}
		}

		public override void SafeUpdate(GameTime gameTime)
		{
			if (Main.LocalPlayer.controlInv)
				visible = false;

			if (!Populated)
			{
				panel = new UIPanel();
				panel.Left.Set(LeftPadding, 0.5f);
				panel.Top.Set(TopPadding, 0.5f);
				panel.Width.Set(PanelWidth, 0);
				panel.Height.Set(PanelHeight, 0);
				panel.BackgroundColor = new Color(20, 15, 40) * 0.85f;
				Append(panel);

				if (fullscreen)
				{
					panel.Width.Set(0, 1f);
					panel.Height.Set(0, 1f);
					panel.Left.Set(0, 0f);
					panel.Top.Set(0, 0f);
				}

				inner = new InnerPanel();
				inner.Left.Set(0, 0);
				inner.Top.Set(0, 0);
				inner.Width.Set(0, 1f);
				inner.Height.Set(0, 1f);
				panel.Append(inner);

				closeButton = new UIImageButton(Assets.GUI.CloseButton);
				closeButton.Left.Set(-32, 1f);
				closeButton.Top.Set(-4, 0f);
				closeButton.Width.Set(38, 0);
				closeButton.Height.Set(38, 0);
				closeButton.OnLeftClick += (a, b) =>
				{
					if (fullscreen)
						IngameFancyUI.Close();

					visible = false;
				};
				closeButton.SetVisibility(1, 1);
				panel.Append(closeButton);

				fullscreenButton = new UIImageButton(Assets.GUI.fullscreenButton);
				fullscreenButton.Left.Set(-36 - 38, 1f);
				fullscreenButton.Top.Set(-4, 0f);
				fullscreenButton.Width.Set(38, 0);
				fullscreenButton.Height.Set(38, 0);
				fullscreenButton.OnLeftClick += (a, b) =>
				{
					if (!fullscreen)
					{
						inner.LineOff += panel.GetDimensions().Position();
						panel.Width.Set(0, 1f);
						panel.Height.Set(0, 1f);
						panel.Left.Set(0, 0f);
						panel.Top.Set(0, 0f);
						fullscreen = true;

						Recalculate();
						IngameFancyUI.OpenUIState(this);

						fullscreenButton.IsMouseHovering = false;
					}
					else
					{
						panel.Left.Set(LeftPadding, 0.5f);
						panel.Top.Set(TopPadding, 0.5f);
						panel.Width.Set(PanelWidth, 0);
						panel.Height.Set(PanelHeight, 0);
						fullscreen = false;

						Recalculate();
						inner.LineOff -= panel.GetDimensions().Position();
						Recalculate();
						IngameFancyUI.Close();
					}
				};
				fullscreenButton.SetVisibility(1, 1);
				panel.Append(fullscreenButton);

				editButton = new UIImageButton(Assets.GUI.CustomButton);
				editButton.Left.Set(-32, 1f);
				editButton.Top.Set(80, 0f);
				editButton.Width.Set(38, 0);
				editButton.Height.Set(38, 0);
				editButton.OnLeftClick += (a, b) =>
				{
					editing = !editing;

					if (editing)
					{
						if (!TreeSystem.hasCustomTree)
						{
							TreeSystem.SwitchToCustomTree();
							TreeSystem.hasCustomTree = true;
							Refresh();
						}

						StartEdit();
					}
					else
					{
						RemoveChild(selector);
						RemoveChild(costEditor);
						RemoveChild(exportButton);
						TreeSystem.tree.RegenrateConnections();
						TreeSystem.tree.RegenerateFlows();
						Refresh();
					}

					Recalculate();
					Main.NewText("Editing: " + editing);
				};
				editButton.SetVisibility(1, 1);
				panel.Append(editButton);

				TreeSystem.tree.Nodes.ForEach(n => inner.Append(new PassiveElement(n)));
				Recalculate();
				Populated = true;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			Texture2D tex = Assets.GUI.PassiveFrameTiny.Value;
			TreePlayer mp = Main.LocalPlayer.GetModPlayer<TreePlayer>();

			Vector2 umbraBasePos = panel.GetDimensions().ToRectangle().TopLeft() + new Vector2(32, 32);
			var umbraRect = new Rectangle((int)umbraBasePos.X - 16, (int)umbraBasePos.Y - 16, 132, 32);

			spriteBatch.Draw(tex, umbraBasePos, null, Color.White, 0, tex.Size() / 2f, 1, 0, 0);
			Utils.DrawBorderStringBig(spriteBatch, $"{mp.UmbraPoints}", umbraBasePos, mp.UmbraPoints > 0 ? new Color(210, 160, 255) : Color.Gray, 0.5f, 0.5f, 0.35f);
			Utils.DrawBorderStringBig(spriteBatch, Language.GetText("Mods.Umbra.GUI.Tree.Umbra").Value, umbraBasePos + new Vector2(24, 0), new Color(240, 210, 255), 0.5f, 0f, 0.35f);

			if (umbraRect.Contains(Main.MouseScreen.ToPoint()))
			{
				Tooltip.SetName(Language.GetText("Mods.Umbra.GUI.Tree.UmbraTooltipTitle").Format(mp.UmbraPoints));
				Tooltip.SetTooltip(Language.GetText("Mods.Umbra.GUI.Tree.UmbraTooltipDesc").Format(Math.Round(100 * UmbraDropNPC.UmbraChance, 2)));
			}

			Vector2 doomBasePos = panel.GetDimensions().ToRectangle().TopLeft() + new Vector2(32, 76);
			var doomRect = new Rectangle((int)doomBasePos.X - 16, (int)doomBasePos.Y - 16, 102, 32);

			spriteBatch.Draw(tex, doomBasePos, null, Color.White, 0, tex.Size() / 2f, 1, 0, 0);
			Utils.DrawBorderStringBig(spriteBatch, $"{TreeSystem.tree.difficulty}", doomBasePos, new Color(255, 160, 160), 0.5f, 0.5f, 0.35f);
			Utils.DrawBorderStringBig(spriteBatch, Language.GetText("Mods.Umbra.GUI.Tree.Doom").Value, doomBasePos + new Vector2(24, 0), new Color(255, 210, 210), 0.5f, 0f, 0.35f);

			if (doomRect.Contains(Main.MouseScreen.ToPoint()))
			{
				Tooltip.SetName(Language.GetText("Mods.Umbra.GUI.Tree.DoomTooltipTitle").Format(TreeSystem.tree.difficulty));

				if (DoomEffectsSystem.Inverted)
				{
					Tooltip.SetTooltip(Language.GetText("Mods.Umbra.GUI.Tree.DoomTooltipDescInverted").Format(
						Math.Round(DoomEffectsSystem.DoomValueMult * 100, 2),
						Math.Round(DoomEffectsSystem.LuckBonus, 2),
						Math.Round(100 * ((UmbraDropNPC.UmbraChance - 0.01f) / 0.01f), 2)
					));
				}
				else
				{
					Tooltip.SetTooltip(Language.GetText("Mods.Umbra.GUI.Tree.DoomTooltipDescDefault").Format(
						Math.Round(DoomEffectsSystem.DoomValueMult * 100, 2),
						Math.Round(DoomEffectsSystem.LuckBonus, 2),
						Math.Round(DoomEffectsSystem.DoubleLootChance * 100, 2),
						Math.Round(100 * ((UmbraDropNPC.UmbraChance - 0.01f) / 0.01f), 2)
					));
				}
			}

			if (closeButton.IsMouseHovering)
			{
				Tooltip.SetName(Language.GetTextValue("Mods.Umbra.GUI.Tree.CloseButton"));
				Tooltip.SetTooltip("");
			}

			if (fullscreenButton.IsMouseHovering)
			{
				Tooltip.SetName(Language.GetTextValue("Mods.Umbra.GUI.Tree.FullscreenButton"));
				Tooltip.SetTooltip("");
			}

			if (exportButton != null && exportButton.IsMouseHovering)
			{
				Tooltip.SetName(Language.GetTextValue("Mods.Umbra.GUI.Tree.ExportButton"));
				Tooltip.SetTooltip("");
			}

			if (editButton.IsMouseHovering)
			{
				Tooltip.SetName(Language.GetTextValue("Mods.Umbra.GUI.Tree.EditButton"));
				Tooltip.SetTooltip(Language.GetTextValue("Mods.Umbra.GUI.Tree.EditButtonDesc"));
			}

			if (editing)
			{
				Utils.DrawBorderString(spriteBatch, Language.GetText("Mods.Umbra.GUI.Tree.EditModeTooltip").Value, new Vector2(16, Main.screenHeight - 16), Color.White, 1f, 0f, 1f);
			}

			// Have to redraw since ingame fancy UI disables it normally
			if (fullscreen)
				UILoader.GetUIState<Tooltip>().Draw(spriteBatch);
		}

		public override void Recalculate()
		{
			if(editing)
			{
				if (fullscreen)
				{
					selector.Left.Set(32, 0f);
					selector.Top.Set(124, 0f);

					costEditor.Left.Set(32, 0f);
					costEditor.Top.Set(124 + selector.Height.Pixels + 12, 0f);

					exportButton.Left.Set(32 + costEditor.Width.Pixels + 12, 0f);
					exportButton.Top.Set(124 + selector.Height.Pixels + 12, 0f);
				}
				else
				{
					selector.Left.Set(LeftPadding - 220, 0.5f);
					selector.Top.Set(TopPadding, 0.5f);

					costEditor.Left.Set(LeftPadding - 220, 0.5f);
					costEditor.Top.Set(TopPadding + selector.Height.Pixels + 12, 0.5f);

					exportButton.Left.Set(LeftPadding - 220 + costEditor.Width.Pixels + 12, 0.5f);
					exportButton.Top.Set(TopPadding + selector.Height.Pixels + 12, 0.5f);
				}
			}

			base.Recalculate();
		}
	}

	internal class InnerPanel : SmartUIElement
	{
		private Vector2 start;
		private Vector2 root;
		public Vector2 LineOff = Vector2.One * 400;

		private Vector2 mouseDownAt;
		private bool movingEnabled;

		private bool moved;
		private bool freeze;

		private float zoom = 1;

		private PassiveElement movingPassive;

		private UIElement Panel => Parent;

		private Vector2 CenterPos => GetDimensions().Center() + LineOff - new Vector2(GetDimensions().Width, GetDimensions().Height) / 2f;

		public override void Draw(SpriteBatch spriteBatch)
		{
			Rectangle oldRect = spriteBatch.GraphicsDevice.ScissorRectangle;
			spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = true;

			var scissor = Panel.GetDimensions().ToRectangle();
			scissor.X = (int)(scissor.X * Main.UIScale);
			scissor.Y = (int)(scissor.Y * Main.UIScale);
			scissor.Width = (int)(scissor.Width * Main.UIScale);
			scissor.Height = (int)(scissor.Height * Main.UIScale);

			spriteBatch.GraphicsDevice.ScissorRectangle = scissor;

			var matrix = Matrix.CreateScale(zoom * Main.UIScale);

			spriteBatch.End();
			spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);

			foreach (PassiveEdge edge in TreeSystem.tree.Edges)
			{
				Passive start = TreeSystem.tree.nodesById[edge.Start];
				Passive end = TreeSystem.tree.nodesById[edge.End];

				float opacity = Math.Min(start.opacity, end.opacity);

				Texture2D chainTex = Assets.GUI.Link.Value;

				Color color = Color.DimGray;

				if (end.CanAllocate(Main.LocalPlayer) && start.active || start.CanAllocate(Main.LocalPlayer) && end.active)
					color = Color.Lerp(Color.Gray, Color.LightGray, (float)Math.Sin(Main.timeForVisualEffects * 0.1f) * 0.5f + 0.5f);

				if (end.active && start.active || Tree.editing)
					color = Color.White;

				for (float k = 0; k <= 1; k += 1 / (Vector2.Distance(start.TreePos, end.TreePos) / 16))
				{
					Vector2 pos = GetDimensions().Position() + Vector2.Lerp(start.TreePos * zoom, end.TreePos * zoom, k) + LineOff;
					Main.spriteBatch.Draw(chainTex, pos, null, color * opacity, start.TreePos.DirectionTo(end.TreePos).ToRotation(), chainTex.Size() / 2, zoom, 0, 0);
				}
			}

			foreach (PassiveEdge edge in TreeSystem.tree.flows)
			{
				Passive start = TreeSystem.tree.nodesById[edge.Start];
				Passive end = TreeSystem.tree.nodesById[edge.End];

				var rand = new Random(start.GetHashCode());

				Texture2D glow = Assets.GUI.GlowAlpha.Value;
				var glowColor = new Color(rand.Next(150, 200), 100, 255, 0);

				float dist = Vector2.Distance(start.TreePos, end.TreePos);

				for (int k = 0; k < (int)(dist / 20); k++)
				{
					float len = (80 + rand.Next(80)) * dist / 50f;
					float scale = 0.05f + rand.NextSingle() * 0.12f;

					float progress = ((float)Main.timeForVisualEffects + 15 * k) % len / (float)len;
					Vector2 pos = GetDimensions().Position() + Vector2.SmoothStep(start.TreePos * zoom, end.TreePos * zoom, progress) + LineOff;
					float scale2 = (float)Math.Sin(progress * 3.14f) * (0.4f - scale);
					spriteBatch.Draw(glow, pos, null, glowColor * scale2, 0, glow.Size() / 2f, scale2 * zoom, 0, 0);
				}
			}

			base.Draw(spriteBatch);
		
			if (Tree.editing)
				DrawEditing(spriteBatch);

			spriteBatch.End();
			spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);

			spriteBatch.GraphicsDevice.ScissorRectangle = oldRect;
			spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = false;

			if (!UILoader.GetUIState<Tree>().fullscreen)
			{
				var frame = Parent.GetDimensions().ToRectangle();
				frame.Inflate(4, 4);
				GUIHelper.DrawFancyFrame(spriteBatch, frame);
			}
		}

		/// <summary>
		/// Draws the extra elements for editing the tree
		/// </summary>
		/// <param name="spriteBatch"></param>
		public void DrawEditing(SpriteBatch spriteBatch)
		{
			// Draw the dots to guide the grid
			Texture2D tex = Assets.MagicPixel.Value;
			float scale = 2f / Assets.MagicPixel.Width() * zoom;
			Vector2 origin = Assets.MagicPixel.Size() / 2f;
			Color color = Color.White * 0.25f;

			for (int x = -100; x < 100; x++)
			{
				for (int y = -100; y < 100; y++)
				{
					Vector2 pos = GetDimensions().Position() + new Vector2(x * 16, y * 16) * zoom + LineOff;
					Main.spriteBatch.Draw(tex, pos, null, color, 0f, origin, scale, 0, 0);
				}
			}

			// Draw ghost cursor for selected passive
			if (!Children.Any(n => n.IsMouseHovering))
			{
				Vector2 rawAim = (Main.MouseScreen - GetDimensions().Position() - LineOff) / (16 * zoom);
				int aimX = (int)rawAim.X;
				int aimY = (int)rawAim.Y;

				Vector2 aim = new Vector2(aimX, aimY) * (16 * zoom) + GetDimensions().Position() + LineOff;

				if (Tree.toPlace != null)
					spriteBatch.Draw(Tree.toPlace.texture.Value, aim, null, Color.White * 0.5f, 0f, Tree.toPlace.texture.Size() / 2f, zoom, 0, 0);
			}
		}

		public override void SafeUpdate(GameTime gameTime)
		{
			if (Panel.IsMouseHovering)
				Main.LocalPlayer.mouseInterface = true;

			if (!moved && Main.mouseLeft && Children.Any(n => n.IsMouseHovering))
				freeze = true;

			if (Main.mouseLeft && Panel.IsMouseHovering && !freeze)
			{
				if (start == Vector2.Zero)
				{
					start = Main.MouseScreen;
					root = LineOff;
				}

				if (start != Main.MouseScreen)
					moved = true;

				LineOff = root + Main.MouseScreen - start;
				Recalculate();
			}
			else
			{
				start = Vector2.Zero;
			}

			if (mouseDownAt != default && Vector2.Distance(Main.MouseScreen, mouseDownAt) > 16)
				movingEnabled = true;

			if (Tree.editing && freeze && movingPassive != null && movingEnabled)
			{
				Vector2 rawAim = (Main.MouseScreen - GetDimensions().Position() - LineOff) / (16 * zoom);
				int aimX = (int)rawAim.X;
				int aimY = (int)rawAim.Y;

				movingPassive.passive.X = aimX;
				movingPassive.passive.Y = aimY;
				movingPassive.Left.Set(movingPassive.passive.TreePos.X - movingPassive.passive.Width / 2, 0);
				movingPassive.Top.Set(movingPassive.passive.TreePos.Y - movingPassive.passive.Height / 2, 0);
				movingPassive.root = new Vector2(movingPassive.Left.Pixels, movingPassive.Top.Pixels);
				Recalculate();
			}

			if (!Main.mouseLeft)
			{
				freeze = false;
				movingEnabled = false;
			}

			//Recalculate();
		}

		public override void Recalculate()
		{
			foreach (UIElement element in Elements)
			{
				if (element is PassiveElement ele)
				{
					ele.AdjustForScale(zoom);
					element.Left.Set(ele.scaledRoot.X + LineOff.X, 0);
					element.Top.Set(ele.scaledRoot.Y + LineOff.Y, 0);
				}
			}

			base.Recalculate();
		}

		public override void SafeClick(UIMouseEvent evt)
		{
			if (Tree.editing && Tree.toPlace != null && !Children.Any(n => n.IsMouseHovering) && !moved)
			{
				Vector2 rawAim = (Main.MouseScreen - GetDimensions().Position() - LineOff) / (16 * zoom);
				int aimX = (int)rawAim.X;
				int aimY = (int)rawAim.Y;

				Passive aimedAt = TreeSystem.tree.nodesByLocation.ContainsKey((aimX, aimY)) ? TreeSystem.tree.nodesByLocation[(aimX, aimY)] : null;

				if (aimedAt is null && TreeSystem.tree.CanInsert(Tree.toPlace))
				{
					int newID = TreeSystem.tree.Insert(Tree.toPlace, aimX, aimY);

					if (Tree.selected != null)
						TreeSystem.tree.Connect(Tree.selected.ID, newID);

					Tree.selected = TreeSystem.tree.nodesById[newID];

					UILoader.GetUIState<Tree>().Refresh();
				}
				else
				{
					SoundEngine.PlaySound(SoundID.Unlock);
				}
			}

			moved = false;
		}

		public override void SafeMouseDown(UIMouseEvent evt)
		{
			if (Tree.editing && !moved && !Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
			{
				var pElement = Children.FirstOrDefault(n => n.IsMouseHovering && n is PassiveElement) as PassiveElement;

				if (pElement is null)
					return;

				Passive aimedAt = pElement.passive;

				if (aimedAt != null)
				{
					Tree.selected = aimedAt;
					movingPassive = pElement;
				}
			}

			mouseDownAt = Main.MouseScreen;
		}

		public override void SafeMouseUp(UIMouseEvent evt)
		{
			if (movingPassive != null)
				movingPassive = null;
		}

		public override void SafeScrollWheel(UIScrollWheelEvent evt)
		{
			Vector2 scaledMouseDiff = (CenterPos - Main.MouseScreen) * (1f / zoom);

			zoom += evt.ScrollWheelValue / 1200f * zoom;

			if (zoom < 0.5f)
				zoom = 0.5f;

			if (zoom > 2f)
				zoom = 2f;

			Vector2 scaledMouseDiff2 = (CenterPos - Main.MouseScreen) * (1f / zoom);

			LineOff += (scaledMouseDiff - scaledMouseDiff2) * zoom;
			Recalculate();
		}
	}
}
