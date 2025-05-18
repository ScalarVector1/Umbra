using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Umbra.Core;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class ManaFlux : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.ManaFlux;
			difficulty = 40;
			size = 1;
		}

		public override void Update()
		{
			FlatManaCostSystem.flatCostToAdd += 4;
		}
	}

	internal class ManaFluxPlayer : ModPlayer
	{
		public static bool Active => ModContent.GetInstance<TreeSystem>().tree.Nodes.Any(n => n is ManaFlux && n.active);

		public override void PostUpdate()
		{		
			if (Active && Player.HasBuff(BuffID.Bleeding) && Player.statMana > 0)
				Player.statMana--;
		}
	}
}
