using Terraria.DataStructures;
using Umbra.Content.Passives.Large;

namespace Umbra.Core.PassiveTreeSystem
{
	internal class DoomEffectsSystem : ModSystem
	{
		public static bool Inverted => TreeSystem.tree.AnyActive<TwistOfFate>();

		public static int Doom => TreeSystem.tree.difficulty;

		public static float DoomValueMult => 1.7f * MathF.Log(0.01f * Doom + 1);
		public static float DoubleLootChance => 0.1f * MathF.Atan(Doom * 0.001f);
		public static float LuckBonus => 0.5f * MathF.Log(0.01f * Doom + 1);
	}

	internal class DoomEffectsNPC : GlobalNPC
	{
		public override void OnSpawn(NPC npc, IEntitySource source)
		{
			if (DoomEffectsSystem.Inverted)
			{
				npc.value = (int)(npc.value * (1f / (1 + DoomEffectsSystem.DoomValueMult)));
			}
			else
			{
				npc.value += npc.value * DoomEffectsSystem.DoomValueMult;
			}
		}

		public override void OnKill(NPC npc)
		{
			if (!DoomEffectsSystem.Inverted && Main.rand.NextFloat() <= DoomEffectsSystem.DoubleLootChance)
			{
				npc.NPCLoot();
			}
		}
	}

	internal class DoomEffectsPlayer : ModPlayer
	{
		public override void ModifyLuck(ref float luck)
		{
			if (!DoomEffectsSystem.Inverted)
				luck += DoomEffectsSystem.LuckBonus;
			else
				luck -= DoomEffectsSystem.LuckBonus;
		}
	}
}
