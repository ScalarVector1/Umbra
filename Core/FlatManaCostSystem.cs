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
			return entity.damage > 0 && entity.pick == 0 && entity.axe == 0 && entity.hammer == 0;
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
