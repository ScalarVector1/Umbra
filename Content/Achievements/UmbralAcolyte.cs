using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;

namespace Umbra.Content.Achievements
{
	internal class UmbralAcolyte : ModAchievement
	{
		public static CustomFlagCondition condition;

		public override string TextureName => "Umbra/Assets/Achievements/UmbralAcolyte";

		public override void SetStaticDefaults()
		{
			Achievement.SetCategory(AchievementCategory.Challenger);
			condition = AddCondition("AllocateAnyNode");
		}
	}
}
