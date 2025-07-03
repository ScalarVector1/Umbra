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
			selector.Height.Set(800, 0);
			selector.OnInitialize();
			Append(selector);

			costEditor = new("Cost", (a) =>
			{
				if (selected != null)
					selected.Cost = a;
			}, 0, () => selected?.Cost ?? 0, "Cost for the selected node");

			costEditor.Left.Set(LeftPadding - 420, 0.5f);
			costEditor.Top.Set(TopPadding, 0.5f);
			costEditor.OnInitialize();
			Append(costEditor);

			if (fullscreen)
			{
				selector.Left.Set(32, 0f);
				selector.Top.Set(124, 0f);

				costEditor.Left.Set(16, 0f);
				costEditor.Top.Set(136 + selector.Height.Pixels, 0f);
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
				fullscreenButton.OnLeftClick += (a, b) => {
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

				exportButton = new UIImageButton(Assets.GUI.ExportButton);
				exportButton.Left.Set(-32, 1f);
				exportButton.Top.Set(38, 0f);
				exportButton.Width.Set(38, 0);
				exportButton.Height.Set(38, 0);
				exportButton.OnLeftClick += (a, b) =>
				{
					TreeSystem.Export();
					Main.NewText("Exported tree!");
				};
				exportButton.SetVisibility(1, 1);
				panel.Append(exportButton);

				editButton = new UIImageButton(Assets.GUI.ExportButton);
				editButton.Left.Set(-32, 1f);
				editButton.Top.Set(80, 0f);
				editButton.Width.Set(38, 0);
				editButton.Height.Set(38, 0);
				editButton.OnLeftClick += (a, b) =>
				{
					editing = !editing;

					if (editing)
					{
						StartEdit();
					}
					else
					{
						RemoveChild(selector);
						RemoveChild(costEditor);
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

			if (exportButton.IsMouseHovering)
			{
				Tooltip.SetName("Export Tree [c/FF0000: DEBUG TOOL]");
				Tooltip.SetTooltip("use at own risk");
			}

			if (editButton.IsMouseHovering)
			{
				Tooltip.SetName("Edit Tree [c/FF0000: DEBUG TOOL]");
				Tooltip.SetTooltip("use at own risk");
			}

			if (editing)
			{
				Utils.DrawBorderString(spriteBatch, Language.GetText("Mods.Umbra.GUI.Tree.EditModeTooltip").Value, new Vector2(16, Main.screenHeight - 16), Color.White, 1f, 0f, 1f);
			}

			// Have to redraw since ingame fancy UI disables it normally
			if (fullscreen)
				UILoader.GetUIState<Tooltip>().Draw(spriteBatch);
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
			{
				for (int x = -100; x < 100; x++)
				{
					for (int y = -100; y < 100; y++)
					{
						Vector2 pos = GetDimensions().Position() + new Vector2(x * 16, y * 16) * zoom + LineOff;
						Main.spriteBatch.Draw(Assets.GUI.Box.Value, pos, null, Color.White * 0.25f, 0f, Assets.GUI.Box.Size() / 2, 0.2f * zoom, 0, 0);
					}
				}
			}

			if (Tree.editing && !Children.Any(n => n.IsMouseHovering))
			{
				Vector2 rawAim = (Main.MouseScreen - GetDimensions().Position() - LineOff) / (16 * zoom);
				int aimX = (int)rawAim.X;
				int aimY = (int)rawAim.Y;

				Vector2 aim = new Vector2(aimX, aimY) * (16 * zoom) + GetDimensions().Position() + LineOff;

				if (Tree.toPlace != null)
					spriteBatch.Draw(Tree.toPlace.texture.Value, aim, null, Color.White * 0.5f, 0f, Tree.toPlace.texture.Size() / 2f, zoom, 0, 0);
			}

			spriteBatch.End();
			spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);

			spriteBatch.GraphicsDevice.ScissorRectangle = oldRect;
			spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = false;

			if (!UILoader.GetUIState<Tree>().fullscreen)
			{
				var frame = Parent.GetDimensions().ToRectangle();
				frame.Inflate(4, 4);
				DrawFrame(spriteBatch, frame);
			}
		}

		public void DrawFrame(SpriteBatch spriteBatch, Rectangle target)
		{
			Texture2D tex = Assets.GUI.Frame.Value;

			Color color = Color.White;

			var sourceCorner = new Rectangle(0, 4, 12, 12);
			var sourceEdge = new Rectangle(12, 4, 8, 12);

			spriteBatch.Draw(tex, new Rectangle(target.X + 10, target.Y, target.Width - 32, 6), new Rectangle(10, 6, 2, 6), color, 0, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + 10, 6, target.Height - 24), new Rectangle(0, 16, 6, 2), color, 0, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X + 32, target.Y + target.Height - 6, target.Width - 42, 6), new Rectangle(42, tex.Height - 12, 2, 6), color, 0, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X + target.Width - 6, target.Y + 10, 6, target.Height - 20), new Rectangle(tex.Width - 6, 16, 6, 2), color, 0, Vector2.Zero, 0, 0);

			spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 10, 10), new Rectangle(0, 6, 10, 10), color, 0, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X + target.Width - 32, target.Y - 6, 32, 20), new Rectangle(tex.Width - 32, 0, 32, 20), color, 0, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X + target.Width - 10, target.Y + target.Height - 10, 10, 10), new Rectangle(tex.Width - 10, tex.Height - 16, 10, 10), color, 0, Vector2.Zero, 0, 0);
			spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height - 14, 32, 20), new Rectangle(0, tex.Height - 20, 32, 20), color, 0, Vector2.Zero, 0, 0);
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

				if (aimedAt is null)
				{
					int newID = TreeSystem.tree.Insert(Tree.toPlace, aimX, aimY);

					if (Tree.selected != null)
						TreeSystem.tree.Connect(Tree.selected.ID, newID);

					Tree.selected = TreeSystem.tree.nodesById[newID];

					UILoader.GetUIState<Tree>().Refresh();
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
			Vector2 scaledMouseDiff = (CenterPos - Main.MouseScreen) * (1f/zoom);

			zoom += evt.ScrollWheelValue / 1200f * zoom;

			if (zoom < 0.5f)
				zoom = 0.5f;

			if (zoom > 2f)
				zoom = 2f;

			Vector2 scaledMouseDiff2 = (CenterPos - Main.MouseScreen) * (1f/zoom);

			LineOff += (scaledMouseDiff - scaledMouseDiff2) * zoom;
			Recalculate();
		}
	}

	internal class PassiveElement : SmartUIElement
	{
		public Passive passive;
		public Vector2 root;
		public Vector2 scaledRoot;

		private int allocateFlashTime;
		private int deallocateFlashTime;
		private int hoverTime;
		private float scale = 1f;

		public PassiveElement(Passive passive)
		{
			this.passive = passive;
			Left.Set(passive.TreePos.X - passive.Width / 2, 0);
			Top.Set(passive.TreePos.Y - passive.Height / 2, 0);
			Width.Set(passive.Width, 0);
			Height.Set(passive.Height, 0);

			root = new Vector2(Left.Pixels, Top.Pixels);
			scaledRoot = root;
		}

		public void AdjustForScale(float scale)
		{
			scaledRoot = root * scale;
			Width.Set(passive.Width * scale, 0);
			Height.Set(passive.Height * scale, 0);
			this.scale = scale;
		}

		public override void SafeUpdate(GameTime gameTime)
		{
			if (IsMouseHovering && hoverTime < 10)
				hoverTime++;

			if (!IsMouseHovering && hoverTime > 0)
				hoverTime--;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Tree.editing)
			{
				if (Tree.selected == passive)
				{
					Texture2D glow = Assets.GUI.GlowAlpha.Value;
					Texture2D star = Assets.GUI.StarAlpha.Value;

					var glowColor = new Color(120, 255, 120)
					{
						A = 0
					};

					spriteBatch.Draw(glow, GetDimensions().Center(), null, glowColor * 0.5f, 0, glow.Size() / 2f, scale, 0, 0);
					spriteBatch.Draw(star, GetDimensions().Center(), null, glowColor * 0.25f, 0, star.Size() / 2f, scale, 0, 0);
				}

				if (IsMouseHovering)
				{
					Texture2D glow = Assets.GUI.GlowAlpha.Value;
					Texture2D star = Assets.GUI.StarAlpha.Value;

					var glowColor = new Color(160, 160, 60)
					{
						A = 0
					};

					spriteBatch.Draw(glow, GetDimensions().Center(), null, glowColor * 0.5f, 0, glow.Size() / 2f, scale, 0, 0);
					spriteBatch.Draw(star, GetDimensions().Center(), null, glowColor * 0.25f, 0, star.Size() / 2f, scale, 0, 0);
				}
			}

			if (hoverTime > 0)
			{
				Texture2D glow = Assets.GUI.GlowAlpha.Value;

				var glowColor = new Color(80, 80, 80, 0);

				if (passive.active || passive.CanAllocate(Main.LocalPlayer))
					glowColor = new(160, 80, 200, 0);

				float prog = hoverTime / 10f;
				spriteBatch.Draw(glow, GetDimensions().Center(), null, glowColor * prog, 0, glow.Size() / 2f, prog * scale * (0.6f + passive.size * 0.1f), 0, 0);
			}

			passive.Draw(spriteBatch, GetDimensions().Center(), scale);

			if (Tree.editing)
			{
				Utils.DrawBorderString(spriteBatch, passive.Cost.ToString(), GetDimensions().ToRectangle().Center(), Color.Lavender);
			}

			if (allocateFlashTime > 0)
			{
				Texture2D glow = Assets.GUI.GlowAlpha.Value;
				Texture2D star = Assets.GUI.StarAlpha.Value;

				float prog = allocateFlashTime / 20f;

				var glowColor = new Color(180, 120, 255)
				{
					A = 0
				};

				glowColor *= prog * 0.5f;

				spriteBatch.Draw(glow, GetDimensions().Center(), null, glowColor, 0, glow.Size() / 2f, scale * (1 + (1f - prog)), 0, 0);
				spriteBatch.Draw(star, GetDimensions().Center(), null, glowColor * 0.5f, 0, star.Size() / 2f, scale * (1 + prog), 0, 0);

				allocateFlashTime--;
			}

			if (deallocateFlashTime > 0)
			{
				Texture2D glow = Assets.GUI.GlowAlpha.Value;
				Texture2D star = Assets.GUI.StarAlpha.Value;

				float prog = deallocateFlashTime / 20f;

				var glowColor = new Color(120, 80, 200)
				{
					A = 0
				};

				glowColor *= prog * 0.5f;

				spriteBatch.Draw(glow, GetDimensions().Center(), null, glowColor, 0, glow.Size() / 2f, scale * (1 + (1f - prog)), 0, 0);
				spriteBatch.Draw(star, GetDimensions().Center(), null, glowColor * 0.5f, 0, star.Size() / 2f, scale * (1 + (1f - prog)), 0, 0);

				deallocateFlashTime--;
			}

			if (IsMouseHovering)
			{
				string tip = passive.Tooltip;

				if (passive.difficulty > 0)
					tip += "\n" + Language.GetText("Mods.Umbra.GUI.Node.Doom").Format(passive.difficulty);

				if (passive.Cost > 0)
				{
					if (!passive.active)
						tip += "\n" + Language.GetText("Mods.Umbra.GUI.Node.Cost").Format(passive.Cost);

					if (passive.active && passive.CanDeallocate(Main.LocalPlayer))
						tip += "\n" + Language.GetText("Mods.Umbra.GUI.Node.Refund").Format((int)Math.Ceiling(passive.Cost / 2f));
				}

				Tooltip.SetName(passive.Name);
				Tooltip.SetTooltip(tip);
			}
		}

		public override void SafeMouseOver(UIMouseEvent evt)
		{
			if (passive.active || passive.CanAllocate(Main.LocalPlayer))
				SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt.WithPitchOffset(0.5f).WithVolume(0.5f));
			else
				SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt.WithPitchOffset(-0.5f).WithVolume(0.25f));
		}

		public override void SafeClick(UIMouseEvent evt)
		{
			if (Tree.editing)
			{
				if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) && Tree.selected != null)
				{
					TreeSystem.tree.Connect(Tree.selected.ID, passive.ID);
					return;
				}

				if (Tree.selected != passive)
					Tree.selected = passive;
				//else
				//Tree.selected = null;

				return;
			}

			if (passive.TryAllocate(Main.LocalPlayer))
			{
				allocateFlashTime = 20;
				SoundEngine.PlaySound(SoundID.DD2_BookStaffCast.WithPitchOffset(-0.1f).WithVolume(0.5f));
				SoundEngine.PlaySound(SoundID.GuitarAm.WithVolume(0.2f).WithPitchOffset(-0.2f));
				SoundEngine.PlaySound(SoundID.DrumKick);
			}
		}

		public override void SafeRightClick(UIMouseEvent evt)
		{
			if (Tree.editing)
			{
				if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) && Tree.selected != null)
				{
					TreeSystem.tree.Disconnect(Tree.selected.ID, passive.ID);
					return;
				}

				if (Tree.selected == passive)
					Tree.selected = null;

				TreeSystem.tree.Remove(passive.ID);
				UILoader.GetUIState<Tree>().Refresh();
				return;
			}

			if (passive.TryDeallocate(Main.LocalPlayer))
			{
				deallocateFlashTime = 20;
				SoundEngine.PlaySound(SoundID.GuitarAm.WithVolume(0.2f).WithPitchOffset(0.1f));
				SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath);
			}
		}
	}

	internal class NodeTypeSelector : SmartUIElement
	{
		readonly UIGrid choices = new();
		readonly UIScrollbar scroll = new();

		public override void OnInitialize()
		{
			var panel = new UIPanel();
			panel.Left.Set(0, 0);
			panel.Top.Set(0, 0);
			panel.Width.Set(0, 1f);
			panel.Height.Set(0, 1f);
			panel.BackgroundColor = new Color(0f, 0f, 0f, 0.4f);
			Append(panel);

			scroll.Left.Set(-24, 0);
			scroll.Top.Set(0, 0);
			scroll.Width.Set(32, 0);
			scroll.Height.Set(800, 0);
			Append(scroll);

			choices.Left.Set(12, 0);
			choices.Top.Set(12, 0);
			choices.Width.Set(200, 0);
			choices.Height.Set(800, 0);
			choices.SetScrollbar(scroll);
			Append(choices);

			foreach (Type type in Umbra.Instance.Code.GetTypes())
			{
				if (!type.IsAbstract && type.IsSubclassOf(typeof(Passive)))
				{
					var instance = Activator.CreateInstance(type) as Passive;
					choices.Add(new NodeChoice(instance));
				}
			}

			choices.UpdateOrder();
		}
	}

	internal class NodeChoice : SmartUIElement
	{
		public Passive passive;

		public NodeChoice(Passive passive)
		{
			this.passive = passive;

			Width.Set(40, 0);
			Height.Set(40, 0);
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
				string cost = $"\nCost: [c/BB88FF:{passive.Cost} Umbra]";

				Tooltip.SetName(passive.Name);
				Tooltip.SetTooltip(passive.Tooltip + cost);
			}

			base.Draw(spriteBatch);
		}

		public override void SafeClick(UIMouseEvent evt)
		{
			Tree.toPlace = passive;
		}

		public override void SafeRightClick(UIMouseEvent evt)
		{
			if (Tree.selected != null)
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

				return passive.Name.CompareTo(choice.passive.Name);
			}

			return base.CompareTo(obj);
		}
	}
}
