using Umbra.Core.PassiveTreeSystem;

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
		public bool Active => TreeSystem.tree.AnyActive<BadMedicine>();

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
