using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Umbra.Core.TreeSystem;

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
	}

	internal class ManaTitheItem : GlobalItem
	{
		public bool penaltyApplied;
		public static bool Active => ModContent.GetInstance<TreeSystem>().tree.Nodes.Any(n => n is ManaTithe && n.active);

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Item entity, bool lateInstantiation)
		{
			return entity.damage > 0;
		}

		public override void SetDefaults(Item entity)
		{
			if (Active && !penaltyApplied)
			{
				entity.mana += 4;
				penaltyApplied = true;
			}
		}

		public override void UpdateInventory(Item item, Player player)
		{
			if (Active && !penaltyApplied)
			{
				item.mana += 4;
				penaltyApplied = true;
			}

			if (penaltyApplied && !Active)
			{
				item.mana -= 4;
				penaltyApplied = false;
			}
		}
	}
}
