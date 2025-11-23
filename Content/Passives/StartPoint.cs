using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
	internal class StartPoint : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.StartPassive;
			difficulty = 0;
			size = 2;
			contributesToTooltips = false;
		}

		public override bool CanDeallocate(Player player)
		{
			return false;
		}
	}
}
