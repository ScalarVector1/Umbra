using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Umbra.Content.Items;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class VengefulShades : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.VengefulShades;
			difficulty = 30;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			if (!npc.boss && npc.type != NPCID.Wraith && Main.rand.NextBool(10))
			{
				npc.GetGlobalNPC<VengefulShadesNPC>().active = true;
			}
		}
	}

	internal class VengefulShadesNPC : GlobalNPC
	{
		public bool active;

		public override bool InstancePerEntity => true;

		public override void OnKill(NPC npc)
		{
			if (active)
			{
				NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.Wraith);
			}

			if (npc.type == NPCID.Wraith && ModContent.GetInstance<TreeSystem>().tree.Nodes.Any(n => n.active && n is VengefulShades))
			{
				Item.NewItem(npc.GetSource_Death(), npc.Hitbox, ModContent.ItemType<UmbraPickup>());
			}
		}
	}
}
