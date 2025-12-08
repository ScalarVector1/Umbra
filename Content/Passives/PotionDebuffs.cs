using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
	internal class WeaknessWhilePotion : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.WeaknessWhilePotion;
			difficulty = 8;
		}

		public override void BuffPlayer(Player player)
		{
			player.GetModPlayer<PotionDebuffPlayer>().allDamageWhilePotion -= 0.02f;
		}
	}

	internal class DefenseWhilePotion : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.DefenseWhilePotion;
			difficulty = 8;
		}

		public override void BuffPlayer(Player player)
		{
			player.GetModPlayer<PotionDebuffPlayer>().defenseWhilePotion -= 2;
		}
	}

	public class PotionDebuffPlayer : ModPlayer
	{
		public float allDamageWhilePotion;
		public int defenseWhilePotion;

		public override void PostUpdateEquips()
		{
			if (Player.HasBuff(BuffID.PotionSickness))
			{
				Player.allDamage += allDamageWhilePotion;
				Player.statDefense += defenseWhilePotion;
			}
		}

		public override void ResetEffects()
		{
			allDamageWhilePotion = 0;
			defenseWhilePotion = 0;
		}
	}
}
