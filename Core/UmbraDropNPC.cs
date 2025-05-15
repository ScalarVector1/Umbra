using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Umbra.Content.Items;
using Umbra.Core.TreeSystem;

namespace Umbra.Core
{
	internal class UmbraDropNPC : GlobalNPC
	{
		public static float UmbraChance => 0.02f + 0.25f * MathF.Atan(ModContent.GetInstance<TreeSystem.TreeSystem>().tree.difficulty * 0.0025f);

		public override void OnKill(NPC npc)
		{
			int rolls = npc.boss ? 10 : !npc.friendly ? 1 : 0;

			for (int k = 0; k < rolls; k++)
			{
				if (Main.rand.NextFloat() <= UmbraChance)
					Item.NewItem(npc.GetSource_Death(), npc.Hitbox, ModContent.ItemType<UmbraPickup>());
			}
		}
	}
}
