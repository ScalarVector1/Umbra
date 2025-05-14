using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using Umbra.Content.GUI.FieldEditors;
using Umbra.Core;
using Umbra.Core.Loaders.UILoading;
using Umbra.Core.TreeSystem;
using static AssGen.Assets;

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

		private NodeTypeSelector selector;
		private IntEditor costEditor;

		public bool visible;
		public static bool editing;

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
		}

		public void Refresh()
		{
			inner.RemoveAllChildren();
			TreeSystem.tree.Nodes.ForEach(n => inner.Append(new PassiveElement(n) { }));
		}

		public override void Draw(SpriteBatch spriteBatch)
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

				closeButton = new UIImageButton(Assets.GUI.CloseButton);
				closeButton.Left.Set(LeftPadding + PanelWidth - 32 - 19, 0.5f);
				closeButton.Top.Set(TopPadding + 10, 0.5f);
				closeButton.Width.Set(38, 0);
				closeButton.Height.Set(38, 0);
				closeButton.OnLeftClick += (a, b) => visible = false;
				closeButton.SetVisibility(1, 1);
				Append(closeButton);

				exportButton = new UIImageButton(Assets.GUI.ExportButton);
				exportButton.Left.Set(LeftPadding + PanelWidth - 32 - 19, 0.5f);
				exportButton.Top.Set(TopPadding + 56, 0.5f);
				exportButton.Width.Set(38, 0);
				exportButton.Height.Set(38, 0);
				exportButton.OnLeftClick += (a, b) =>
				{
					TreeSystem.Export();
					Main.NewText("Exported tree!");
				};
				exportButton.SetVisibility(1, 1);
				Append(exportButton);

				editButton = new UIImageButton(Assets.GUI.ExportButton);
				editButton.Left.Set(LeftPadding + PanelWidth - 32 - 19, 0.5f);
				editButton.Top.Set(TopPadding + 102, 0.5f);
				editButton.Width.Set(38, 0);
				editButton.Height.Set(38, 0);
				editButton.OnLeftClick += (a, b) =>
				{
					editing = !editing;

					if (editing)
						StartEdit();
					else
					{
						RemoveChild(selector);
						RemoveChild(costEditor);
					}

					Main.NewText("Editing: " + editing);
				};
				editButton.SetVisibility(1, 1);
				Append(editButton);

				inner = new InnerPanel();
				inner.Left.Set(0, 0);
				inner.Top.Set(0, 0);
				inner.Width.Set(PanelWidth - 0, 0);
				inner.Height.Set(PanelHeight - 0, 0);
				panel.Append(inner);

				TreeSystem.tree.Nodes.ForEach(n => inner.Append(new PassiveElement(n)));
				Populated = true;
			}

			Recalculate();

			base.Draw(spriteBatch);

			Texture2D tex = Assets.GUI.PassiveFrameTiny.Value;
			TreePlayer mp = Main.LocalPlayer.GetModPlayer<TreePlayer>();

			Vector2 umbraBasePos = panel.GetDimensions().ToRectangle().TopLeft() + new Vector2(32, 32);
			Rectangle umbraRect = new Rectangle((int)umbraBasePos.X - 16, (int)umbraBasePos.Y - 16, 132, 32);

			spriteBatch.Draw(tex, umbraBasePos, null, Color.White, 0, tex.Size() / 2f, 1, 0, 0);
			Utils.DrawBorderStringBig(spriteBatch, $"{mp.UmbraPoints}", umbraBasePos, mp.UmbraPoints > 0 ? new Color(210, 160, 255) : Color.Gray, 0.5f, 0.5f, 0.35f);
			Utils.DrawBorderStringBig(spriteBatch, $"Umbra", umbraBasePos + new Vector2(24, 0), new Color(240, 210, 255), 0.5f, 0f, 0.35f);

			if(umbraRect.Contains(Main.MouseScreen.ToPoint()))
			{
				Tooltip.SetName($"{mp.UmbraPoints} Umbra Points Remaining");
				Tooltip.SetTooltip($"Umbra points are used to allocate nodes on the umbra tree. The chance to gain a point of umbra increases with your doom, currently you have a [c/AAAAFF:{Math.Round(100 * UmbraDropNPC.UmbraChance, 2)}%] chance to drop an umbra point on killing an enemy.");
			}

			Vector2 doomBasePos = panel.GetDimensions().ToRectangle().TopLeft() + new Vector2(32, 76);
			Rectangle doomRect = new Rectangle((int)doomBasePos.X - 16, (int)doomBasePos.Y - 16, 102, 32);

			spriteBatch.Draw(tex, doomBasePos, null, Color.White, 0, tex.Size() / 2f, 1, 0, 0);
			Utils.DrawBorderStringBig(spriteBatch, $"{TreeSystem.tree.difficulty}", doomBasePos, new Color(255, 160, 160), 0.5f, 0.5f, 0.35f);
			Utils.DrawBorderStringBig(spriteBatch, $"Doom", doomBasePos + new Vector2(24, 0), new Color(255, 210, 210), 0.5f, 0f, 0.35f);

			if (doomRect.Contains(Main.MouseScreen.ToPoint()))
			{
				Tooltip.SetName($"{TreeSystem.tree.difficulty} Doom");
				Tooltip.SetTooltip($"Doom is an approximate measure of the influence of the umbral tree on the game's difficulty. Additionally, it provides the following benefits:" +
					$"\n[c/AAAAFF:{TreeSystem.tree.difficulty * 0.1f}%] increased chungosity");
			}

			if (editing)
			{
				Utils.DrawBorderString(spriteBatch, 
					"Edit mode controls:\n" +
					"Left Click choice to select, Left click grid to add\n" +
					"Left Click node to select, Right click node to delete\n" +
					"Shift + Left Click node to connect to selected node\n" +
					"Shift + Right click node to disconnect from selected node\n" +
					"Right click choice to change selection to that node\n",
					new Vector2(100, 100), Color.White, 1);
			}
		}
	}

	internal class InnerPanel : SmartUIElement
	{
		private Vector2 start;
		private Vector2 root;
		private Vector2 LineOff = Vector2.One * 400;
		private bool moved;

		private UIElement Panel => Parent;

		private static TreePlayer TreePlayer => Main.LocalPlayer.GetModPlayer<TreePlayer>();
		private static TreeSystem TreeSystem => ModContent.GetInstance<TreeSystem>();

		public override void Draw(SpriteBatch spriteBatch)
		{
			Rectangle oldRect = spriteBatch.GraphicsDevice.ScissorRectangle;
			spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
			spriteBatch.GraphicsDevice.ScissorRectangle = Panel.GetDimensions().ToRectangle();

			spriteBatch.End();
			spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);

			foreach (PassiveEdge edge in TreeSystem.tree.Edges)
			{
				Passive start = TreeSystem.tree.nodesById[edge.Start];
				Passive end = TreeSystem.tree.nodesById[edge.End];

				Texture2D chainTex = Assets.GUI.Link.Value;

				Color color = Color.DimGray;

				if (end.CanAllocate(Main.LocalPlayer) && start.active)
					color = Color.Lerp(Color.Gray, Color.LightGray, (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.5f + 0.5f);

				if (end.active && start.active)
					color = Color.White;

				for (float k = 0; k <= 1; k += 1 / (Vector2.Distance(start.TreePos, end.TreePos) / 16))
				{
					Vector2 pos = GetDimensions().Position() + Vector2.Lerp(start.TreePos, end.TreePos, k) + LineOff;
					Main.spriteBatch.Draw(chainTex, pos, null, color, start.TreePos.DirectionTo(end.TreePos).ToRotation(), chainTex.Size() / 2, 1, 0, 0);
				}

				if (end.active && start.active)
				{
					var rand = new Random(edge.GetHashCode());

					Texture2D glow = Assets.GUI.GlowAlpha.Value;
					var glowColor = new Color(rand.Next(150, 200), 100, 255)
					{
						A = 0
					};

					for (int k = 0; k < 8; k++)
					{
						float dist = Vector2.Distance(start.TreePos, end.TreePos);
						float len = (40 + rand.Next(120)) * dist / 50;
						float scale = 0.05f + rand.NextSingle() * 0.15f;

						float progress = (Main.GameUpdateCount + 15 * k) % len / (float)len;
						Vector2 pos = GetDimensions().Position() + Vector2.SmoothStep(start.TreePos, end.TreePos, progress) + LineOff;
						float scale2 = (float)Math.Sin(progress * 3.14f) * (0.4f - scale);
						spriteBatch.Draw(glow, pos, null, glowColor * scale2, 0, glow.Size() / 2f, scale2, 0, 0);
					}
				}
			}

			base.Draw(spriteBatch);

			if (Tree.editing)
			{
				for (int x = -100; x < 100; x++)
				{
					for (int y = -100; y < 100; y++)
					{
						Vector2 pos = GetDimensions().Position() + new Vector2(x * 16, y * 16) + LineOff;
						Main.spriteBatch.Draw(Assets.GUI.Box.Value, pos, null, Color.White * 0.5f, 0f, Assets.GUI.Box.Size() / 2, 0.25f, 0, 0);
					}
				}
			}

			if (Tree.editing && !Children.Any(n => n.IsMouseHovering))
			{
				Vector2 rawAim = (Main.MouseScreen - GetDimensions().Position() - LineOff) / 16;
				int aimX = (int)rawAim.X;
				int aimY = (int)rawAim.Y;

				Vector2 aim = new Vector2(aimX, aimY) * 16 + GetDimensions().Position() + LineOff;

				if (Tree.toPlace != null)
					spriteBatch.Draw(Tree.toPlace.texture.Value, aim, null, Color.White * 0.5f, 0f, Tree.toPlace.texture.Size() / 2f, 1f, 0, 0);
			}

			spriteBatch.End();
			spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);

			spriteBatch.GraphicsDevice.ScissorRectangle = oldRect;
			spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = false;
		}

		public override void SafeUpdate(GameTime gameTime)
		{
			if (Panel.IsMouseHovering)
				Main.LocalPlayer.mouseInterface = true;

			if (Main.mouseLeft && Panel.IsMouseHovering)
			{
				if (start == Vector2.Zero)
				{
					start = Main.MouseScreen;
					root = LineOff;
				}

				if (start != Main.MouseScreen)
					moved = true;

				LineOff = root + Main.MouseScreen - start;
			}
			else
			{
				start = Vector2.Zero;
			}

			Recalculate();
		}

		public override void Recalculate()
		{
			foreach (UIElement element in Elements)
			{
				if (element is PassiveElement ele)
				{
					element.Left.Set(ele.root.X + LineOff.X, 0);
					element.Top.Set(ele.root.Y + LineOff.Y, 0);
				}
			}

			base.Recalculate();
		}

		public override void SafeClick(UIMouseEvent evt)
		{
			if (Tree.editing && Tree.toPlace != null && !Children.Any(n => n.IsMouseHovering) && !moved)
			{
				Vector2 rawAim = (Main.MouseScreen - GetDimensions().Position() - LineOff) / 16;
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
				else
				{
					Tree.selected = aimedAt;
				}
			}

			moved = false;
		}
	}

	internal class PassiveElement : SmartUIElement
	{
		private readonly Passive passive;
		public Vector2 root;

		private int allocateFlashTime;
		private int deallocateFlashTime;
		private int hoverTime;

		public PassiveElement(Passive passive)
		{
			this.passive = passive;
			Left.Set(passive.TreePos.X - passive.Width / 2, 0);
			Top.Set(passive.TreePos.Y - passive.Height / 2, 0);
			Width.Set(passive.Width, 0);
			Height.Set(passive.Height, 0);

			root = new Vector2(Left.Pixels, Top.Pixels);
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

					spriteBatch.Draw(glow, GetDimensions().Center(), null, glowColor * 0.5f, 0, glow.Size() / 2f, 1f, 0, 0);
					spriteBatch.Draw(star, GetDimensions().Center(), null, glowColor * 0.25f, 0, star.Size() / 2f, 1f, 0, 0);
				}

				if (IsMouseHovering)
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
			}

			if (hoverTime > 0)
			{
				Texture2D glow = Assets.GUI.GlowAlpha.Value;

				var glowColor = new Color(80, 80, 80, 0);

				if (passive.active || passive.CanAllocate(Main.LocalPlayer))
					glowColor = new(160, 80, 200, 0);

				float prog = hoverTime / 10f;
				spriteBatch.Draw(glow, GetDimensions().Center(), null, glowColor * prog, 0, glow.Size() / 2f, prog * (0.6f + passive.size * 0.1f), 0, 0);
			}

			passive.Draw(spriteBatch, GetDimensions().Center());

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

				spriteBatch.Draw(glow, GetDimensions().Center(), null, glowColor, 0, glow.Size() / 2f, 1 + (1f - prog), 0, 0);
				spriteBatch.Draw(star, GetDimensions().Center(), null, glowColor * 0.5f, 0, star.Size() / 2f, 1 + prog, 0, 0);

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

				spriteBatch.Draw(glow, GetDimensions().Center(), null, glowColor, 0, glow.Size() / 2f, 1 + (1f - prog), 0, 0);
				spriteBatch.Draw(star, GetDimensions().Center(), null, glowColor * 0.5f, 0, star.Size() / 2f, 1 + (1f - prog), 0, 0);

				deallocateFlashTime--;
			}

			if (IsMouseHovering)
			{
				string tip = passive.Tooltip;

				if (passive.difficulty > 0)
				{
					tip += $"\nDoom: [c/FFAAAA:{passive.difficulty} Doom]";
				}

				if (passive.Cost > 0)
				{
					if (!passive.active)
						tip += $"\nCost: [c/BB88FF:{passive.Cost} Umbra]";

					if (passive.active && passive.CanDeallocate(Main.LocalPlayer))
						tip += $"\nRefund: [c/BB88FF:{passive.Cost / 2} Umbra]";
				}

				Tooltip.SetName(passive.Name);
				Tooltip.SetTooltip(tip);
			}

			Recalculate();
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
					ModContent.GetInstance<TreeSystem>().tree.Connect(Tree.selected.ID, passive.ID);
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
					ModContent.GetInstance<TreeSystem>().tree.Edges.RemoveAll(n => n.Start == Tree.selected.ID && n.End == passive.ID);
					return;
				}

				if (Tree.selected == passive)
					Tree.selected = null;

				ModContent.GetInstance<TreeSystem>().tree.Remove(passive.ID);
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
		UIGrid choices = new();

		public override void OnInitialize()
		{
			choices.Left.Set(0, 0);
			choices.Top.Set(0, 0);
			choices.Width.Set(200, 0);
			choices.Height.Set(800, 0);
			Append(choices);

			foreach (Type type in Umbra.Instance.Code.GetTypes())
			{
				if (!type.IsAbstract && type.IsSubclassOf(typeof(Passive)))
				{
					var instance = Activator.CreateInstance(type) as Passive;
					choices.Add(new NodeChoice(instance));
				}
			}
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

				ModContent.GetInstance<TreeSystem>().tree.Nodes.Remove(Tree.selected);
				Tree.selected = passive.Clone();
				Tree.selected.ID = id;
				Tree.selected.X = x;
				Tree.selected.Y = y;
				Tree.selected.Cost = cost;
				ModContent.GetInstance<TreeSystem>().tree.Nodes.Add(Tree.selected);

				UILoader.GetUIState<Tree>().Refresh();
			}	
		}
	}
}
