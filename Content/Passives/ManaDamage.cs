using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
	internal class ManaDamage : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.ManaDamage;
			difficulty = 4;
		}

		public override void BuffPlayer(Player player)
		{
			player.GetModPlayer<ManaStealPlayer>().lostOnHit += 10;
		}
	}

	internal class ManaStealPlayer : ModPlayer
	{
		public int lostOnHit;

		public override void OnHurt(Player.HurtInfo info)
		{
			Player.statMana -= lostOnHit;

			if (Player.statMana < 0)
				Player.statMana = 0;
		}

		public override void ResetEffects()
		{
			lostOnHit = 0;
		}
	}
}
