using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class DoomConflux : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.DoomConflux;
			difficulty = 40;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.GetGlobalNPC<StatusChanceNPC>().AddStatusChance(ModContent.BuffType<Doomed>(), 0.01f * (TreeSystem.tree.difficulty / 10));
		}
	}

	internal class Doomed : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override string Texture => "Umbra/Assets/Buffs/Doomed";
	}

	internal class DoomedPlayer : ModPlayer
	{
		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (Player.HasBuff<Doomed>())
				modifiers.FinalDamage *= 1.5f;
		}
	}
}
