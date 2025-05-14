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
		public static float UmbraChance => 0.02f + MathF.Atan(ModContent.GetInstance<TreeSystem.TreeSystem>().tree.difficulty * 0.01f);

		public override void ModifyGlobalLoot(GlobalLoot globalLoot)
		{
			globalLoot.Add(ItemDropRule.Common(ModContent.ItemType<UmbraPickup>(), (int)(1f / UmbraChance)));
		}
	}
}
