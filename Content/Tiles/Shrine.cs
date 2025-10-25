using StarlightRiver.Content.Items.Hovers;
using StarlightRiver.Content.Tiles.Permafrost;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Tiles
{
	internal class Shrine : ModTile
	{
		public override string Texture => "Umbra/Assets/Tiles/Shrine";

		public override void SetStaticDefaults()
		{
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<ShrineEntity>().Hook_AfterPlacement, -1, 0, false);

			this.QuickSetFurniture(3, 3, DustID.Stone, SoundID.Tink, true, new Color(75, 75, 75), false, false, "Umbral Shrine", new AnchorData(AnchorType.SolidTile, 3, 0));
			Main.tileLighted[Type] = true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (tile.TileFrameX == 18 && tile.TileFrameY == 0)
			{
				int x = i - tile.TileFrameX / 18;
				int y = j - tile.TileFrameY / 18;

				int index = ModContent.GetInstance<ShrineEntity>().Find(x, y - 1);

				if (index == -1)
					return;

				var entity = (ShrineEntity)TileEntity.ByID[index];
				float power = entity.Power;

				float sin = 1.1f + (float)System.Math.Sin(Main.GameUpdateCount * 0.04f) * (float)System.Math.Cos(Main.GameUpdateCount * 0.065f) * 0.15f;
				(r, g, b) = (0.75f * sin * power, 0.5f * sin * power, 1f * sin * power);
			}
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			int x = i - tile.TileFrameX / 18;
			int y = j - tile.TileFrameY / 18;

			int index = ModContent.GetInstance<ShrineEntity>().Find(x, y);

			if (index == -1)
				return;

			var entity = (ShrineEntity)TileEntity.ByID[index];
			float power = entity.Power;

			var bauble = Assets.Tiles.ShrineBauble.Value;

			Texture2D glow = Assets.GUI.GlowAlpha.Value;
			Texture2D star = Assets.GUI.StarAlpha.Value;

			Texture2D back = Assets.GUI.GlowSoft.Value;

			Vector2 pos = new Vector2(i, j) * 16 + new Vector2(8, 8 - 32);

			pos.Y += MathF.Sin(Main.GameUpdateCount * 0.03f) * 4f;
			pos += Vector2.One * Main.offScreenRange;

			float sinTime = 0.7f + 0.1f * (float)Math.Sin(Main.GameUpdateCount / 30f * 3.14f);

			var glowColor = new Color(160, 100, 255, 0);

			spriteBatch.Draw(back, pos + new Vector2(0, -6) - Main.screenPosition, null, Color.Black * power, 0, back.Size() / 2f, 1f, 0, 0);

			spriteBatch.Draw(bauble, pos - Main.screenPosition, null, Lighting.GetColor(i, j - 1), 0, bauble.Size() / 2f, 1f, 0, 0);

			spriteBatch.Draw(glow, pos + new Vector2(0, -6) - Main.screenPosition, null, glowColor * sinTime * power, 0, glow.Size() / 2f, 0.8f * sinTime, 0, 0);
			spriteBatch.Draw(star, pos + new Vector2(0, -6) - Main.screenPosition, null, glowColor * sinTime * power, 0, star.Size() / 2f, 0.25f * sinTime, 0, 0);

			// Draw number if nearby
			pos = new Vector2(i, j) * 16 + new Vector2(8, 8 - 64);
			var dist = Vector2.Distance(pos, Main.LocalPlayer.Center);

			if (dist < 200)
			{
				Vector2 offset = (pos - Main.LocalPlayer.Center) * -0.1f;
				float alpha = dist > 100 ? 1f - (dist - 100) / 100f : 1f;

				offset += Vector2.UnitX.RotatedBy(Main.GameUpdateCount * 0.1f) * 2f;
				Vector2 drawPos = pos + offset + Vector2.One * Main.offScreenRange - Main.screenPosition;

				Utils.DrawBorderString(spriteBatch, $"{entity.ShownUmbra}", drawPos, new Color(200, 140, 255) * alpha, 1f, 0.5f, 0.5f);
			}
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (tile.TileFrameX == 18 && tile.TileFrameY == 0)
			{
				int x = i - tile.TileFrameX / 18;
				int y = j - tile.TileFrameY / 18;

				int index = ModContent.GetInstance<ShrineEntity>().Find(x, y);

				if (index == -1)
					return;

				var entity = (ShrineEntity)TileEntity.ByID[index];
				float power = entity.Power;

				Main.instance.TilesRenderer.AddSpecialLegacyPoint(new Point(i, j));

				Vector2 pos = new Vector2(i, j) * 16 + new Vector2(8, 8 - 32);

				if (Main.rand.NextBool(3))
					Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(12, 12), ModContent.DustType<Dusts.PixelatedEmber>(), Vector2.UnitY, 0, new Color(150, 50, 255, 0) * power, Main.rand.NextFloat(0.3f) * power);

				float rot = Main.rand.NextFloat(6.28f);
				Dust.NewDustPerfect(pos + new Vector2(0, -6) + Vector2.UnitX.RotatedBy(rot) * 32 * power, ModContent.DustType<Dusts.PixelatedGlow>(), Vector2.UnitX.RotatedBy(rot) * -1, 0, new Color(50, 50, 150, 0) * power, 0.1f * power);
			}
		}

		public override bool RightClick(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			int x = i - tile.TileFrameX / 18;
			int y = j - tile.TileFrameY / 18;

			int index = ModContent.GetInstance<ShrineEntity>().Find(x, y);

			if (index == -1)
				return true;

			var entity = (ShrineEntity)TileEntity.ByID[index];
			var tp = Main.LocalPlayer.GetModPlayer<TreePlayer>();

			if (tp.UmbraPoints > 0)
			{
				entity.queuedUmbra += tp.UmbraPoints;
				entity.storedUmbra += tp.UmbraPoints;
				tp.UmbraPoints = 0;

				UmbraNet.SyncPoints(Main.myPlayer);
				NetMessage.SendTileSquare(Main.myPlayer, i - 3, j - 3, 6);

				Vector2 pos = new Vector2(x + 1, y) * 16 + Vector2.One * 8;

				for (int k = 0; k < 30; k++)
				{
					float rot = Main.rand.NextFloat(6.28f);
					Dust.NewDustPerfect(pos + new Vector2(0, -32) + Vector2.UnitX.RotatedBy(rot) * 64, ModContent.DustType<Dusts.PixelatedGlow>(), Vector2.UnitX.RotatedBy(rot) * -Main.rand.NextFloat(2f, 4f), 0, new Color(80, 50, 150, 0), Main.rand.NextFloat(0.3f));
				}

				SoundEngine.PlaySound(SoundID.DD2_BookStaffCast.WithPitchOffset(-0.1f).WithVolume(0.5f));
				SoundEngine.PlaySound(SoundID.GuitarD.WithVolume(0.4f).WithPitchOffset(-0.3f));
				SoundEngine.PlaySound(SoundID.DrumKick);
			}
			else
			{
				tp.UmbraPoints += entity.storedUmbra;
				entity.storedUmbra = 0;
				entity.queuedUmbra = 0;

				UmbraNet.SyncPoints(Main.myPlayer);
				NetMessage.SendTileSquare(Main.myPlayer, i - 3, j - 3, 6);

				Vector2 pos = new Vector2(x + 1, y) * 16 + Vector2.One * 8;

				for (int k = 0; k < 30; k++)
				{
					float rot = Main.rand.NextFloat(6.28f);
					Dust.NewDustPerfect(pos + new Vector2(0, -32), ModContent.DustType<Dusts.PixelatedGlow>(), Vector2.UnitX.RotatedBy(rot) * Main.rand.NextFloat(2f, 4f), 0, new Color(80, 50, 150, 0), Main.rand.NextFloat(0.3f));
				}

				SoundEngine.PlaySound(SoundID.DD2_BookStaffCast.WithPitchOffset(-0.1f).WithVolume(1f));
				SoundEngine.PlaySound(SoundID.GuitarAm.WithVolume(0.8f).WithPitchOffset(-0.4f));
				SoundEngine.PlaySound(SoundID.DrumKick);
			}

			return true;
		}
	}
	internal sealed class ShrineEntity : ModTileEntity
	{
		public int queuedUmbra;
		public int storedUmbra;

		public int ShownUmbra => storedUmbra - queuedUmbra;
		public float Power => Math.Min(1f, ShownUmbra / 100f);

		public override bool IsTileValidForEntity(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			return tile.TileType == ModContent.TileType<Shrine>() && tile.HasTile && tile.TileFrameX == 0 && tile.TileFrameY == 0;
		}

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 3);
				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i - 1, j - 1, Type, 0f, 0, 0, 0);
				return -1;
			}

			return Place(i - 1, j - 2);
		}

		public override void Update()
		{
			if (queuedUmbra > 0)
				queuedUmbra -= Math.Max(1, queuedUmbra / 10);

			if (queuedUmbra < 0)
				queuedUmbra = 0;
		}

		public override void SaveData(TagCompound tag)
		{
			tag["stored"] = storedUmbra;
		}

		public override void LoadData(TagCompound tag)
		{
			storedUmbra = tag.GetInt("stored");
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(storedUmbra);
		}

		public override void NetReceive(BinaryReader reader)
		{
			storedUmbra = reader.ReadInt32();
		}
	}

	public class ShrineItem : ModItem
	{
		public override string Texture => "Umbra/Assets/Tiles/ShrineItem";

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 9999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Shrine>();
			Item.rare = ItemRarityID.LightPurple;
			Item.value = 0;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ItemID.StoneBlock, 20)
			.AddIngredient(ItemID.GoldBar, 5)
			.AddTile(TileID.WorkBenches)
			.Register();

			CreateRecipe()
			.AddIngredient(ItemID.StoneBlock, 20)
			.AddIngredient(ItemID.PlatinumBar, 5)
			.AddTile(TileID.WorkBenches)
			.Register();
		}
	}
}
