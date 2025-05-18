using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Passives.Large
{
	internal class TwistOfFate : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.TwistOfFate;
			difficulty = 0;
			size = 1;
		}
	}
}
