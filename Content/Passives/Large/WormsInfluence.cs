using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class WormsInfluence : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.WormsInfluence;
			difficulty = 35;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<TreeNPC>().endurance += 0.15f;

			if (npc.aiStyle == NPCAIStyleID.Worm || npc.aiStyle == NPCAIStyleID.TheDestroyer)
				npc.GetGlobalNPC<TreeNPC>().endurance += 0.1f;
		}
	}
}
