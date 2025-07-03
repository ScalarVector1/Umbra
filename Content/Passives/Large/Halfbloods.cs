using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.GameContent.Achievements;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class Halfbloods : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.Halfbloods;
			difficulty = 50;
			size = 2;
		}

		public override void Update()
		{
			if (!Main.dayTime && Main.time >= (32400 / 2) && !Main.bloodMoon)
			{
				if (Main.netMode == 0)
				{
					AchievementsHelper.NotifyProgressionEvent(4);
					Main.bloodMoon = true;
					if (Main.GetMoonPhase() == MoonPhase.Empty)
						Main.moonPhase = 5;

					Main.NewText(Lang.misc[8].Value, 50, byte.MaxValue, 130);
				}
				else
				{
					NetMessage.SendData(61, -1, -1, null, Main.LocalPlayer.whoAmI, -10f);
				}
			}
		}
	}
}
