using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Umbra.Content.Projectiles;

namespace Umbra.Core
{
	internal class ArmorPenetrationNPC : GlobalNPC
	{
		public int penetrate;

		public override bool InstancePerEntity => true;

		public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
		{
			modifiers.ArmorPenetration += penetrate;
		}
	}

	internal class ArmorPenetrationProjectile : GlobalProjectile
	{
		public int penetrate;

		public override bool InstancePerEntity => true;

		public override void OnSpawn(Projectile projectile, IEntitySource source)
		{
			if (source is EntitySource_Parent sourcez && sourcez.Entity is NPC npc && npc.TryGetGlobalNPC<ArmorPenetrationNPC>(out var gnpc))
			{
				penetrate = gnpc.penetrate;
			}
		}

		public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
		{
			modifiers.ArmorPenetration += penetrate;
		}
	}
}
