using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;

namespace Umbra.Core.TreeSystem
{
	internal class TreeProjectile : GlobalProjectile
	{
		public TreeNPC spawner;

		public override bool InstancePerEntity => true;

		public override void OnSpawn(Projectile projectile, IEntitySource source)
		{
			if (source is EntitySource_Parent sourcez && sourcez.Entity is NPC npc)
			{
				spawner = npc.GetGlobalNPC<TreeNPC>();

				projectile.damage += spawner.flatDamage;
				projectile.damage += (int)(projectile.damage * spawner.increasedDamage);
				foreach (float more in spawner.moreDamage)
				{
					projectile.damage += (int)(projectile.damage * more);
				}
			}
		}

		public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
		{
			if (spawner != null)
			{
				foreach (KeyValuePair<int, float> pair in spawner.statusChances)
				{
					if (Main.rand.NextFloat() < pair.Value)
						target.AddBuff(pair.Key, spawner.statusDuration);
				}
			}
		}
	}
}
