using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using Umbra.Content.Achievements;
using Umbra.Content.Items.Slottables.Effects;
using Umbra.Helpers;

namespace Umbra.Content.Items.Slottables
{
	internal class CraftedGem : Slottable
	{
		public int nameVariant1;
		public int nameVariant2;

		public override string Texture => "Umbra/Assets/Items/GemFakeBase";

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
			];

		public override void Load()
		{
			On_Item.Prefix += AllowReforgingGems;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.rare = ItemRarityID.Blue;

			if (effects.Count == 0)
				RollGem();

			Item.SetNameOverride(DisplayName.Format(Language.GetText($"Mods.Umbra.Items.CraftedGem.Variant1_{nameVariant1}"), Language.GetText($"Mods.Umbra.Items.CraftedGem.Variant2_{nameVariant2}")));
		}

		public override void OnCreated(ItemCreationContext context)
		{
			base.OnCreated(context);
		}

		private bool AllowReforgingGems(On_Item.orig_Prefix orig, Item self, int prefixWeWant)
		{
			if (self.ModItem is CraftedGem)
				return true;

			return orig(self, prefixWeWant);
		}

		public void RollGem()
		{
			var rolled = effectPool.OrderBy(n => Main.rand.Next()).ToList();
			int effectCount = Main.rand.Next(2, 4);

			for (int k = 0; k < effectCount; k++)
			{
				AddEffect(rolled[k], Main.rand.Next(1, 4));
			}

			nameVariant1 = Main.rand.Next(5);
			nameVariant2 = Main.rand.Next(5);
			Item.color = ColorHelper.FromHSV(Main.rand.NextFloat(), 1f, 1f);
		}

		public override void PostReforge()
		{
			effects.RemoveRange(1, effects.Count - 1);

			var rolled = effectPool.OrderBy(n => Main.rand.Next()).ToList();
			rolled.RemoveAll(n => n.FullName == effects[0].FullName);

			int effectCount = Main.rand.Next(1, 3);

			for (int k = 0; k < effectCount; k++)
			{
				AddEffect(rolled[k], Main.rand.Next(1, 3));
			}

			Item.prefix = 0;
		}

		public override bool CanReforge()
		{
			return true;
		}

		public override bool ReforgePrice(ref int reforgePrice, ref bool canApplyDiscount)
		{
			reforgePrice = Item.gold * 10 * effects.Sum(n => n.tier);
			return true;
		}

		public override void DrawInSlot(SpriteBatch spriteBatch, Vector2 center, float scale, int slotID)
		{
			Texture2D baseTex = Assets.Passives.SlotGemFake.Value;
			spriteBatch.Draw(baseTex, center, null, Color.White, 0, baseTex.Size() / 2f, scale, 0, 0);

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

				spriteBatch.Draw(shine, center, null, color * sin * 0.2f, rot, shine.Size() / 2f, scale * (0.4f + sin * 0.4f) * 0.45f, 0, 0);
				spriteBatch.Draw(shine, center, null, new Color(255, 255, 255, 0) * sin * 0.1f, rot, shine.Size() / 2f, scale * (0.4f + sin * 0.4f) * 0.3f, 0, 0);
			}

			Texture2D tex = Assets.Passives.SlotGemFakeColor.Value;
			Texture2D tex2 = Assets.Passives.SlotGemFakeAdd.Value;
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
			Texture2D baseTex = Assets.Items.GemFakeBase.Value;
			Texture2D tex = Assets.Items.GemFakeColor.Value;
			Texture2D tex2 = Assets.Items.GemFakeAdd.Value;

			spriteBatch.Draw(baseTex, position, null, Color.White, 0, baseTex.Size() / 2f, scale, 0, 0);
			spriteBatch.Draw(tex, position, null, Item.color, 0, tex.Size() / 2f, scale, 0, 0);
			spriteBatch.Draw(tex2, position, null, new Color(255, 220, 180, 0), 0, tex2.Size() / 2f, scale, 0, 0);

			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture2D baseTex = Assets.Items.GemFakeBase.Value;
			Texture2D tex = Assets.Items.GemFakeColor.Value;
			Texture2D tex2 = Assets.Items.GemFakeAdd.Value;

			spriteBatch.Draw(baseTex, Item.Center - Main.screenPosition, null, lightColor, rotation, baseTex.Size() / 2f, scale, 0, 0);
			spriteBatch.Draw(tex, Item.Center - Main.screenPosition, null, Item.color.MultiplyRGB(lightColor), rotation, tex.Size() / 2f, scale, 0, 0);
			spriteBatch.Draw(tex2, Item.Center - Main.screenPosition, null, new Color(255, 220, 180, 0).MultiplyRGBA(lightColor), rotation, tex2.Size() / 2f, scale, 0, 0);

			return false;
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
