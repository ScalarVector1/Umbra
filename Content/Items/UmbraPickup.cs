using System.IO;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Umbra.Content.GUI;
using Umbra.Core;
using Umbra.Core.Loaders.UILoading;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Items
{
	internal class UmbraPickup : ModItem
	{
		int worldTimer;

		public override string Texture => "Umbra/Assets/Items/UmbraPickup";

		public override void SetStaticDefaults()
		{
			ItemID.Sets.ItemNoGravity[Type] = true;
			ItemID.Sets.IsAPickup[Type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture2D tex = Assets.Items.UmbraPickup.Value;
			Texture2D glow = Assets.GUI.GlowAlpha.Value;
			Texture2D star = Assets.GUI.StarAlpha.Value;

			Texture2D back = Assets.GUI.GlowSoft.Value;

			float sinTime = worldTimer < 60 ? 0.7f + 0.3f * (float)Math.Sin(worldTimer / 60f * 3.14f) : 0.7f + 0.1f * (float)Math.Sin(worldTimer / 30f * 3.14f);
			float lineTime = Math.Min(1f, worldTimer / 90f);

			var glowColor = new Color(160, 100, 255, 0);

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

			if (worldTimer >= 90)
			{
				Player nearest = null;
				float lastDist = float.PositiveInfinity;

				foreach (Player player in Main.ActivePlayers)
				{
					float dist = Vector2.Distance(Item.Center, player.Center);

					if (!player.dead && dist < lastDist)
					{
						nearest = player;
						lastDist = dist;
					}
				}

				if (nearest != null)
				{
					Item.velocity = Vector2.Normalize(nearest.Center - Item.Center) * Math.Min(15, (worldTimer - 90) / 5f);

					var d = Dust.NewDustPerfect(Item.Center, DustID.Shadowflame, Vector2.Zero, 255, new Color(210, 160, 255), Main.rand.NextFloat(0.8f, 1f));
					d.noGravity = true;
				}
			}
		}

		public override bool CanPickup(Player player)
		{
			return worldTimer >= 90;
		}

		public override bool OnPickup(Player player)
		{
			TreePlayer tp = player.GetModPlayer<TreePlayer>();

			if (!tp.firstPoint)
			{
				tp.firstPoint = true;
				UILoader.GetUIState<InventoryButton>().flashing = true;
			}

			tp.UmbraPoints++;
			CombatText.NewText(player.Hitbox, new Color(200, 160, 255), Language.GetTextValue("Mods.Umbra.Misc.UmbraGainPopup"));

			for (int k = 0; k < 20; k++)
			{
				var d = Dust.NewDustPerfect(Item.Center, DustID.FireworksRGB, Vector2.UnitX.RotatedByRandom(6.28f) * Main.rand.NextFloat(7f), 255, new Color(210, 160, 255), Main.rand.NextFloat(0.3f, 0.8f));
				d.noGravity = true;
			}

			if (Main.myPlayer == player.whoAmI)
			{
				SoundEngine.PlaySound(SoundID.DD2_BookStaffCast.WithPitchOffset(-0.1f).WithVolume(0.5f));
				SoundEngine.PlaySound(SoundID.GuitarAm.WithVolume(0.4f).WithPitchOffset(-0.4f));
				SoundEngine.PlaySound(SoundID.DrumKick);

				UmbraNet.SyncPoints(player.whoAmI);
			}

			return false;
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(worldTimer);
		}

		public override void NetReceive(BinaryReader reader)
		{
			worldTimer = reader.ReadInt32();
		}
	}
}
