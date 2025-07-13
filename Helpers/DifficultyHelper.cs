using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbra.Helpers
{
	internal class DifficultyHelper
	{
		/// <summary>
		/// Helper to get the desired number to pass as projectile damage for the resultant values for
		/// each difficulty. Normally the game applies multipliers to projectile values making them hard
		/// to predict for a given difficulty.
		/// </summary>
		/// <param name="normal">The desired final damage on normal mode</param>
		/// <param name="expert">The desired final damage on expert mode</param>
		/// <param name="master">The desired final damage on master mode</param>
		/// <returns></returns>
		public static int GetProjectileDamage(int normal, int expert, int master)
		{
			return Main.masterMode ? master / 6 : Main.expertMode ? expert / 4 : normal / 2;
		}

		/// <summary>
		/// Helper to get the desired number to pass as projectile damage for the resultant values for
		/// each difficulty. Normally the game applies multipliers to projectile values making them hard
		/// to predict for a given difficulty.
		/// </summary>
		/// <param name="allDifficulties">The desired final damage for all difficulties</param>
		/// <returns></returns>
		public static int GetProjectileDamage(int allDifficulties)
		{
			return Main.masterMode ? allDifficulties / 6 : Main.expertMode ? allDifficulties / 4 : allDifficulties / 2;
		}
	}
}
