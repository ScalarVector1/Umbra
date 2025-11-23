using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Umbra.Content.Configs
{
	internal class CustomTreeConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[DefaultValue(typeof(string), "")]
		public string customTreeJson;
	}
}
