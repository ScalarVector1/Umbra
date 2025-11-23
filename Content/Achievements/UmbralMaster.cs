using System.Collections.Generic;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;

namespace Umbra.Content.Achievements
{
	internal class UmbralMaster : ModAchievement
	{
		public static CustomIntCondition condition;

		public override string TextureName => "Umbra/Assets/Achievements/UmbralMaster";

		public override void SetStaticDefaults()
		{
			Achievement.SetCategory(AchievementCategory.Challenger);
			condition = AddIntCondition(1000);
		}

		public override IEnumerable<Position> GetModdedConstraints()
		{
			yield return new After(ModContent.GetInstance<UmbralAdept>());
		}
	}
}
