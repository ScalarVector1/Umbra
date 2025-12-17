using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Umbra.Content.Buffs;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class SetShatter : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.SetShatter;
			difficulty = 35;
			size = 1;
		}
	}

	internal class SetShatterPlayer : ModPlayer
	{
		public override void OnHurt(Player.HurtInfo info)
		{
			if (TreeSystem.tree.AnyActive<SetShatter>() && Player.statLife < Player.statLifeMax2 / 2)
				Player.AddBuff(ModContent.BuffType<SetShatterBuff>(), 600);
		}
	}
}
