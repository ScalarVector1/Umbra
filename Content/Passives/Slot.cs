using Terraria.Audio;
using Terraria.ID;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
	internal class Slot : Passive
	{
		public override bool AllowDuplicates => true;

		public Item SlottedItem => TreeSystem.tree.storedItems.ContainsKey(ID) ? TreeSystem.tree.storedItems[ID] : null;

		public override string Tooltip => SlottedItem?.ToolTip?._text?.Value ?? "";

		public override void SetDefaults()
		{
			texture = Assets.Passives.Slot;
			difficulty = 0;
			size = 1;
		}

		public override void Draw(SpriteBatch spriteBatch, Vector2 center, float scale)
		{
			base.Draw(spriteBatch, center, scale);

			if (SlottedItem != null)
			{
				Texture2D shine = Assets.Masks.ShinyGlow.Value;

				Color alphaColor = SlottedItem.color;
				alphaColor.A = 0;

				for (int k = 0; k < 6; k++)
				{
					float sin = (float)Math.Sin((Main.timeForVisualEffects + k * 20) / 120f * 6.28f);
					Color color = alphaColor;

					if (k == 0)
						color.R += 200;
					if (k == 2)
						color.G += 200;
					if (k == 4)
						color.B += 200;

					float rot = (float)Main.timeForVisualEffects * (0.002f * k) * (k % 2 == 0 ? -1 : 1) + k;

					spriteBatch.Draw(shine, center, null, color * sin * 0.25f, rot, shine.Size() / 2f, scale * (0.4f + sin * 0.4f) * 0.7f, 0, 0);
					spriteBatch.Draw(shine, center, null, new Color(255, 255, 255, 0) * sin * 0.1f, rot, shine.Size() / 2f, scale * (0.4f + sin * 0.4f) * 0.5f, 0, 0);
				}

				Texture2D tex = Assets.Passives.SlotGem.Value;
				Texture2D tex2 = Assets.Passives.SlotGemAdd.Value;
				spriteBatch.Draw(tex, center, null, SlottedItem.color, 0, tex.Size() / 2f, scale, 0, 0);
				spriteBatch.Draw(tex2, center, null, new Color(255, 220, 180, 0), 0, tex2.Size() / 2f, scale, 0, 0);
			}
		}

		public override void OnClick()
		{
			if (Main.mouseItem != null && !Main.mouseItem.IsAir && SlottedItem is null)
			{
				TreeSystem.tree.storedItems[ID] = Main.mouseItem.Clone();
				Main.mouseItem.TurnToAir();

				SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact.WithPitchOffset(-0.2f));
				SoundEngine.PlaySound(SoundID.Tink.WithPitchOffset(-0.5f));
				SoundEngine.PlaySound(SoundID.Shatter.WithPitchOffset(-0.3f));
			}
			else if ((Main.mouseItem is null || Main.mouseItem.IsAir) && SlottedItem is not null)
			{
				Main.mouseItem = SlottedItem.Clone();
				TreeSystem.tree.storedItems.Remove(ID);

				SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact.WithPitchOffset(-0.5f));
				SoundEngine.PlaySound(SoundID.Tink.WithPitchOffset(-0f));
				SoundEngine.PlaySound(SoundID.Shatter.WithPitchOffset(-0.5f));
			}

			TreeSystem.tree.CalcDifficultyAndTooltips();
		}

		public override bool CanDeallocate(Player player)
		{
			return base.CanDeallocate(player) && SlottedItem is null;
		}
	}
}
