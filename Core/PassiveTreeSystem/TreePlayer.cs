using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Umbra.Core.PassiveTreeSystem
{
	internal class TreePlayer : ModPlayer
	{
		public int UmbraPoints;
		public bool firstPoint;

		public int partialPoints;
		public int nextPoint = 8;

		public override void UpdateEquips()
		{
			if (partialPoints >= nextPoint)
			{
				partialPoints -= nextPoint;
				UmbraPoints += 1;

				if (nextPoint < 40)
					nextPoint += 2;

				CombatText.NewText(Player.Hitbox, new Color(200, 160, 255), "+1 Umbra");

				for (int k = 0; k < 20; k++)
				{
					var d = Dust.NewDustPerfect(Player.Center, DustID.FireworksRGB, Vector2.UnitX.RotatedByRandom(6.28f) * Main.rand.NextFloat(7f), 255, new Color(210, 160, 255), Main.rand.NextFloat(0.3f, 0.8f));
					d.noGravity = true;
				}

				SoundEngine.PlaySound(SoundID.DD2_BookStaffCast.WithPitchOffset(-0.1f).WithVolume(0.5f));
				SoundEngine.PlaySound(SoundID.GuitarAm.WithVolume(0.4f).WithPitchOffset(-0.4f));
				SoundEngine.PlaySound(SoundID.DrumKick);
			}

			foreach (Passive passive in TreeSystem.tree.activeNodes)
			{
				passive.BuffPlayer(Player);
			}
		}

		public override void OnEnterWorld()
		{
			UmbraNet.RequestTreeOnJoin();
			Main.NewText("Thanks for checking out out the Umbra Pre-release! This is a very early demo being used to gauge interest and find issues. PLEASE report any issues to https://discord.gg/KFpCz5p688 or https://github.com/ScalarVector1/Umbra/issues", new Color(200, 150, 255));
		}

		public override void SaveData(TagCompound tag)
		{
			tag["points"] = UmbraPoints;
			tag["firstPoint"] = firstPoint;
			tag["partialPoints"] = partialPoints;
			tag["nextPoint"] = nextPoint;
		}

		public override void LoadData(TagCompound tag)
		{
			UmbraPoints = tag.GetInt("points");
			firstPoint = tag.GetBool("firstPoint");
			partialPoints = tag.GetInt("partialPoints");
			nextPoint = tag.GetInt("nextPoint");
		}
	}
}
