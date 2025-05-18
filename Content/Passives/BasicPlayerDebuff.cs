using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
	internal class PlayerWither : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.PlayerWither;
			difficulty = 4;
		}

		public override void BuffPlayer(Player player)
		{
			player.GetModPlayer<RegenerationPlayer>().increasedRegen -= 0.05f;
		}
	}

	internal class ManaCost : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.ManaCost;
			difficulty = 10;
		}

		public override void Update()
		{
			FlatManaCostSystem.flatCostToAdd += 1;
		}
	}
}
