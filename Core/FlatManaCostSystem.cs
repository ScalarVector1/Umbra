using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbra.Core
{
	internal class FlatManaCostSystem : ModSystem
	{
		public static int flatCostToAdd;

		public override void PostUpdateEverything()
		{
			flatCostToAdd = 0;
		}
	}

	internal class FlatManaCostItem : GlobalItem
	{
		public int lastFlatCostAdded;

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Item entity, bool lateInstantiation)
		{
			return entity.damage > 0;
		}

		public override void SetDefaults(Item entity)
		{
			if (lastFlatCostAdded != FlatManaCostSystem.flatCostToAdd)
			{
				entity.mana += FlatManaCostSystem.flatCostToAdd - lastFlatCostAdded;
				lastFlatCostAdded = FlatManaCostSystem.flatCostToAdd;
			}
		}

		public override void UpdateInventory(Item item, Player player)
		{
			if (lastFlatCostAdded != FlatManaCostSystem.flatCostToAdd)
			{
				item.mana += FlatManaCostSystem.flatCostToAdd - lastFlatCostAdded;
				lastFlatCostAdded = FlatManaCostSystem.flatCostToAdd;
			}
		}
	}
}
