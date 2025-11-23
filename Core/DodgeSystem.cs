using System.Collections.Generic;
using Terraria.ID;

namespace Umbra.Core
{
	internal class DodgeNPC : GlobalNPC
	{
		public float flatDodge;
		public float increasedDodge;
		public List<float> moreDodge = [];
		public float dodgeRoll;

		public static event Action<NPC, NPC.HitInfo> OnDodge;

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
		{
			return !entity.friendly && entity.lifeMax > 5 && !NPCID.Sets.CountsAsCritter[entity.type] && entity.damage > 0;
		}

		public override void SetDefaults(NPC entity)
		{
			dodgeRoll = Main.rand.NextFloat();
		}

		public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
		{
			float dodge = flatDodge;
			dodge += dodge * increasedDodge;
			foreach (float more in moreDodge)
			{
				dodge += dodge * more;
			}

			if (dodgeRoll <= dodge)
			{
				modifiers.FinalDamage *= 0;
				modifiers._combatTextHidden = true;
			}
		}

		public override void HitEffect(NPC npc, NPC.HitInfo hit)
		{
			float dodge = flatDodge;
			dodge += dodge * increasedDodge;
			foreach (float more in moreDodge)
			{
				dodge += dodge * more;
			}

			if (dodgeRoll <= dodge)
			{
				CombatText.NewText(npc.Hitbox, Color.PaleGreen, "Dodge!");
				OnDodge?.Invoke(npc, hit);
				npc.life += 1;
			}

			dodgeRoll = Main.rand.NextFloat();
		}
	}
}
