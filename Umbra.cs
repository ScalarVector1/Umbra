global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using System;
global using Terraria;
global using Terraria.ModLoader;
using System.IO;
using Terraria.ID;
using Umbra.Content.GUI;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra
{
	public class Umbra : Mod
	{
		public static Umbra Instance { get; set; }

		public Umbra()
		{
			Instance = this;
		}

		public override void Load()
		{
#if DEBUG
			// This has to be here to generate localization, boo womp
			foreach (Type type in Umbra.Instance.Code.GetTypes())
			{
				if (!type.IsAbstract && type.IsSubclassOf(typeof(Passive)))
				{
					Passive instance = Activator.CreateInstance(type) as Passive;
					Logger.Info($"{instance.Name}: {instance.Tooltip}");
				}
			}
#endif
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			string type = reader.ReadString();

			if (type == "RequestTreeOnJoin")
			{
				if (Main.netMode == NetmodeID.Server)
					UmbraNet.SyncTree(whoAmI);
			}
			else if (type == "SyncTree")
			{
				TreeSystem.tree.Deserialize(reader);

				if (Main.netMode == NetmodeID.Server)
					UmbraNet.SyncTree(-1, whoAmI);
			}
		}
	}
}
