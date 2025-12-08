using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Umbra.Content.Achievements;
using Umbra.Content.Items.Slottables.Effects;
using Umbra.Helpers;

namespace Umbra.Content.Items.Slottables
{
	internal class LargeGem : Slottable
	{
		public int nameVariant1;
		public int nameVariant2;

		public override string Texture => "Umbra/Assets/Items/Gem";

		public static readonly List<SlottableEffect> specialPool = [
			ModContent.GetInstance<Flamebolts>(),
			ModContent.GetInstance<HealThorns>(),
			ModContent.GetInstance<ExtraHeal>(),
			ModContent.GetInstance<ShockingCrits>()
			];

		public static readonly List<SlottableEffect> effectPool = [
			ModContent.GetInstance<PlayerLifeEffect>(),
			ModContent.GetInstance<PlayerManaEffect>(),
			ModContent.GetInstance<PlayerLifeRegenEffect>(),
			ModContent.GetInstance<PlayerManaRegenEffect>(),
			ModContent.GetInstance<PlayerDefenseEffect>(),
			ModContent.GetInstance<PlayerEnduranceEffect>(),
			ModContent.GetInstance<PlayerSpeedEffect>(),
			ModContent.GetInstance<PlayerDamageEffect>(),
			ModContent.GetInstance<PlayerCritEffect>(),
			ModContent.GetInstance<PlayerFlightEffect>(),
			ModContent.GetInstance<PlayerKnockbackEffect>(),
			ModContent.GetInstance<PlayerArmorPenetrateEffect>(),
			];

		public void RollGem()
		{
			AddEffect(specialPool[Main.rand.Next(specialPool.Count)], 1);

			var rolled = effectPool.OrderBy(n => Main.rand.Next()).ToList();
			int effectCount = Main.rand.Next(2, 4);
			int maxTier = effectCount == 2 ? 7 : 5;

			for (int k = 0; k < effectCount; k++)
			{
				AddEffect(rolled[k], Main.rand.Next(2, maxTier));
			}

			nameVariant1 = Main.rand.Next(5);
			nameVariant2 = Main.rand.Next(5);
			Item.color = ColorHelper.FromHSV(Main.rand.NextFloat(), 1f, 1f);
		}

		public override void SetDefaults()
		{
			base.SetDefaults();	
			Item.rare = ItemRarityID.LightPurple;

			if (effects.Count == 0)
				RollGem();

			Item.SetNameOverride(DisplayName.Format(Language.GetText($"Mods.Umbra.Items.LargeGem.Variant1_{nameVariant1}"), Language.GetText($"Mods.Umbra.Items.LargeGem.Variant2_{nameVariant2}")));
		}

		public override void DrawInSlot(SpriteBatch spriteBatch, Vector2 center, float scale, int slotID)
		{
			Texture2D shine = Assets.Masks.ShinyGlow.Value;

			Color alphaColor = Item.color;
			alphaColor.A = 0;

			for (int k = 0; k < 6; k++)
			{
				float sin = (float)Math.Sin((Main.timeForVisualEffects + slotID * 17 + k * 30) / 180f * 6.28f);
				Color color = alphaColor;

				if (k == 0)
					color.R += 200;
				if (k == 2)
					color.G += 200;
				if (k == 4)
					color.B += 200;

				float rot = (float)(Main.timeForVisualEffects + slotID * 17) * (0.005f + 0.0005f * k) * (k % 2 == 0 ? -1 : 1) + k;

				spriteBatch.Draw(shine, center, null, color * sin * 0.2f, rot, shine.Size() / 2f, scale * (0.4f + sin * 0.4f) * 0.65f, 0, 0);
				spriteBatch.Draw(shine, center, null, new Color(255, 255, 255, 0) * sin * 0.1f, rot, shine.Size() / 2f, scale * (0.4f + sin * 0.4f) * 0.5f, 0, 0);
			}

			Texture2D tex = Assets.Passives.SlotGem.Value;
			Texture2D tex2 = Assets.Passives.SlotGemAdd.Value;
			spriteBatch.Draw(tex, center, null, Item.color, 0, tex.Size() / 2f, scale, 0, 0);
			spriteBatch.Draw(tex2, center, null, new Color(255, 220, 180, 0), 0, tex2.Size() / 2f, scale, 0, 0);
		}

		public override void OnSocket()
		{
			SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact.WithPitchOffset(-0.5f));
			SoundEngine.PlaySound(SoundID.Shatter.WithPitchOffset(-0.3f).WithVolume(0.25f));

			GlimmerOfHope.condition.Complete();
		}

		public override void OnDesocket()
		{
			SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact.WithPitchOffset(-0.6f));
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Texture2D tex = Assets.Items.Gem.Value;
			Texture2D tex2 = Assets.Items.GemAdd.Value;

			spriteBatch.Draw(tex, position, null, Item.color, 0, tex.Size() / 2f, scale, 0, 0);
			spriteBatch.Draw(tex2, position, null, new Color(255, 220, 180, 0), 0, tex2.Size() / 2f, scale, 0, 0);

			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture2D shine = Assets.Masks.ShinyGlow.Value;

			alphaColor = Item.color;
			alphaColor.A = 0;

			for (int k = 0; k < 6; k++)
			{
				float sin = (float)Math.Sin((Main.timeForVisualEffects * 1.7f + k * 30) / 180f * 6.28f);
				Color color = alphaColor;

				if (k == 0)
					color.R += 200;
				if (k == 2)
					color.G += 200;
				if (k == 4)
					color.B += 200;

				float rot = (float)(Main.timeForVisualEffects * 1.7f) * (0.005f + 0.0005f * k) * (k % 2 == 0 ? -1 : 1) + k;

				spriteBatch.Draw(shine, Item.Center - Main.screenPosition, null, color * sin * 0.2f, rot, shine.Size() / 2f, scale * (0.4f + sin * 0.4f) * 0.65f, 0, 0);
				spriteBatch.Draw(shine, Item.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * sin * 0.1f, rot, shine.Size() / 2f, scale * (0.4f + sin * 0.4f) * 0.5f, 0, 0);
			}

			Texture2D tex = Assets.Items.Gem.Value;
			Texture2D tex2 = Assets.Items.GemAdd.Value;

			spriteBatch.Draw(tex, Item.Center - Main.screenPosition, null, Item.color.MultiplyRGB(lightColor), rotation, tex.Size() / 2f, scale, 0, 0);
			spriteBatch.Draw(tex2, Item.Center - Main.screenPosition, null, new Color(255, 220, 180, 0).MultiplyRGBA(lightColor), rotation, tex2.Size() / 2f, scale, 0, 0);

			return false;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			Lighting.AddLight(Item.Center, Item.color.ToVector3());
			Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.5f);

			if (Main.rand.NextBool(5))
			{
				Color color = Item.color;
				color.A = 0;
				Dust.NewDust(Item.position, Item.width, Item.height, ModContent.DustType<Dusts.PixelatedEmber>(), 0, 0, 0, color, 0.1f);
			}
			
			gravity = 0.01f;
		}

		public override void NetSend(BinaryWriter writer)
		{
			base.NetSend(writer);
			writer.Write(nameVariant1);
			writer.Write(nameVariant2);
		}

		public override void NetReceive(BinaryReader reader)
		{
			base.NetReceive(reader);
			nameVariant1 = reader.ReadInt32();
			nameVariant2 = reader.ReadInt32();
		}

		public override void SaveData(TagCompound tag)
		{
			base.SaveData(tag);
			tag["nameVariant1"] = nameVariant1;
			tag["nameVariant2"] = nameVariant2;
		}

		public override void LoadData(TagCompound tag)
		{
			base.LoadData(tag);
			nameVariant1 = tag.GetInt("nameVariant1");
			nameVariant2 = tag.GetInt("nameVariant2");
		}
	}
}
