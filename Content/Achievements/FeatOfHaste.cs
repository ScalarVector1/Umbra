using System.Collections.Generic;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Umbra.Content.Passives.Large;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Achievements
{
	internal class FeatOfHaste : ModAchievement
	{
		public static CustomFlagCondition condition;

		public override string TextureName => "Umbra/Assets/Achievements/FeatOfHaste";

		public override void SetStaticDefaults()
		{
			Achievement.SetCategory(AchievementCategory.Challenger);
			condition = AddCondition();

			AchievementsHelper.OnNPCKilled += listener;
		}

		private void listener(Player player, short npcId)
		{
			if (player.whoAmI != Main.myPlayer)
				return;

			if (npcId == NPCID.MoonLordCore && TreeSystem.tree.AnyActive<TemporalTrance>())
			{
				condition.Complete();
			}
		}

		public override IEnumerable<Position> GetModdedConstraints()
		{
			yield return new After(ModContent.GetInstance<UmbralMaster>());
		}
	}
}
