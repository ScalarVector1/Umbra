using Terraria.DataStructures;
using Terraria.ID;
using Umbra.Content.Items.Slottables;

namespace Umbra.Content.Items
{
	internal class CraftableGemCraft : ModItem
	{
		public override string Texture => "Umbra/Assets/Items/CraftableGemCraft";

		public override void OnCreated(ItemCreationContext context)
		{
			if(context is RecipeItemCreationContext)
				Item.SetDefaults(ModContent.ItemType<CraftedGem>());
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<BasicSetting>())
			.AddIngredient(ItemID.PlatinumCoin)
			.AddIngredient(ItemID.Amethyst, 5)
			.AddIngredient(ItemID.Topaz, 5)
			.AddIngredient(ItemID.Sapphire, 5)
			.AddIngredient(ItemID.Emerald, 5)
			.AddIngredient(ItemID.Ruby, 5)
			.AddIngredient(ItemID.Diamond, 5)
			.Register();
		}
	}
}
