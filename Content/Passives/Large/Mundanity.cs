using Umbra.Content.Items;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class Mundanity : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.Mundanity;
			difficulty = 150;
			size = 2;
		}

		public override void BuffPlayer(Player player)
		{
			if (player.armor[3].ModItem is not MundanityItem)
			{
				if (!player.armor[3].IsAir)
					player.QuickSpawnItemDirect(player.GetSource_DropAsItem(), player.armor[3].Clone());

				player.armor[3].SetDefaults(ModContent.ItemType<MundanityItem>());
			}

			if (player.armor[4].ModItem is not MundanityItem)
			{
				if (!player.armor[4].IsAir)
					player.QuickSpawnItemDirect(player.GetSource_DropAsItem(), player.armor[4].Clone());

				player.armor[4].SetDefaults(ModContent.ItemType<MundanityItem>());
			}
		}
	}
}
