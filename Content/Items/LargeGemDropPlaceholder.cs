using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Umbra.Content.DropConditions;
using Umbra.Content.Items.Slottables;

namespace Umbra.Content.Items
{
	internal class LargeGemDropPlaceholder : ModItem
	{
		public override string Texture => "Umbra/Assets/Items/LargeGemDropPlaceholder";

		public override void OnCreated(ItemCreationContext context)
		{
			if (context is not InitializationItemCreationContext)
				Item.SetDefaults(ModContent.ItemType<LargeGem>());
		}

		public override void OnSpawn(IEntitySource source)
		{
			Item.SetDefaults(ModContent.ItemType<LargeGem>());
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			Item.SetDefaults(ModContent.ItemType<LargeGem>());
		}

		public override void UpdateInventory(Player player)
		{
			Item.SetDefaults(ModContent.ItemType<LargeGem>());
		}
	}

	internal class LargeGemDrops : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			if (npc.type == NPCID.WallofFlesh)
			{
				npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsPreHardmode(), ModContent.ItemType<LargeGemDropPlaceholder>()));
			}

			if (npc.type == NPCID.Plantera)
			{
				npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FirstTimeKillingPlantera(), ModContent.ItemType<LargeGemDropPlaceholder>()));
			}

			if (npc.type == NPCID.CultistBoss)
			{
				npcLoot.Add(ItemDropRule.ByCondition(new NotDownedCultist(), ModContent.ItemType<LargeGemDropPlaceholder>()));
			}

			if (npc.type == NPCID.HallowBoss)
			{
				npcLoot.Add(ItemDropRule.ByCondition(new Conditions.EmpressOfLightIsGenuinelyEnraged(), ModContent.ItemType<LargeGemDropPlaceholder>()));
			}

			if (npc.type == NPCID.MoonLordCore)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LargeGemDropPlaceholder>()));
			}
		}
	}
}
