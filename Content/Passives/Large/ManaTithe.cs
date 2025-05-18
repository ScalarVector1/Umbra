using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;
using static AssGen.Assets;

namespace Umbra.Content.Passives.Large
{
	internal class ManaTithe : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.ManaTithe;
			difficulty = 50;
			size = 1;
		}

		public override void Update()
		{
			FlatManaCostSystem.flatCostToAdd += 4;
		}
	}
}
