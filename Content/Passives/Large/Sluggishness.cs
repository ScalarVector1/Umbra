using Terraria.ID;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class Sluggishness : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.Sluggishness;
			difficulty = 25;
			size = 1;
		}
	}

	internal class SluggishnessItem : GlobalItem
	{
		public override float UseSpeedMultiplier(Item item, Player player)
		{
			if (TreeSystem.tree.AnyActive<Sluggishness>())
			{
				if (player.HasBuff(BuffID.Slow))
					return 0.8f;

				return 0.9f;
			}

			return 1f;
		}
	}
}
