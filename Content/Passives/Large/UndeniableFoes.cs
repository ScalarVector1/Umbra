using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class UndeniableFoes : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.UndeniableFoes;
			difficulty = 20;
			size = 1;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			npc.knockBackResist = 0f;
		}
	}
}
