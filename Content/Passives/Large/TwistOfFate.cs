using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class TwistOfFate : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.TwistOfFate;
			difficulty = 0;
			size = 1;
			contributesToTooltips = false;
		}
	}
}
