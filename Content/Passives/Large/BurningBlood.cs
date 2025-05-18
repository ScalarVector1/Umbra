using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class BurningBlood : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.BurningBlood;
			difficulty = 15;
			size = 1;
		}

		public override void BuffPlayer(Player player)
		{
			if (player.HasBuff(BuffID.Bleeding))
			{
				int index = player.FindBuffIndex(BuffID.Bleeding);
				int duration = player.buffTime[index];

				if (!player.HasBuff(BuffID.OnFire))
				{
					player.AddBuff(BuffID.OnFire, duration);

					int fireIndex = player.FindBuffIndex(BuffID.OnFire);
					player.buffTime[fireIndex] = duration;
				}
				else
				{
					int fireIndex = player.FindBuffIndex(BuffID.OnFire);
					int fireDuration = player.buffTime[fireIndex];

					if (fireDuration < duration)
						player.buffTime[fireIndex] = duration;
				}
			}
		}
	}
}
