using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;

namespace Umbra.Core
{
	internal class StatusChanceNPC : GlobalNPC
	{
		public Dictionary<int, float> statusChances = [];
		public int statusDuration = 180;

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
		{
			return !entity.friendly && entity.lifeMax > 5 && !NPCID.Sets.CountsAsCritter[entity.type] && entity.damage > 0;
		}

		public void AddStatusChance(int type, float chance)
		{
			if (statusChances.ContainsKey(type))
				statusChances[type] += chance;
			else
				statusChances.Add(type, chance);
		}

		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			foreach (KeyValuePair<int, float> pair in statusChances)
			{
				if (Main.rand.NextFloat() < pair.Value)
					target.AddBuff(pair.Key, statusDuration);
			}
		}
	}

	internal class StatusChanceProjectile : GlobalProjectile
	{
		public StatusChanceNPC spawner;

		public override bool InstancePerEntity => true;

		public override void OnSpawn(Projectile projectile, IEntitySource source)
		{
			if (source is EntitySource_Parent sourcez && sourcez.Entity is NPC npc && npc.TryGetGlobalNPC<StatusChanceNPC>(out StatusChanceNPC statusNPC))
			{
				spawner = statusNPC;
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
