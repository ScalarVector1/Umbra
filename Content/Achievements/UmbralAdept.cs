using System.Collections.Generic;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;

namespace Umbra.Content.Achievements
{
	internal class UmbralAdept : ModAchievement
	{
		public static CustomIntCondition condition;

		public override string TextureName => "Umbra/Assets/Achievements/UmbralAdept";

		public override void SetStaticDefaults()
		{
			Achievement.SetCategory(AchievementCategory.Challenger);
			condition = AddIntCondition(100);
		}

		public override IEnumerable<Position> GetModdedConstraints()
		{
			yield return new After(ModContent.GetInstance<UmbralAcolyte>());
		}
	}
}
