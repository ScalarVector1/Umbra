using Terraria.Audio;
using Terraria.ID;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Items.Slottables
{
	internal abstract class Slottable : ModItem
	{
		public int difficulty = 0;

		public override void SetDefaults()
		{
			Item.width = 42;
			Item.height = 42;
		}

		public virtual void DrawInSlot(SpriteBatch spriteBatch, Vector2 center, float scale, int slotID)
		{

		}

		public virtual void OnSocket()
		{

		}

		public virtual void OnDesocket()
		{

		}

		public virtual void BuffPlayer(Player player)
		{

		}

		public virtual void OnEnemySpawn(NPC npc)
		{

		}

		public virtual void UpdateSocketed(Passive slot)
		{

		}
	}
}
