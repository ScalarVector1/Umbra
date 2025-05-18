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
	internal class Comorbidity : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.Comorbidity;
			difficulty = 25;
			size = 1;
		}

		public override void BuffPlayer(Player player)
		{
			if (player.HasBuff(BuffID.Poisoned))
			{
				int index = player.FindBuffIndex(BuffID.Poisoned);
				int duration = player.buffTime[index];

				if (!player.HasBuff(BuffID.Venom))
				{
					player.AddBuff(BuffID.Venom, duration);

					int venomIndex = player.FindBuffIndex(BuffID.Venom);
					player.buffTime[venomIndex] = duration;
				}
				else
				{
					int venomIndex = player.FindBuffIndex(BuffID.Venom);
					int venomDuration = player.buffTime[venomIndex];

					if (venomDuration < duration)
						player.buffTime[venomIndex] = duration;
				}
			}

			if (player.HasBuff(BuffID.Venom))
			{
				int index = player.FindBuffIndex(BuffID.Venom);
				int duration = player.buffTime[index];

				if (!player.HasBuff(BuffID.Poisoned))
				{
					player.AddBuff(BuffID.Poisoned, duration);

					int poisonIndex = player.FindBuffIndex(BuffID.Poisoned);
					player.buffTime[poisonIndex] = duration;
				}
				else
				{
					int poisonIndex = player.FindBuffIndex(BuffID.Poisoned);
					int poisonDuration = player.buffTime[poisonIndex];

					if (poisonDuration < duration)
						player.buffTime[poisonIndex] = duration;
				}
			}
		}
	}
}
