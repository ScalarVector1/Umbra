using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Content.Buffs;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class CorrosiveConcoctions : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.CorrosiveConcoctions;
			difficulty = 50;
			size = 1;
		}
	}

	internal class CorrosiveConcoctionsItem : GlobalItem
	{
		public bool Active => TreeSystem.tree.AnyActive<CorrosiveConcoctions>();

		public override bool AppliesToEntity(Item entity, bool lateInstantiation)
		{
			return entity.healLife > 0;
		}

		public override bool? UseItem(Item item, Player player)
		{
			if (Active)
				player.AddBuff(ModContent.BuffType<SetShatterBuff>(), 1200);

			return null;
		}
	}
}
