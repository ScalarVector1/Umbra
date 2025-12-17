using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using Terraria.Utilities;
using Umbra.Core.PixelationSystem;

namespace Umbra.Content.Items.Slottables.Effects
{
	internal class ShockingCrits : SlottableEffect
	{
		public override void BuffPlayer(Player player)
		{
			player.GetModPlayer<ShockingCritsPlayer>().shockingCritsActive = true;
		}
	}

	internal class ShockingCritsPlayer : ModPlayer
	{
		public bool shockingCritsActive;
		public int shockingCritsCooldown;

		public override void PreUpdate()
		{
			if (shockingCritsCooldown > 0)
				shockingCritsCooldown--;
		}

		public override void ResetEffects()
		{
			shockingCritsActive = false;
		}
	}

	internal class ShockingCritsNPC : GlobalNPC
	{
		public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
			ShockingCritsPlayer mp = player.GetModPlayer<ShockingCritsPlayer>();

			if (mp.shockingCritsActive && mp.shockingCritsCooldown <= 0 && hit.Crit)
			{
				Projectile.NewProjectile(player.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<ShockingCritsProjectile>(), damageDone / 4, 0, player.whoAmI);
				mp.shockingCritsCooldown = 10;
			}
		}

		public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			if (projectile.owner < 0)
				return;

			Player player = Main.player[projectile.owner];
			ShockingCritsPlayer mp = player.GetModPlayer<ShockingCritsPlayer>();

			if (mp.shockingCritsActive && mp.shockingCritsCooldown <= 0 && hit.Crit)
			{
				Projectile.NewProjectile(player.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<ShockingCritsProjectile>(), damageDone / 4, 0, player.whoAmI);
				mp.shockingCritsCooldown = 10;
			}
		}
	}

	internal class ShockingCritsProjectile : ModProjectile
	{
		Vector2 savedPos = Vector2.Zero;

		readonly List<NPC> hitNPCs = [];

		public override string Texture => "Umbra/Assets/Invisible";

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
		{
			if (Projectile.timeLeft == 180)
			{
				savedPos = Projectile.Center;
				Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);

				Projectile.extraUpdates = 0;
				Projectile.velocity *= 0;
			}

			if (Projectile.timeLeft > 15)
				Projectile.timeLeft = 15;

			Projectile.Opacity = MathF.Sin(Projectile.timeLeft / 15f * 3.14f);

			for (int k = 0; k < hitNPCs.Count; k++)
			{
				Vector2 start = k == 0 ? savedPos : hitNPCs[k - 1].Center;
				Vector2 end = hitNPCs[k].Center;

				Vector2 vel = end.DirectionTo(start).RotatedBy(Main.rand.NextBool() ? 0.5f : -0.5f) * Main.rand.NextFloat(0.2f, 0.8f);
				Dust.NewDustPerfect(Vector2.Lerp(start, end, Main.rand.NextFloat()), ModContent.DustType<Dusts.PixelatedEmber>(), vel, 0, new Color(80, 50, 200, 0), Main.rand.NextFloat(0.05f, 0.1f));

				if (Main.rand.NextBool(3))
				{
					vel = end.DirectionTo(start).RotatedBy(Main.rand.NextBool() ? 0.3f : -0.3f) * Main.rand.NextFloat(6f, 12f);
					Dust.NewDustPerfect(Vector2.Lerp(start, end, Main.rand.NextFloat()), ModContent.DustType<Dusts.GlowFastDecelerate>(), vel, 0, new Color(200, 150, 255, 0), Main.rand.NextFloat(0.15f, 0.25f));
				}
			}
		}

		public override void PostDraw(Color lightColor)
		{
			for (int k = 0; k < hitNPCs.Count; k++)
			{
				Vector2 start = k == 0 ? savedPos : hitNPCs[k - 1].Center;
				Vector2 end = hitNPCs[k].Center;

				ModContent.GetInstance<PixelationSystem>().QueueRenderAction("UnderProjectiles", () => DrawLightning(Main.spriteBatch, start, end));
			}
		}

		private void DrawLightning(SpriteBatch spritebatch, Vector2 point1, Vector2 point2)
		{
			Texture2D tex = Assets.Trails.GlowTrailNoEnd.Value;
			Texture2D tex2 = Assets.Trails.GlowTrail.Value;
			Texture2D glow = Assets.Masks.StarGlow.Value;

			float dist = Vector2.Distance(point1, point2);
			float rot = point1.DirectionTo(point2).ToRotation();

			var target = new Rectangle((int)(point1.X - Main.screenPosition.X), (int)(point1.Y - Main.screenPosition.Y), (int)dist, (int)(60 * Projectile.Opacity));
			var target2 = new Rectangle((int)(point1.X - Main.screenPosition.X), (int)(point1.Y - Main.screenPosition.Y), (int)dist, (int)(90 * Projectile.Opacity));

			spritebatch.Draw(tex, target, null, new Color(150, 50, 200, 0) * 0.45f * Projectile.Opacity, rot, new Vector2(0, tex.Height / 2f), 0, 0);
			spritebatch.Draw(tex, target2, null, new Color(100, 30, 200, 0) * 0.25f * Projectile.Opacity, rot, new Vector2(0, tex.Height / 2f), 0, 0);

			var rand = new UnifiedRandom((int)Main.GameUpdateCount / 3 ^ 901273125 + Projectile.whoAmI ^ 917232);

			float lastOffset = 0;

			int segments = (int)(dist / 42) + 1;
			int step = (int)(dist / segments);

			for (int k = 0; k < step * segments; k += step)
			{
				var segStart = Vector2.Lerp(point1, point2, k / dist);
				segStart += Vector2.UnitX.RotatedBy(rot + 1.57f) * lastOffset;

				lastOffset = rand.NextFloat(-10, 10);

				if (k == step * segments - step)
					lastOffset = 0;

				var segEnd = Vector2.Lerp(point1, point2, (k + step) / dist);
				segEnd += Vector2.UnitX.RotatedBy(rot + 1.57f) * lastOffset;

				float segDist = Vector2.Distance(segStart, segEnd);
				float segWidth = 6 + MathF.Sin(k / dist * 3.14f) * 10;

				var segTarget = new Rectangle((int)(segStart.X - Main.screenPosition.X), (int)(segStart.Y - Main.screenPosition.Y), (int)segDist + 2, (int)(segWidth * Projectile.Opacity));
				spritebatch.Draw(tex2, segTarget, null, new Color(220, 200, 255, 0) * Projectile.Opacity, segStart.DirectionTo(segEnd).ToRotation(), new Vector2(0, tex2.Height / 2f), 0, 0);
			}

			spritebatch.Draw(glow, point2 - Main.screenPosition, null, new Color(180, 150, 200, 0) * 0.85f * Projectile.Opacity, 0, glow.Size() / 2f, 0.2f * Projectile.Opacity, 0, 0);
			spritebatch.Draw(glow, point2 - Main.screenPosition, null, new Color(100, 30, 200, 0) * 0.25f * Projectile.Opacity, 0, glow.Size() / 2f, 0.4f * Projectile.Opacity, 0, 0);
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			modifiers.FinalDamage *= 0;
			modifiers.DisableCrit();
			modifiers.DisableKnockback();
			modifiers.HideCombatText();
		}

		public override bool? CanHitNPC(NPC target)
		{
			return hitNPCs.Count <= 0;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			hitNPCs.Add(target);

			CheckAboutNPC(target, 3);

			hitNPCs.ForEach(n => n.SimpleStrikeNPC(Projectile.damage, 0, false, 0));
		}

		public void CheckAboutNPC(NPC origin, int remaining)
		{
			if (remaining <= 0)
				return;

			var rng = new Random();
			int[] indicies = Enumerable.Range(0, Main.maxNPCs + 1).OrderBy(_ => rng.Next()).ToArray();

			for (int k = 0; k < indicies.Length; k++)
			{
				NPC scan = Main.npc[indicies[k]];
				if (scan.active && scan.chaseable && !scan.friendly && !scan.immortal && !scan.dontTakeDamage && !hitNPCs.Contains(scan) && Vector2.Distance(scan.Center, origin.Center) < 500)
				{
					hitNPCs.Add(scan);
					CheckAboutNPC(scan, remaining - 1);
					break;
				}
			}
		}
	}
}
