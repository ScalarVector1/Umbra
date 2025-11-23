using System.ComponentModel;
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

		[DefaultValue(typeof(bool), "false")]
		public bool ShowCustomMode;
	}
}
