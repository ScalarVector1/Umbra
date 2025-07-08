using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;
using Umbra.Content.GUI.Config;

namespace Umbra.Content.Configs
{
	internal class UIConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[DefaultValue(typeof(Vector2), "574, 274")]
		[CustomModConfigItem(typeof(UmbraRepositionElement))]
		public Vector2 UmbraIconPosition;
	}
}
