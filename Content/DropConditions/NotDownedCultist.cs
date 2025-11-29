using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Umbra.Content.DropConditions
{
	internal class NotDownedCultist : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info)
		{
			return !NPC.downedAncientCultist;
		}

		public bool CanShowItemDropInUI()
		{
			return true;
		}

		public string GetConditionDescription()
		{
			return "";
		}
	}
}
