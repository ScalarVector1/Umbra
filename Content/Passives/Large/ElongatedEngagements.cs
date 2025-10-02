using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Umbra.Content.DropConditions;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
    internal class ElongatedEngagements : Passive
    {
        public override void SetDefaults()
        {
            texture = Assets.Passives.ElongatedEngagements;
            difficulty = 30;
            size = 1;
        }

        public override void OnEnemySpawn(NPC npc)
        {
            if (npc.boss)
                npc.GetGlobalNPC<TreeNPC>().moreLife.Add(0.5f);
        }
    }

    internal class ElongatedEngagementsNPC : GlobalNPC
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
                        npcLoot.Add(ItemDropRule.ByCondition(new AnyPassiveAllocationCondition<ElongatedEngagements>(), drop.itemId));
                        return;
                    }
                }
            }
        }
    }
}
