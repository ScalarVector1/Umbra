using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class Snowflakes : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.Snowflakes;
			difficulty = 30;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			if (!npc.boss && Main.rand.NextBool(10))
				npc.GetGlobalNPC<SnowflakeNPC>().active = true;
		}
	}

	internal class SnowflakeNPC : GlobalNPC
	{
		public bool active;

		public override bool InstancePerEntity => true;

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Microsoft.Xna.Framework.Color drawColor)
		{
			if (active)
			{
				ModContent.GetInstance<PixelationSystem>().QueueRenderAction("OverPlayers", () =>
				{
					var tex = Assets.Masks.GlowTrailNoEnd.Value;

					var color = new Color(160, 220, 255, 0) * (0.25f + MathF.Sin(Main.GameUpdateCount * 0.1f) * 0.15f);

					for (int k = 0; k < 8; k++)
					{
						float rot = k / 8f * 6.28f;
						Main.EntitySpriteDraw(tex, npc.Center - Main.screenPosition, null, color, rot, new Vector2(tex.Width / 4, tex.Height / 2), new Vector2(0.1f, 0.05f), 0);
					}
				});
			}
		}

		public override void OnKill(NPC npc)
		{
			if (active)
			{
				Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<SnowflakeProjectile>(), npc.damage, 0, Main.myPlayer);
			}
		}
	}

	internal class SnowflakeProjectile : ModProjectile
	{
		public float Prog => MathF.Sin(Projectile.timeLeft / 60f * 3.14f);

		public override string Texture => "Umbra/Assets/Invisible";

		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 60;
			Projectile.hostile = false;
			Projectile.tileCollide = false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			ModContent.GetInstance<PixelationSystem>().QueueRenderAction("OverPlayers", () =>
			{
				var tex = Assets.Masks.GlowTrailNoEnd.Value;
				var time = 60 - Projectile.timeLeft;

				for (int k = 0; k < 8; k++)
				{
					float rot = k / 8f * 6.28f;

					float min = k * 2;
					float max = 60 - k * 2;
					float localProg = Math.Clamp((time - min) / max, 0, 1);

					var opacity = Math.Min(localProg * 2, 1f);
					var scale = 1f;

					if (time > 50)
					{
						opacity = 1f - Helpers.Eases.EaseCircularOut((time - 50) / 10f);
						scale = 1f + Helpers.Eases.EaseCircularOut((time - 50) / 10f) * 0.4f;
					}

					var color = new Color(50 + (int)(50 * localProg), 220, 255, 0) * opacity * 0.25f;

					Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, color, rot, new Vector2(0, tex.Height / 2), new Vector2(1.5f * scale, 0.1f), 0);
					Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * opacity * 0.5f, rot, new Vector2(0, tex.Height / 2), new Vector2(1f * scale, 0.05f), 0);
				}

				var coreOpacity = Math.Min(time / 30f, 1f);

				if (time > 50)
					coreOpacity = 1f - Helpers.Eases.EaseCircularOut((time - 50) / 10f);

				var core = Assets.Masks.GlowAlpha.Value;
				Main.EntitySpriteDraw(core, Projectile.Center - Main.screenPosition, null, new Color(200, 220, 255, 0) * coreOpacity, 0, core.Size() / 2f, 1, 0 ,0);
			});

			return false;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, Projectile.Center);

			for (int k = 0; k < 8; k++)
			{
				float rot = k / 8f * 6.28f;
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedBy(rot) * 10, ModContent.ProjectileType<SnowflakeShard>(), Projectile.damage, 0, Main.myPlayer);
			}
		}
	}

	internal class SnowflakeShard : ModProjectile
	{
		public override string Texture => "Umbra/Assets/Projectiles/SnowflakeShard";

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 40;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.coldDamage = true; // hey its here, might as well
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			Projectile.velocity *= 0.99f;

			Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.15f, 0.24f));
			Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2, 2), ModContent.DustType<Dusts.PixelatedEmber>(), -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.3f), 0, new Color(0.5f, 0.8f, 0.8f, 0f), 0.1f);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.Frostburn, 180);
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);

			for (int k = 0; k < 10; k++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice);
				Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2, 2), ModContent.DustType<Dusts.PixelatedEmber>(), Projectile.velocity.RotatedBy(1f) * k / 10f * 0.75f, 0, new Color(0.5f, 0.8f, 0.8f, 0f), 0.2f * (1f - k / 10f));
				Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2, 2), ModContent.DustType<Dusts.PixelatedEmber>(), Projectile.velocity.RotatedBy(-1f) * k / 10f * 0.75f, 0, new Color(0.5f, 0.8f, 0.8f, 0f), 0.2f * (1f - k / 10f));
				Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2, 2), ModContent.DustType<Dusts.PixelatedEmber>(), Projectile.velocity.RotatedBy(0f) * k / 10f * 0.75f, 0, new Color(0.5f, 0.8f, 0.8f, 0f), 0.2f * (1f - k / 10f));
			}
		}
	}
}
