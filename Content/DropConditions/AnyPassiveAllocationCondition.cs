using Terraria.GameContent.ItemDropRules;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.DropConditions
{
	internal class AnyPassiveAllocationCondition<T> : IItemDropRuleCondition where T : Passive
	{
		public bool CanDrop(DropAttemptInfo info)
		{
			return TreeSystem.tree.AnyActive<T>();
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
