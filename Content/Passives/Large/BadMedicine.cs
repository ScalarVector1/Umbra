using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class BadMedicine : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.BadMedicine;
			difficulty = 20;
			size = 1;
		}
	}

	internal class BadMedicineItem : GlobalItem
	{
		public bool Active => ModContent.GetInstance<TreeSystem>().tree.Nodes.Any(n => n is BadMedicine && n.active);

		public override bool AppliesToEntity(Item entity, bool lateInstantiation)
		{
			return entity.healLife > 0;
		}

		public override bool? UseItem(Item item, Player player)
		{
			if (Active)
				player.AddBuff(ModContent.BuffType<Doomed>(), 300);

			return null;
		}
	}
}
