using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.DropConditions
{
	internal class AnyPassiveAllocationCondition<T> : IItemDropRuleCondition where T : Passive
	{
		public bool CanDrop(DropAttemptInfo info)
		{
			return ModContent.GetInstance<TreeSystem>().tree.Nodes.Any(n => n.active && n is T);
		}

		public bool CanShowItemDropInUI()
		{
			return false;
		}

		public string GetConditionDescription()
		{
			return "";
		}
	}
}
