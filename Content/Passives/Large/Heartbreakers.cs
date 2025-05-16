using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class Heartbreakers : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.Heartbreakers;
			difficulty = 30;
			size = 1;
		}
	}

	internal class HeartbreakersPlayer : ModPlayer
	{
		public int maxPenalty;
		public int tickTimer;

		public bool Active => ModContent.GetInstance<TreeSystem>().tree.Nodes.Any(n => n is Heartbreakers && n.active);

		public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
		{
			base.ModifyMaxStats(out health, out mana);
			health.Base -= maxPenalty;
		}

		public override void PostUpdateEquips()
		{
			if (maxPenalty > 0)
			{
				tickTimer++;

				if (tickTimer > 10)
				{
					maxPenalty--;
					tickTimer = 0;
				}
			}
			else
			{
				tickTimer = 0;
			}
		}

		public override void PostHurt(Player.HurtInfo info)
		{
			if (Active)
				maxPenalty += 20;
		}
	}
}
