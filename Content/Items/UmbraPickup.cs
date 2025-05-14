using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Items
{
	internal class UmbraPickup : ModItem
	{
		int worldTimer;

		public override string Texture => "Umbra/Assets/Items/UmbraPickup";

		public override void SetStaticDefaults()
		{
			ItemID.Sets.ItemNoGravity[Type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			var tex = Assets.Items.UmbraPickup.Value;
			var glow = Assets.GUI.GlowAlpha.Value;
			var star = Assets.GUI.StarAlpha.Value;

			var back = Assets.GUI.GlowSoft.Value;

			float sinTime = worldTimer < 60 ? 0.7f + 0.3f * (float)Math.Sin(worldTimer / 60f * 3.14f) : 0.7f + 0.1f * (float)Math.Sin(worldTimer / 30f * 3.14f);
			float lineTime = Math.Min(1f, worldTimer / 90f);

			Color glowColor = new Color(160, 100, 255, 0);

			spriteBatch.Draw(back, Item.Center + new Vector2(0, -6) - Main.screenPosition, null, Color.Black, rotation, back.Size() / 2f, 1f, 0, 0);

			spriteBatch.Draw(tex, Item.Center - Main.screenPosition, null, lightColor, rotation, tex.Size() / 2f, scale, 0, 0);

			spriteBatch.Draw(glow, Item.Center + new Vector2(0, -6) - Main.screenPosition, null, glowColor * sinTime, rotation, glow.Size() / 2f, 0.8f * sinTime, 0, 0);
			spriteBatch.Draw(star, Item.Center + new Vector2(0, -6) - Main.screenPosition, null, glowColor * sinTime, rotation, star.Size() / 2f, 0.25f * sinTime, 0, 0);

			spriteBatch.Draw(glow, Item.Center + new Vector2(0, -6) - Main.screenPosition, null, glowColor * (1f - lineTime), rotation, glow.Size() / 2f, 3 * lineTime, 0, 0);

			return false;
		}

		public override void PostUpdate()
		{
			worldTimer++;

			Lighting.AddLight(Item.Center, new Vector3(0.6f, 0.4f, 1f));

			if (Main.rand.NextBool(10))
			{
				var d = Dust.NewDustPerfect(Item.Center, DustID.FireworksRGB, Vector2.UnitX.RotatedByRandom(6.28f) * Main.rand.NextFloat(3f), 255, new Color(210, 160, 255), Main.rand.NextFloat(0.3f, 0.8f));
				d.noGravity = true;
			}
		}

		public override bool CanPickup(Player player)
		{
			return worldTimer >= 90;
		}

		public override bool OnPickup(Player player)
		{
			player.GetModPlayer<TreePlayer>().UmbraPoints++;
			CombatText.NewText(player.Hitbox, new Color(200, 160, 255), "+1 Umbra");

			for(int k = 0; k < 20; k++)
			{
				var d = Dust.NewDustPerfect(Item.Center, DustID.FireworksRGB, Vector2.UnitX.RotatedByRandom(6.28f) * Main.rand.NextFloat(7f), 255, new Color(210, 160, 255), Main.rand.NextFloat(0.3f, 0.8f));
				d.noGravity = true;
			}

			SoundEngine.PlaySound(SoundID.DD2_BookStaffCast.WithPitchOffset(-0.1f).WithVolume(0.5f));
			SoundEngine.PlaySound(SoundID.GuitarAm.WithVolume(0.4f).WithPitchOffset(-0.4f));
			SoundEngine.PlaySound(SoundID.DrumKick);

			return false;
		}
	}
}
