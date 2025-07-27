using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
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

				CombatText.NewText(Player.Hitbox, new Color(200, 160, 255), Language.GetTextValue("Mods.Umbra.Misc.UmbraGainPopup"));

				for (int k = 0; k < 20; k++)
				{
					var d = Dust.NewDustPerfect(Player.Center, DustID.FireworksRGB, Vector2.UnitX.RotatedByRandom(6.28f) * Main.rand.NextFloat(7f), 255, new Color(210, 160, 255), Main.rand.NextFloat(0.3f, 0.8f));
					d.noGravity = true;
				}

				SoundEngine.PlaySound(SoundID.DD2_BookStaffCast.WithPitchOffset(-0.1f).WithVolume(0.5f));
				SoundEngine.PlaySound(SoundID.GuitarAm.WithVolume(0.4f).WithPitchOffset(-0.4f));
				SoundEngine.PlaySound(SoundID.DrumKick);

				UmbraNet.SyncPoints(Player.whoAmI);
			}

			foreach (Passive passive in TreeSystem.tree.activeNodes)
			{
				passive.BuffPlayer(Player);
			}
		}

		public override void OnEnterWorld()
		{
			UmbraNet.RequestTreeOnJoin();
			UmbraNet.SyncPoints(Player.whoAmI);
			Main.NewText(Language.GetText("Mods.Umbra.Misc.Motd").Format(Mod.Version), new Color(225, 200, 255));
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
