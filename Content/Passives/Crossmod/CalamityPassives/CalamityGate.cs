using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Crossmod.CalamityPassives
{
	internal class CalamityGate : ModGate
	{
		public CalamityGate() : base("CalamityMod") { }

		public override void SetDefaults()
		{
			texture = Assets.Passives.CalamityGate;
			size = 1;
		}
	}

	internal abstract class CalamityPassive : CrossmodPassive
	{
		public CalamityPassive() : base("CalamityMod") { }
	}
}
