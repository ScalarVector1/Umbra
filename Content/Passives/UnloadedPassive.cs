using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
	internal class UnloadedPassive : Passive
	{
		public string savedType;

		public override void SetDefaults()
		{
			texture = Assets.Passives.UnloadedPassive;
			contributesToTooltips = false;
		}
	}
}
