using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.TreeSystem;
using Umbra.Core;

namespace Umbra.Content.Passives
{
	internal class ManaDamage : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.ManaDamage;
			difficulty = 4;
		}

		public override void BuffPlayer(Player player)
		{
			player.GetModPlayer<ManaStealPlayer>().lostOnHit += 10;
		}
	}

	internal class ManaStealPlayer : ModPlayer
	{
		public int lostOnHit;

		public override void OnHurt(Player.HurtInfo info)
		{
			Player.statMana -= lostOnHit;
		}

		public override void ResetEffects()
		{
			lostOnHit = 0;
		}
	}
}
