using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class SpontaneousGeneration : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.SpontaneousGeneration;
			difficulty = 50;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			if (!npc.boss && Main.rand.NextBool(10))
			{
				npc.GetGlobalNPC<SpontaneousGenerationNPC>().active = true;
			}
		}
	}

	internal class SpontaneousGenerationNPC : GlobalNPC
	{
		public bool active;

		public override bool InstancePerEntity => true;

		public override void OnKill(NPC npc)
		{
			if (active)
			{
				for (int k = 0; k < 2; k++)
				{
					int i = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, npc.type);
					Main.npc[i].lifeMax = npc.lifeMax / 2;
				}
			}
		}
	}
}
