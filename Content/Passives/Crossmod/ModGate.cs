using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Crossmod
{
	internal abstract class ModGate : CrossmodPassive
	{
		public ModGate(params string[] mods) : base(mods)
		{
			foreach (string name in modsRequired)
			{
				opacity = 1f;
			}
		}
	}
}
