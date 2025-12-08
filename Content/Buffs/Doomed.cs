using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbra.Content.Buffs
{
	internal class Doomed : ModBuff
	{
		public override string Texture => "Umbra/Assets/Buffs/Doomed";

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}
	}

	internal class DoomedPlayer : ModPlayer
	{
		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (Player.HasBuff<Doomed>())
				modifiers.FinalDamage *= 1.5f;
		}
	}
}
