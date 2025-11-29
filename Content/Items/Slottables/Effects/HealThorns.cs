using System.Collections.Generic;
using System.Linq;
using Terraria.ID;

namespace Umbra.Content.Items.Slottables.Effects
{
	internal class HealThorns : SlottableEffect
	{
		public override void BuffPlayer(Player player)
		{
			player.GetModPlayer<HealThornsPlayer>().healThornsActive = true;
		}
	}

	public class HealThornsPlayer : ModPlayer
	{
		public bool healThornsActive;

		public override void ResetEffects()
		{
			healThornsActive = false;
		}
	}

	public class HealThormsItem : GlobalItem
	{
		public override bool? UseItem(Item item, Player player)
		{
			if (player.GetModPlayer<HealThornsPlayer>().healThornsActive && item.healLife > 0)
			{
				for(int k = 0; k < 10; k++)
				{
					Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.UnitX.RotatedBy( k / 10f * 6.28f), ModContent.ProjectileType<HealThornProjectile>(), 80, 0, player.whoAmI, 0, k * 5);
				}
			}

			return base.UseItem(item, player);
		}
	}

	public class HealThornProjectile : ModProjectile
	{
		List<Vector2> points = [];
		public Vector2 direction = Vector2.Zero;

		public ref float Timer => ref Projectile.ai[0];
		public ref float Pause => ref Projectile.ai[1];

		public override string Texture => "Umbra/Assets/Invisible";

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 900;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
		}

		public override void AI()
		{
			if (Timer == 0)
			{
				direction = Projectile.velocity;
				Projectile.velocity *= 0;
				points.Add(Projectile.Center);

				Timer = 1;
			}

			Pause--;

			if (Pause > 0)
				return;

			Timer++;

			Projectile.extraUpdates = 12 - (int)((Timer) / 50f);

			if (Timer > 600)
				Projectile.timeLeft = 0;

			if (points.Count < Timer / 15f)
			{
				points.Add(points.Last() + direction * 15);
				direction = direction.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
			}

			if (Timer < 570 && Main.rand.NextBool(3))
			{
				Dust.NewDustPerfect(points.Last() + direction * 25, ModContent.DustType<Dusts.PixelatedEmber>(), Main.rand.NextVector2Circular(2f, 2f), 0, new Color(Main.rand.Next(50, 150), 0, 255, 0), Main.rand.NextFloat(0.05f, 0.2f));
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			foreach (Vector2 point in points)
			{
				if (targetHitbox.Intersects(new Rectangle((int)point.X - 8, (int)point.Y - 8, 16, 16)))
					return true;
			}

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Venom, 300);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[ProjectileID.VilethornTip].Value;

			float opacity = 1f;

			if (Timer > 570)
			{
				opacity = 1f - (Timer - 570) / 30f;
			}

			for(int k = 0; k < points.Count; k++)
			{
				var point = points[k];
				Vector2 last = k == 0 ? Projectile.Center : points[k - 1];
				float rot = point.DirectionFrom(last).ToRotation() + 1.57f;

				Main.spriteBatch.Draw(tex, point - Main.screenPosition, null, Lighting.GetColor(point.ToTileCoordinates()) * opacity, rot, tex.Size() / 2f, opacity, 0, 0);
			}

			Vector2 nextPos = points.Last() + direction * 15;
			float nextProg = (Timer - (points.Count - 1) * 15) / 15f;
			Main.spriteBatch.Draw(tex, nextPos - Main.screenPosition, null, Lighting.GetColor(nextPos.ToTileCoordinates()) * nextProg * opacity, 0, tex.Size() / 2f, nextProg * opacity, 0, 0);

			return false;
		}
	}
}
