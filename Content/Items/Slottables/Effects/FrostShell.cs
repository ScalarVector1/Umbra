using StarlightRiver.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Umbra.Core.PixelationSystem;

namespace Umbra.Content.Items.Slottables.Effects
{
	internal class FrostShell : SlottableEffect
	{
		public override void BuffPlayer(Player player)
		{
			player.GetModPlayer<FrostShellPlayer>().frostShellActive = true;
		}
	}

	internal class FrostShellPlayer : ModPlayer
	{
		public int shellHitpointsMax => Player.statDefense;

		public bool frostShellActive;

		public int shellHitpoints;
		public int shellTimer;

		public int regenTimer;

		public override void Load()
		{
			On_Main.DrawPlayers_AfterProjectiles += QueueBubbles;
		}

		public override void PostUpdateEquips()
		{
			if (!frostShellActive)
			{
				shellTimer = 600;
				shellHitpoints = 0;
				return;
			}

			Player.AddBuff(ModContent.BuffType<FrostShellBuff>(), 2);

			if (shellHitpoints > shellHitpointsMax)
				shellHitpoints = shellHitpointsMax;

			if (shellTimer <= 0 && regenTimer <= 0 && shellHitpoints < shellHitpointsMax)
			{
				shellHitpoints++;
				regenTimer = 5;
			}

			if (shellTimer > 0)
				shellTimer--;

			if (regenTimer > 0)
				regenTimer--;
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (frostShellActive && shellHitpoints > 0)
			{
				modifiers.FinalDamage.Flat -= shellHitpoints;
				shellHitpoints = 0;
				shellTimer = 600;

				SoundEngine.PlaySound(SoundID.Shatter.WithPitchOffset(-0.2f), Player.Center);

				for(int k = 0; k < 50; k++)
				{
					Vector2 off = Main.rand.NextVector2Circular(1, 1);
					Dust.NewDustPerfect(Player.Center + off * 25, DustID.Ice, off * Main.rand.NextFloat(6));
				}
			}
		}

		private void QueueBubbles(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
		{
			orig(self);

			foreach(Player player in Main.ActivePlayers)
			{
				FrostShellPlayer mp = player.GetModPlayer<FrostShellPlayer>();

				if (mp.frostShellActive)
				{
					var tex = Assets.Masks.RingGlow.Value;
					var glow = Assets.Masks.RingGlowInner.Value;
					var glow2 = Assets.Masks.RingGlowInnerTwo.Value;

					var shine = Assets.Masks.GlowSoftAlpha.Value;
					var back = Assets.Masks.Glow.Value;

					Vector2 drawPos = player.MountedCenter + Vector2.UnitY * player.gfxOffY - Main.screenPosition;
					float opacity = mp.shellHitpoints / (float)mp.shellHitpointsMax;

					ModContent.GetInstance<PixelationSystem>().QueueRenderAction("OverPlayers", () =>
					{
						Main.spriteBatch.Draw(tex, drawPos, null, new Color(30, 50, 50, 0) * opacity, 0, tex.Size() / 2f, 0.4f, 0, 0);
						Main.spriteBatch.Draw(glow, drawPos, null, new Color(20, 20, 60, 0) * opacity, 0, glow.Size() / 2f, 0.4f, 0, 0);
						Main.spriteBatch.Draw(glow2, drawPos, null, new Color(60, 100, 70, 0) * opacity, 0, glow2.Size() / 2f, 0.4f, 0, 0);

						Main.spriteBatch.Draw(shine, drawPos + new Vector2(10, -10), null, new Color(140, 160, 160, 0) * opacity, 0, shine.Size() / 2f, 0.7f, 0, 0);

						Random rand = new(91128);
						for (int k = 0; k < 80; k++)
						{
							int maxTime = rand.Next(90, 190);
							DrawStar(drawPos, (k * 3f + Main.GameUpdateCount) % maxTime / maxTime, 0.1f + (float)rand.NextDouble() * 0.8f, 28, 0.05f + 0.15f * (float)rand.NextDouble(), opacity);
						}
					});
				}
			}
		}

		public void DrawStar(Vector2 center, float time, float angle, float radius, float scale, float opacity)
		{
			var shine = Assets.Masks.GlowSoftAlpha.Value;

			float baseProg = 0.5f - MathF.Cos(time * 3.14f) * 0.5f;
			float baseScale = MathF.Sin(time * 3.14f);

			float angleProg = MathF.Cos(angle * 3.14f);
			float angleScale = MathF.Sin(angle * 3.14f);

			Vector2 start = center + new Vector2(-radius, radius);
			Vector2 end = center + new Vector2(radius, -radius);
			Vector2 mid = Vector2.One * angleProg * radius;
			Vector2 baseLerped = Vector2.Lerp(start, end, baseProg);
			Vector2 final = baseLerped + mid * baseScale;

			Main.spriteBatch.Draw(shine, final, null, new Color(200, 220, 220, 0) * baseScale * angleScale * opacity, 0, shine.Size() / 2f, scale * baseScale * angleScale, 0, 0);
		}

		public override void ResetEffects()
		{
			frostShellActive = false;
		}
	}

	internal class FrostShellBuff : ModBuff
	{
		public override string Texture => "Umbra/Assets/Buffs/FrostShellBuff";

		public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
		{
			var hitpoints = Main.LocalPlayer.GetModPlayer<FrostShellPlayer>().shellHitpoints;

			Color color = hitpoints == 0 ? Color.Gray : Color.White;

			if (hitpoints == Main.LocalPlayer.GetModPlayer<FrostShellPlayer>().shellHitpointsMax)
				color = Color.SkyBlue;

			Utils.DrawBorderString(spriteBatch, hitpoints.ToString(), drawParams.Position + Vector2.One * 16, color, 1, 0.5f, 0.5f);
		}
	}

	internal class FrostShellLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition()
		{
			return PlayerDrawLayers.BeforeFirstVanillaLayer;
		}

		public override void Draw(ref PlayerDrawSet drawInfo)
		{
			Player player = drawInfo.drawPlayer;
			FrostShellPlayer mp = player.GetModPlayer<FrostShellPlayer>();

			if (mp.frostShellActive && drawInfo.shadow == 0)
			{
				var back = Assets.Masks.Glow.Value;
				Vector2 drawPos = drawInfo.Center - Main.screenPosition;
				float opacity = mp.shellHitpoints / (float)mp.shellHitpointsMax;

				drawInfo.DrawDataCache.Add(new DrawData(back, drawPos, null, Color.Black * opacity * 0.5f, 0, back.Size() / 2f, 1.55f, 0, 0));

				return;
			}
		}
	}
}
