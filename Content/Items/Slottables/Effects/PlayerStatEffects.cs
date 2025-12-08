using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Umbra.Content.Items.Slottables.Effects
{
	internal class PlayerLifeEffect : SlottableEffect
	{
		public override string Tooltip => Language.GetOrRegister(TooltipKey).Format(tier * 10);

		public override void BuffPlayer(Player player)
		{
			player.statLifeMax2 += tier * 10;
		}
	}

	internal class PlayerManaEffect : SlottableEffect
	{
		public override string Tooltip => Language.GetOrRegister(TooltipKey).Format(tier * 10);

		public override void BuffPlayer(Player player)
		{
			player.statManaMax2 += tier * 10;
		}
	}

	internal class PlayerLifeRegenEffect : SlottableEffect
	{
		public override string Tooltip => Language.GetOrRegister(TooltipKey).Format(tier);

		public override void BuffPlayer(Player player)
		{
			player.lifeRegen += tier;
		}
	}

	internal class PlayerManaRegenEffect : SlottableEffect
	{
		public override string Tooltip => Language.GetOrRegister(TooltipKey).Format(tier);

		public override void BuffPlayer(Player player)
		{
			player.manaRegen += tier;
		}
	}

	internal class PlayerDefenseEffect : SlottableEffect
	{
		public override string Tooltip => Language.GetOrRegister(TooltipKey).Format(tier * 2);

		public override void BuffPlayer(Player player)
		{
			player.statDefense += tier * 2;
		}
	}

	internal class PlayerEnduranceEffect : SlottableEffect
	{
		public override string Tooltip => Language.GetOrRegister(TooltipKey).Format(tier);

		public override void BuffPlayer(Player player)
		{
			player.endurance += tier * 0.01f;
		}
	}

	internal class PlayerSpeedEffect : SlottableEffect
	{
		public override string Tooltip => Language.GetOrRegister(TooltipKey).Format(tier * 3);

		public override void BuffPlayer(Player player)
		{
			player.moveSpeed += tier * 0.03f;
		}
	}

	internal class PlayerDamageEffect : SlottableEffect
	{
		public override string Tooltip => Language.GetOrRegister(TooltipKey).Format(tier * 4);

		public override void BuffPlayer(Player player)
		{
			player.allDamage += tier * 0.04f;
		}
	}

	internal class PlayerCritEffect : SlottableEffect
	{
		public override string Tooltip => Language.GetOrRegister(TooltipKey).Format(tier);

		public override void BuffPlayer(Player player)
		{
			player.allCrit += tier;
		}
	}

	internal class PlayerFlightEffect : SlottableEffect
	{
		public override string Tooltip => Language.GetOrRegister(TooltipKey).Format(tier);

		public override void BuffPlayer(Player player)
		{
			player.wingTimeMax += tier * 60;
		}
	}

	internal class PlayerKnockbackEffect : SlottableEffect
	{
		public override string Tooltip => Language.GetOrRegister(TooltipKey).Format(tier * 3);

		public override void BuffPlayer(Player player)
		{
			player.allKB += tier * 0.03f;
		}
	}

	internal class PlayerArmorPenetrateEffect : SlottableEffect
	{
		public override string Tooltip => Language.GetOrRegister(TooltipKey).Format(tier);

		public override void BuffPlayer(Player player)
		{
			player.armorPenetration += tier;
		}
	}
}
