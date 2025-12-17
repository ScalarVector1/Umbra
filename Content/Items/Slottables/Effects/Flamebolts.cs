using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Umbra.Core.Loaders;
using Umbra.Core.PixelationSystem;
using Umbra.Helpers;

namespace Umbra.Content.Items.Slottables.Effects
{
	internal class Flamebolts : SlottableEffect
	{
		public override void BuffPlayer(Player player)
		{
			player.GetModPlayer<FlameboltsPlayer>().flameboltsActive = true;
		}
	}

	public class FlameboltsPlayer : ModPlayer
	{
		public bool flameboltsActive;
		public int flameboltCooldown;

		public override void PreUpdate()
		{
			if (flameboltCooldown > 0)
				flameboltCooldown--;
		}

		public override void ResetEffects()
		{
			flameboltsActive = false;
		}
	}

	public class FlameboltsItem : GlobalItem
	{
		public override bool? UseItem(Item item, Player player)
		{
			if (item.damage > 0 && item.pick == 0 && item.axe == 0 && item.hammer == 0)
			{
				var mp = player.GetModPlayer<FlameboltsPlayer>();
				if (mp.flameboltsActive && mp.flameboltCooldown <= 0)
				{
					mp.flameboltCooldown = 30;
					Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.UnitX.RotatedByRandom(6.28f) * 20, ModContent.ProjectileType<Flamebolt>(), item.damage / 2, 0, player.whoAmI, -1);

					SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot.WithPitchOffset(0.5f), player.Center);
				}
			}

			return base.UseItem(item, player);
		}
	}

	public class Flamebolt : ModProjectile
	{
		private List<Vector2> cache;
		private Trail trail;

		public ref float Target => ref Projectile.ai[0];

		public override string Texture => "Umbra/Assets/Invisible";

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 300;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (Projectile.timeLeft <= 30)
				return;

			if (Target < 0 || Target > Main.npc.Length)
			{
				NPC candidate = null;
				float best = 1000;

				foreach(NPC npc in Main.npc)
				{
					var dist = Vector2.Distance(npc.Center, Projectile.Center);

					if (!npc.friendly && !npc.dontTakeDamage && !npc.immortal && npc.CanBeChasedBy(this) && dist < best)
					{
						candidate = npc;
						best = dist;
					}
				}

				if (candidate != null)
					Target = candidate.whoAmI;
				else
					Target = -1;
			}

			if (Target < 0 || Target > Main.npc.Length)
			{
				if (Projectile.velocity.Length() < 1)
					Projectile.velocity = Vector2.UnitX.RotatedByRandom(6.28f) * 8;
			}
			else
			{
				var targetNPC = Main.npc[(int)Target];

				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.Center.DirectionTo(targetNPC.Center) * 40, 0.05f);

				if (targetNPC.friendly || targetNPC.dontTakeDamage || targetNPC.immortal || !targetNPC.active || !targetNPC.CanBeChasedBy(this))
					Target = -1;
			}

			Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2, 2), ModContent.DustType<Dusts.PixelatedEmber>(), -Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.05f, 0.1f), 0, new Color(1f, Main.rand.NextFloat(0.5f), 0.0f, 0f), Main.rand.NextFloat(0.05f, 0.2f));

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
					effect.Parameters["repeats"].SetValue(3f);
					effect.Parameters["transformMatrix"].SetValue(world * view * projection);
					effect.Parameters["sampleTexture"].SetValue(Assets.Trails.EnergyTrail.Value);
					trail?.Render(effect);
				}
			});

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.timeLeft = 30;
			Projectile.velocity *= 0;

			for (int k = 0; k < 25; k++)
			{
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.PixelatedEmber>(), Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(20), 0, new Color(1f, Main.rand.NextFloat(0.5f), 0.0f, 0f), Main.rand.NextFloat(0.05f, 0.2f));
			}

			target.AddBuff(BuffID.OnFire, 60);

			SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithPitchOffset(0.5f), Projectile.Center);
		}

		public override bool? CanHitNPC(NPC target)
		{
			return Projectile.timeLeft > 30 ? null : false;
		}

		protected void ManageCaches()
		{
			if (cache == null)
			{
				cache = [];

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
				trail = new Trail(Main.instance.GraphicsDevice, 30, new NoTip(), factor => factor * 32, factor =>
				{
					float alpha = 1f;

					if (factor.X == 1)
						alpha = 0;

					if (Projectile.timeLeft < 20)
						alpha *= Projectile.timeLeft / 20f;

					alpha *= factor.X;

					Color color = new Color(1f, MathF.Max(0f, 2 * (factor.X - 0.5f)), 0f, 0) * (Projectile.timeLeft < 30 ? (Projectile.timeLeft / 30f) : 1);

					return color * alpha;
				});
			}

			trail.Positions = cache.ToArray();
			trail.NextPosition = Projectile.Center + Projectile.velocity;
		}
	}
}
