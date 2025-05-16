using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class SicklyPotions : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.SicklyPotions;
			difficulty = 20;
			size = 1;
		}
	}

	internal class SicklyPotionsPlayer : ModPlayer
	{
		public int healingToDeliver;
		public int tickTimer;

		public bool Active => ModContent.GetInstance<TreeSystem>().tree.Nodes.Any(n => n is SicklyPotions && n.active);

		public override void Load()
		{
			On_Player.ApplyLifeAndOrMana += GradualInstead;
		}

		private void GradualInstead(On_Player.orig_ApplyLifeAndOrMana orig, Player self, Item item)
		{
			if (!Active || item.healLife <= 0)
			{
				orig(self, item);
				return;
			}

			if (item.healLife > 0)
			{
				self.GetModPlayer<SicklyPotionsPlayer>().healingToDeliver += self.GetHealLife(item, true);
			}

			var clone = item.Clone();
			clone.healLife = 0;
			orig(self, clone);
		}

		public override void PostUpdateEquips()
		{
			if (healingToDeliver > 0)
			{
				tickTimer++;
				if (tickTimer >= 5)
				{
					int amount = Math.Min(healingToDeliver, 2);
					Player.Heal(amount);
					healingToDeliver -= amount;

					tickTimer = 0;
				}
			}
			else
			{
				tickTimer = 0;
			}
		}
	}
}
