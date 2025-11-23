using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Crossmod.StarlightRiverPassives
{
	internal class StarlightRiverGate : ModGate
	{
		public StarlightRiverGate() : base("StarlightRiver") { }

		public override void SetDefaults()
		{
			texture = Assets.Passives.StarlightRiverGate;
			size = 1;
			contributesToTooltips = false;
		}
	}

	internal abstract class StarlightRiverPassive : CrossmodPassive
	{
		public StarlightRiverPassive() : base("StarlightRiver") { }
	}
}
