using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
    internal class PotionSickness : Passive
    {
        public override void SetDefaults()
        {
            texture = Assets.Passives.PotionSickness;
            difficulty = 5;
        }

        public override void BuffPlayer(Player player)
        {
            player.potionDelayTime += 300;
        }
    }

    internal class PotionHealing : Passive
    {
        public override void SetDefaults()
        {
            texture = Assets.Passives.PotionHealing;
            difficulty = 5;
        }

        public override void BuffPlayer(Player player)
        {
            player.GetModPlayer<PotionStats>().flatHealingDecrease += 5;
        }
    }

    internal class PotionStats : ModPlayer
    {
        public int flatHealingDecrease;
        public float increasedHealingDecrease;

        public override void ResetEffects()
        {
            flatHealingDecrease = 0;
            increasedHealingDecrease = 0;
        }
    }

    internal class PotionItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.healLife > 0;
        }

        public override void GetHealLife(Item item, Player player, bool quickHeal, ref int healValue)
        {
            var potionStats = player.GetModPlayer<PotionStats>();

            int healingDecrease = potionStats.flatHealingDecrease;
            healingDecrease += (int)(healingDecrease * potionStats.increasedHealingDecrease);

            healValue -= healingDecrease;
        }
    }
}
