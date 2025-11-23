using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
	internal class PiercePenalty : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.PiercePenalty;
			difficulty = 6;
		}

		public override void Update()
		{
			PiercePenaltySystem.penalty += 1;
		}
	}

	public class PiercePenaltySystem : ModSystem
	{
		public static int penalty;

		public override void PostUpdateEverything()
		{
			penalty = 0;
		}
	}

	public class PiercePenaltyProjectile : GlobalProjectile
	{
		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (projectile.penetrate > 0)
			{
				projectile.penetrate -= PiercePenaltySystem.penalty;

				if (projectile.penetrate < 0)
					projectile.penetrate = 0;
			}
		}
	}
}
