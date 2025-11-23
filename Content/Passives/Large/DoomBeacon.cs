using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class DoomBeacon : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.DoomBeacon;
			difficulty = 25;
			size = 1;
		}

		public override void Update()
		{
			TreeNPCGlobals.spawnRateModifier += 0.01f * (TreeSystem.tree.difficulty / 5);
		}
	}
}
