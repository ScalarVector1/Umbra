using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umbra.Core.TreeSystem;

namespace Umbra.Content.Passives
{
	internal class EnemyDamage : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyDamage;
			difficulty = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.damage += 2;
		}
	}

	internal class HealthPassive : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyHP;
			difficulty = 1;
		}
		public override void OnEnemySpawn(NPC npc)
		{
			var increase = (int)(npc.lifeMax * 0.1f);
			npc.lifeMax += increase;
			npc.life += increase;
		}
	}

	internal class DefensePassive : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EnemyDefense;
			difficulty = 2;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.defense += 1;
			npc.defDefense += 1;
		}
	}
}
