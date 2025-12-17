using System.Linq;
using Terraria.ID;
using Umbra.Compat;
using Umbra.Content.Items;
using Umbra.Content.Projectiles;
using Umbra.Core.PassiveTreeSystem;

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
			if (!ExtraBossMarks.DoICountAsABoss(npc) && npc.type != NPCID.Wraith && Main.rand.NextBool(10))
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
			if (active && npc.type != NPCID.Wraith && !npc.SpawnedFromStatue)
			{
				Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Spawner>(), 0, 0, Main.myPlayer, NPCID.Wraith, 1f);
			}

			if (npc.type == NPCID.Wraith && TreeSystem.tree.Nodes.Any(n => n.active && n is VengefulShades))
			{
				Item.NewItem(npc.GetSource_Death(), npc.Hitbox, ModContent.ItemType<UmbraPickup>());
			}
		}
	}
}
