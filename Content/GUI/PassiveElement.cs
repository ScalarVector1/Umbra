using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Umbra.Core.Loaders.UILoading;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.GUI
{
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

				Tooltip.SetName(passive.DisplayName);
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
				ClickEditing();
			else
				ClickNormal();
		}

		/// <summary>
		/// Behavior that occurs on left click when not in edit mode
		/// </summary>
		public void ClickNormal()
		{
			if (passive.active)
			{
				passive.OnClick();
			}
			else
			{
				if (passive.TryAllocate(Main.LocalPlayer))
				{
					allocateFlashTime = 20;
					SoundEngine.PlaySound(SoundID.DD2_BookStaffCast.WithPitchOffset(-0.1f).WithVolume(0.5f));
					SoundEngine.PlaySound(SoundID.GuitarAm.WithVolume(0.2f).WithPitchOffset(-0.2f));
					SoundEngine.PlaySound(SoundID.DrumKick);
				}
			}
		}

		/// <summary>
		/// Behavior that occurs on left click when in edit mode
		/// </summary>
		public void ClickEditing()
		{
			if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) && Tree.selected != null)
			{
				TreeSystem.tree.Connect(Tree.selected.ID, passive.ID);
				return;
			}

			if (Tree.selected != passive)
				Tree.selected = passive;
		}

		public override void SafeRightClick(UIMouseEvent evt)
		{
			if (Tree.editing)
				RightClickEditing();
			else
				RightClickNormal();
		}

		/// <summary>
		/// Behavior that occurs on right click when not in edit mode
		/// </summary>
		public void RightClickNormal()
		{
			if (passive.TryDeallocate(Main.LocalPlayer))
			{
				deallocateFlashTime = 20;
				SoundEngine.PlaySound(SoundID.GuitarAm.WithVolume(0.2f).WithPitchOffset(0.1f));
				SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath);
			}
		}

		/// <summary>
		/// Behavior that occurs on right click when in edit mode
		/// </summary>
		public void RightClickEditing()
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
		}
	}
}
