using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Umbra.Content.Items
{
	internal class BasicSetting : ModItem
	{
		public override string Texture => "Umbra/Assets/Items/GemFakeBase";

		public override void SetDefaults()
		{
			Item.width = 42;
			Item.height = 42;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddRecipeGroup(RecipeGroupID.IronBar, 10)
			.AddTile(TileID.Anvils)
			.Register();
		}
	}
}
