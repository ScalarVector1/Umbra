using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class TemporalTrance : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.TemporalTrance;
			difficulty = 100;
			size = 2;
		}
	}

	internal class TemporalTranceSystem : ModSystem
	{
		public static bool dontUpdateUI;

		public override void Load()
		{
			On_Main.DoUpdate += TemporalTranceSpeedup;
			On_Main.UpdateUIStates += DontUpdateUIExtra;
		}

		private void DontUpdateUIExtra(On_Main.orig_UpdateUIStates orig, GameTime gameTime)
		{
			if (!dontUpdateUI)
				orig(gameTime);
			else
				dontUpdateUI = false;
		}

		private void TemporalTranceSpeedup(On_Main.orig_DoUpdate orig, Main self, ref GameTime gameTime)
		{
			orig(self, ref gameTime);

			if (!Main.gameMenu && TreeSystem.tree.AnyActive<TemporalTrance>() && Main.GameUpdateCount % 4 == 0)
			{
				dontUpdateUI = true;
				orig(self, ref gameTime);
			}
		}
	}
}
