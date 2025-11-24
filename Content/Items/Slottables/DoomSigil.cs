using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Items.Slottables
{
	internal class DoomSigil : Slottable
	{
		public override string Texture => "Umbra/Assets/Items/DoomSigil";

		public override void SetDefaults()
		{
			base.SetDefaults();
			difficulty = 100;
		}

		public override void DrawInSlot(SpriteBatch spriteBatch, Vector2 center, float scale, int slotID)
		{
			Texture2D shine = Assets.Masks.GlowAlpha.Value;
			Texture2D shine2 = Assets.Masks.Glow.Value;

			float sin = (float)(Main.timeForVisualEffects % 60) / 60f;

			spriteBatch.Draw(shine, center, null, new Color(200, 0, 0, 0) * (1f - sin), 0, shine.Size() / 2f, scale * 2f * sin, 0, 0);
			spriteBatch.Draw(shine2, center, null, Color.Black * (1f - sin), 0, shine2.Size() / 2f, scale * 2.5f * sin, 0, 0);

			Texture2D tex = Assets.Passives.DoomSigil.Value;
			spriteBatch.Draw(tex, center, null, Color.White, 0, tex.Size() / 2f, scale, 0, 0);
		}

		public override void OnSocket()
		{
			SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot.WithPitchOffset(-1.8f));
			SoundEngine.PlaySound(SoundID.Tink.WithPitchOffset(-0.7f));
		}

		public override void OnDesocket()
		{
			SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot.WithPitchOffset(-2.5f));
			SoundEngine.PlaySound(SoundID.Tink.WithPitchOffset(-0.9f));
		}
	}
}
