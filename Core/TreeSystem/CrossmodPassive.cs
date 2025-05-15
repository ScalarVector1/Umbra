using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbra.Core.TreeSystem
{
	public abstract class CrossmodPassive : Passive
	{
		public List<string> modsRequired = [];

		public CrossmodPassive(params string[] mods) : base()
		{
			modsRequired.AddRange(mods);

			foreach (string name in modsRequired)
			{
				if (!ModLoader.HasMod(name))
					opacity = 0.33f;
			}
		}

		public override bool CanAllocate(Player player)
		{
			foreach (string name in modsRequired)
			{
				if (!ModLoader.HasMod(name))
					return false;
			}

			return base.CanAllocate(player);
		}

		public override bool CanBeActive()
		{
			foreach (string name in modsRequired)
			{
				if (!ModLoader.HasMod(name))
					return false;
			}

			return base.CanBeActive();
		}
	}
}
