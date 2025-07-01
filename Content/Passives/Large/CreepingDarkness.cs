using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class CreepingDarkness : Passive
	{
		public static int darkenAmount;

		public override void SetDefaults()
		{
			texture = Assets.Passives.CreepingDarkness;
			difficulty = 6;
			size = 1;
		}

		public override void Update()
		{
			LightMultiplier.lightMultiplier -= darkenAmount / 300f * 0.1f;

			var dark = Lighting.Brightness((int)Main.LocalPlayer.Center.X / 16, (int)Main.LocalPlayer.Center.Y / 16) < 0.1f;

			if (dark && darkenAmount < 300)
				darkenAmount++;

			if (!dark && darkenAmount > 0)
				darkenAmount--;
		}
	}
}
