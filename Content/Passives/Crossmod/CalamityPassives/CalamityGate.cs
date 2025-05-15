using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Passives.Crossmod.CalamityPassives
{
	internal class CalamityGate : ModGate
	{
		public CalamityGate() : base("Calamity") { }

		public override void SetDefaults()
		{
			texture = Assets.Passives.CalamityGate;
			size = 1;
		}
	}

	internal abstract class CalamityPassive : CrossmodPassive
	{
		public CalamityPassive() : base("Calamity") { }
	}
}
