using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Passives.Crossmod.StarlightRiverPassives
{
	internal class StarlightRiverGate : ModGate
	{
		public StarlightRiverGate() : base("StarlightRiver") { }

		public override void SetDefaults()
		{
			texture = Assets.Passives.StarlightRiverGate;
			size = 1;
		}
	}

	internal abstract class StarlightRiverPassive : CrossmodPassive
	{
		public StarlightRiverPassive() : base("StarlightRiver") { }
	}
}
