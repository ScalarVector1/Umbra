using Umbra.Compat;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class InstrumentsOfHarm : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.InstrumentsOfHarm;
			difficulty = 30;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			if (!ExtraBossMarks.DoICountAsABoss(npc) && Main.rand.NextBool(5))
			{
				npc.GetGlobalNPC<HarmfulNPC>().active = true;
				npc.GetGlobalNPC<TreeNPC>().moreDamage.Add(0.5f);
			}
		}
	}

	internal class HarmfulNPC : GlobalNPC
	{
		public bool active;

		public override bool InstancePerEntity => true;

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Microsoft.Xna.Framework.Color drawColor)
		{
			if (active)
			{
				Texture2D tex = Assets.Passives.EnemyDamage.Value;
				spriteBatch.Draw(tex, npc.position + new Vector2(npc.width / 2, -16) - Main.screenPosition, null, drawColor * 0.5f, 0, tex.Size() / 2f, 0.5f, 0, 0);
			}
		}
	}
}
