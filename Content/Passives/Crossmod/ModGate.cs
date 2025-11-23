using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Crossmod
{
	internal abstract class ModGate : CrossmodPassive
	{
		public override bool AllowDuplicates => true;

		public ModGate(params string[] mods) : base(mods)
		{
			foreach (string name in modsRequired)
			{
				opacity = 1f;
			}
		}
	}
}
