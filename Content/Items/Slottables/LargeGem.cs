using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;

namespace Umbra.Content.Items.Slottables
{
	internal class LargeGem : Slottable
	{
		public override string Texture => "Umbra/Assets/Items/Gem";

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
	}
}
