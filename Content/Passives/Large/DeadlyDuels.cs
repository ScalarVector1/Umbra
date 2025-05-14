using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Umbra.Content.DropConditions;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class DeadlyDuels : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.DeadlyDuels;
			difficulty = 30;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			if (npc.boss)
				npc.GetGlobalNPC<TreeNPC>().moreDamage.Add(1.3f);
		}
	}

	internal class DeadlyDuelsNPC : GlobalNPC
	{
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
		{
			return entity.boss;
		}

		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			List<IItemDropRule> loots = npcLoot.Get();

			foreach (IItemDropRule loot in loots)
			{
				List<DropRateInfo> drops = [];
				DropRateInfoChainFeed ratesInfo = new();
				loot.ReportDroprates(drops, ratesInfo);

				foreach (DropRateInfo drop in drops)
				{
					if (ItemID.Sets.BossBag[drop.itemId])
					{
						npcLoot.Add(ItemDropRule.ByCondition(new AnyPassiveAllocationCondition<DeadlyDuels>(), drop.itemId));
						return;
					}
				}
			}
		}
	}
}
