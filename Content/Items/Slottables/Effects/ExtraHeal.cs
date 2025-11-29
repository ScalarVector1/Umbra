using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;

namespace Umbra.Content.Items.Slottables.Effects
{
	internal class ExtraHeal : SlottableEffect
	{
		public override void BuffPlayer(Player player)
		{
			player.GetModPlayer<ExtraHealPlayer>().extraHealActive = true;
		}
	}

	internal class ExtraHealPlayer : ModPlayer
	{
		public bool extraHealActive;

		public override void PostUpdateBuffs()
		{
			if(extraHealActive && Player.HasBuff(BuffID.PotionSickness))
			{
				int index = Player.FindBuffIndex(BuffID.PotionSickness);

				if (Player.buffTime[index] == 60 * 30)
				{
					Player.Heal(50);

					for(int k = 0; k < 40; k++)
					{
						var d = Dust.NewDustPerfect(Player.Center, ModContent.DustType<Dusts.GlowFollowPlayer>(), Main.rand.NextVector2Circular(3, 3), 0, new Color(50 + Main.rand.Next(100), 50 + Main.rand.Next(100), 255, 0), Main.rand.NextFloat(0.4f, 0.6f));
						d.customData = new object[] { Player, Vector2.Zero };

						Dust.NewDustPerfect(Player.Center, DustID.Water, Main.rand.NextVector2Circular(5, 5));
					}

					SoundEngine.PlaySound(SoundID.Splash.WithPitchOffset(0.5f));
					SoundEngine.PlaySound(SoundID.Shatter.WithPitchOffset(0.8f));
				}
			}
		}

		public override void ResetEffects()
		{
			extraHealActive = false;
		}
	}
}
