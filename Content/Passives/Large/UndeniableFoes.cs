using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class UndeniableFoes : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.UndeniableFoes;
			difficulty = 20;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.knockBackResist = 0f;
		}
	}
}
