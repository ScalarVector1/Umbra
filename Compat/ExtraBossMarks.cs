using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Umbra.Compat
{
	/// <summary>
	/// Provides additional crossmod data for which entities should count as bosses
	/// for the sake of being effected by the tree
	/// </summary>
	internal class ExtraBossMarks : ModSystem
	{
		public static List<int> countsAsBoss = new();
		public static List<string> modsNotPresent = new();

		public override void PostSetupContent()
		{
			countsAsBoss.Clear();
			modsNotPresent.Clear();

			// Add a handful of vanilla entities that should count as bosses
			countsAsBoss.Add(NPCID.Creeper);

			countsAsBoss.Add(NPCID.EaterofWorldsBody);
			countsAsBoss.Add(NPCID.EaterofWorldsTail);

			countsAsBoss.Add(NPCID.SkeletronHand);

			countsAsBoss.Add(NPCID.WallofFleshEye);
			countsAsBoss.Add(NPCID.TheHungry);
			countsAsBoss.Add(NPCID.TheHungryII);

			countsAsBoss.Add(NPCID.TheDestroyerBody);
			countsAsBoss.Add(NPCID.TheDestroyerTail);

			countsAsBoss.Add(NPCID.PrimeCannon);
			countsAsBoss.Add(NPCID.PrimeLaser);
			countsAsBoss.Add(NPCID.PrimeSaw);
			countsAsBoss.Add(NPCID.PrimeVice);

			countsAsBoss.Add(NPCID.PlanterasHook);
			countsAsBoss.Add(NPCID.PlanterasTentacle);

			countsAsBoss.Add(NPCID.MartianSaucerCannon);
			countsAsBoss.Add(NPCID.MartianSaucerCore);
			countsAsBoss.Add(NPCID.MartianSaucerTurret);

			countsAsBoss.Add(NPCID.PumpkingBlade);

			countsAsBoss.Add(NPCID.PirateShipCannon);

			countsAsBoss.Add(NPCID.GolemFistLeft);
			countsAsBoss.Add(NPCID.GolemFistRight);
			countsAsBoss.Add(NPCID.GolemHead);
			countsAsBoss.Add(NPCID.GolemHeadFree);

			countsAsBoss.Add(NPCID.MoonLordFreeEye);
			countsAsBoss.Add(NPCID.MoonLordHand);
			countsAsBoss.Add(NPCID.MoonLordHead);
			countsAsBoss.Add(NPCID.MoonLordLeechBlob);

			// Modded entries, largely up to community maintinence. Please PR additions or removals!

			// Starlight River
			TryAddBoss("StarlightRiver", "Tentacle"); // Example entry 

			TryAddBoss("StarlightRiver", "VitricBossCrystal");
			TryAddBoss("StarlightRiver", "ArenaBottom");

			TryAddBoss("StarlightRiver", "DeadBrain");
			TryAddBoss("StarlightRiver", "TheThinker");
			TryAddBoss("StarlightRiver", "Neurysm");
			TryAddBoss("StarlightRiver", "HorrifyingVisage");
			TryAddBoss("StarlightRiver", "WeakPoint");
		}

		/// <summary>
		/// Attempts to add the given NPC from the given mod to the boss registry. If the mod is not present
		/// it will be marked as such and other NPCs from that mod not attempted
		/// </summary>
		/// <param name="mod"></param>
		/// <param name="internalName"></param>
		public void TryAddBoss(string modName, string internalName)
		{
			if (modsNotPresent.Contains(modName))
				return;

			if (ModLoader.TryGetMod(modName, out Mod mod))
			{
				if (mod.TryFind<ModNPC>(internalName, out ModNPC npc))
					countsAsBoss.Add(npc.Type);
			}
			else
			{
				modsNotPresent.Add(modName);
			}
		}

		/// <summary>
		/// Should be used to check if a given NPC is a boss for all tree functionality
		/// </summary>
		/// <param name="npc">The NPC to check</param>
		/// <returns>If the NPC should be treated as a boss</returns>
		public static bool DoICountAsABoss(NPC npc)
		{
			return npc.boss || npc.GetGlobalNPC<ExtraBossMarkNPC>().countsAsBossForTree;
		}
	}

	internal class ExtraBossMarkNPC : GlobalNPC
	{
		public bool countsAsBossForTree;

		public override bool InstancePerEntity => true;

		public override void SetDefaults(NPC entity)
		{
			if (ExtraBossMarks.countsAsBoss.Contains(entity.type))
				countsAsBossForTree = true;
		}
	}
}
