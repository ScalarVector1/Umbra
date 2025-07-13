using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;
using Umbra.Core;
using Umbra.Helpers;
using static AssGen.Assets;
using Umbra.Core.Loaders;
using Terraria.Audio;
using Terraria.ID;

namespace Umbra.Content.Passives.Large
{
	internal class FlyingDaggers : Passive
	{
		public override void Load(Mod mod)
		{
			DodgeNPC.OnDodge += DaggerOnDodge;
		}

		public override void SetDefaults()
		{
			texture = Assets.Passives.FlyingDaggers;
			difficulty = 40;
			size = 1;
		}

		private void DaggerOnDodge(NPC nPC, NPC.HitInfo info)
		{
			if (TreeSystem.tree.AnyActive<FlyingDaggers>())
			{
				SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing.WithPitchOffset(0.5f), nPC.Center);
				Projectile.NewProjectile(nPC.GetSource_FromThis(), nPC.Center, Vector2.UnitX.RotatedByRandom(6.28f) * 5f, ModContent.ProjectileType<DodgeDagger>(), DifficultyHelper.GetProjectileDamage(nPC.damage / 2), 0.5f);
			}
		}
	}

	internal class DodgeDagger : ModProjectile
	{
		private List<Vector2> cache;
		private Trail trail;

		public override string Texture => "Umbra/Assets/Invisible";

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 300;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			Projectile.velocity *= 1.01f;

			if (Main.rand.NextBool(7))
			{
				Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2, 2), ModContent.DustType<Dusts.PixelatedEmber>(), -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.3f), 0, new Color(0.2f, 0.6f, 0.3f, 0f), Main.rand.NextFloat(0.05f, 0.1f));
			}

			if (!Main.dedServ)
			{
				ManageCaches();
				ManageTrail();
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			ModContent.GetInstance<PixelationSystem>().QueueRenderAction("OverPlayers", () =>
			{
				Effect effect = ShaderLoader.GetShader("RepeatingChain").Value;

				if (effect != null)
				{
					var world = Matrix.CreateTranslation(-Main.screenPosition.ToVector3());
					Matrix view = Matrix.Identity;
					var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

					effect.Parameters["alpha"].SetValue(1f);
					effect.Parameters["repeats"].SetValue(1f);
					effect.Parameters["transformMatrix"].SetValue(world * view * projection);
					effect.Parameters["sampleTexture"].SetValue(Assets.Trails.GlowTrail.Value);
					trail?.Render(effect);
				}
			});

			var tex = Assets.Projectiles.DodgeDagger.Value;
			Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(0.6f, 1.0f, 0.7f, 0f), Projectile.rotation + 1.57f, tex.Size() / 2f, Projectile.scale * 2f, 0, 0);
			Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + 1.57f, tex.Size() / 2f, Projectile.scale, 0, 0);

			return false;
		}

		protected void ManageCaches()
		{
			if (cache == null)
			{
				cache = new List<Vector2>();

				for (int i = 0; i < 30; i++)
				{
					cache.Add(Projectile.Center);
				}
			}

			cache.Add(Projectile.Center);

			while (cache.Count > 30)
			{
				cache.RemoveAt(0);
			}
		}

		protected void ManageTrail()
		{
			if (trail is null || trail.IsDisposed)
			{
				trail = new Trail(Main.instance.GraphicsDevice, 30, new NoTip(), factor => factor * 8, factor =>
				{
					float alpha = 1f;

					if (factor.X == 1)
						alpha = 0;

					if (Projectile.timeLeft < 20)
						alpha *= Projectile.timeLeft / 20f;

					alpha *= factor.X;

					Color color = new Color(0.2f, 0.3f + factor.X * 0.5f, 0.3f, 0) * (Projectile.timeLeft < 30 ? (Projectile.timeLeft / 30f) : 1);

					return color * alpha;
				});
			}

			trail.Positions = cache.ToArray();
			trail.NextPosition = Projectile.Center + Projectile.velocity;
		}
	}
}
