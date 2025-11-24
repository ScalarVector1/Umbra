using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Umbra.Content.Items.Slottables;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
	internal class Slot : Passive
	{
		public override bool AllowDuplicates => true;

		public Slottable SlottedItem => TreeSystem.tree.storedItems.ContainsKey(ID) ? TreeSystem.tree.storedItems[ID].ModItem as Slottable : null;

		public override string DisplayName => SlottedItem?.Item?.Name ?? base.Name;

		public override string Tooltip => SlottedItem?.Item?.ToolTip?._text?.Value ?? "";

		public override void SetDefaults()
		{
			texture = Assets.Passives.Slot;
			difficulty = 0;
			size = 1;
		}

		public override void Draw(SpriteBatch spriteBatch, Vector2 center, float scale)
		{
			base.Draw(spriteBatch, center, scale);
			SlottedItem?.DrawInSlot(spriteBatch, center, scale, ID);
		}

		public override void OnClick()
		{
			if (Main.mouseItem != null && !Main.mouseItem.IsAir && Main.mouseItem.ModItem is Slottable && SlottedItem is null)
			{
				TreeSystem.tree.storedItems[ID] = Main.mouseItem.Clone();
				Main.mouseItem.TurnToAir();

				SlottedItem?.OnSocket();
			}
			else if ((Main.mouseItem is null || Main.mouseItem.IsAir) && SlottedItem is not null)
			{
				SlottedItem?.OnDesocket();

				Main.mouseItem = SlottedItem.Item.Clone();
				TreeSystem.tree.storedItems.Remove(ID);
			}

			TreeSystem.tree.CalcDifficultyAndTooltips();
		}

		public override bool CanDeallocate(Player player)
		{
			return base.CanDeallocate(player) && SlottedItem is null;
		}

		public override void BuffPlayer(Player player)
		{
			SlottedItem?.BuffPlayer(player);
		}

		public override void OnEnemySpawn(NPC npc)
		{
			SlottedItem?.OnEnemySpawn(npc);
		}

		public override void Update()
		{
			SlottedItem?.UpdateSocketed(this);
		}
	}
}
