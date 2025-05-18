using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace Umbra.Core.PassiveTreeSystem
{
	internal class TreePlayer : ModPlayer
	{
		public int UmbraPoints;
		public bool firstPoint;

		public override void UpdateEquips()
		{
			foreach (Passive passive in TreeSystem.tree.activeNodes)
			{
				passive.BuffPlayer(Player);
			}
		}

		public override void SaveData(TagCompound tag)
		{
			tag["points"] = UmbraPoints;
			tag["firstPoint"] = firstPoint;
		}

		public override void LoadData(TagCompound tag)
		{
			UmbraPoints = tag.GetInt("points");
			firstPoint = tag.GetBool("firstPoint");
		}
	}
}
