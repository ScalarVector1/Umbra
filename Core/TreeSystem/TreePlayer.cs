using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace Umbra.Core.TreeSystem
{
	internal class TreePlayer : ModPlayer
	{
		public int UmbraPoints;
		public bool firstPoint;

		public int flatRegen;
		public float increasedRegen;

		public override void ResetEffects()
		{
			flatRegen = 0;
			increasedRegen = 0;
		}

		public override void UpdateEquips()
		{
			foreach (Passive passive in ModContent.GetInstance<TreeSystem>().tree.Nodes)
			{
				if (passive.active)
					passive.BuffPlayer(Player);
			}
		}

		public override void UpdateLifeRegen()
		{
			Player.lifeRegen += flatRegen;
			Player.lifeRegen += (int)(Player.lifeRegen * increasedRegen);
		}

		public override void SaveData(TagCompound tag)
		{
			tag["points"] = UmbraPoints;
			tag["firstPoint"] = firstPoint;
		}

		public override void LoadData(TagCompound tag)
		{
			UmbraPoints = tag.GetInt("points");
			firstPoint = tag.GetBool("firsPoint");
		}
	}
}
