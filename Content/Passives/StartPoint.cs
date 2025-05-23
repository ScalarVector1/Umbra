﻿using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives
{
	internal class StartPoint : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.StartPassive;
			difficulty = 0;
			size = 2;
		}

		public override bool CanDeallocate(Player player)
		{
			return false;
		}
	}
}
