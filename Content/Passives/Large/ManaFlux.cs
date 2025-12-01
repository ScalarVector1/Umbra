using Terraria.ID;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class ManaFlux : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.ManaFlux;
			difficulty = 40;
			size = 1;
		}

		public override void Update()
		{
			FlatManaCostSystem.flatCostToAdd += 4;
		}
	}

	internal class ManaFluxPlayer : ModPlayer
	{
		public static bool Active => TreeSystem.tree.AnyActive<ManaFlux>();

		public override void PostUpdate()
		{
			if (Active && Player.HasBuff(BuffID.Bleeding) && Player.statMana > 0)
			{
				Player.CheckMana(1, true, true);
			}
		}
	}
}
