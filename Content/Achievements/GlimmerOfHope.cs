using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Umbra.Content.Passives.Large;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Achievements
{
	internal class GlimmerOfHope : ModAchievement
	{
		public static CustomFlagCondition condition;

		public override string TextureName => "Umbra/Assets/Achievements/GlimmerOfHope";

		public override void SetStaticDefaults()
		{
			Achievement.SetCategory(AchievementCategory.Challenger);
			condition = AddCondition();
		}

		public override IEnumerable<Position> GetModdedConstraints()
		{
			yield return new After(ModContent.GetInstance<UmbralMaster>());
		}
	}
}
