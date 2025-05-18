using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class PanicGuard : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.PanicGuard;
			difficulty = 35;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<PanicGuardNPC>().active = true;
		}
	}

	internal class PanicGuardNPC : GlobalNPC
	{
		public bool active;
		public bool buffed;

		public override bool InstancePerEntity => true;

		public override bool PreAI(NPC npc)
		{
			if (active && !buffed && npc.life < npc.lifeMax / 3)
			{
				npc.defense += 15;
				buffed = true;
			}

			return true;
		}
	}
}
