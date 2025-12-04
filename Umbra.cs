global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using System;
global using Terraria;
global using Terraria.ModLoader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra
{
	public class Umbra : Mod
	{
		private List<IOrderedLoadable> loadCache;

		public static Umbra Instance { get; set; }

		public Umbra()
		{
			Instance = this;
		}

		public override void Load()
		{
			loadCache = [];

			foreach (Type type in Code.GetTypes())
			{
				if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IOrderedLoadable)))
				{
					object instance = Activator.CreateInstance(type);
					loadCache.Add(instance as IOrderedLoadable);
				}

				loadCache.Sort((n, t) => n.Priority.CompareTo(t.Priority));
			}

			for (int k = 0; k < loadCache.Count; k++)
			{
				loadCache[k].Load();
				Terraria.ModLoader.UI.Interface.loadMods.SubProgressText = "Loading " + loadCache[k].GetType().Name;
			}
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			string type = reader.ReadString();

#if DEBUG
			Logger.Info($"Recieved packet: {type} from: {whoAmI}");

			if (Main.netMode == NetmodeID.MultiplayerClient)
				Main.NewText($"CLIENT: Recieved packet: {type} from: {whoAmI}", Color.Wheat);

			if (Main.netMode == NetmodeID.Server)
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"SERVER: Recieved packet: {type} from: {whoAmI}"), Color.Lavender);
#endif

			if (type == "RequestTreeOnJoin")
			{
				if (Main.netMode == NetmodeID.Server)
					UmbraNet.SyncPotentiallyNewCustomTree(whoAmI);
			}
			else if (type == "SyncTree")
			{
				TreeSystem.tree.Deserialize(reader);

				if (Main.netMode == NetmodeID.Server)
					UmbraNet.SyncTree(-1, whoAmI);
			}
			else if (type == "SyncCustomTree")
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					bool hasCustom = reader.ReadBoolean();

					if (hasCustom)
					{
						string customTree = reader.ReadString();
						TreeSystem.LoadFromString(customTree);
					}

					TreeSystem.tree.Deserialize(reader);
				}
			}
			else if (type == "PointsSync")
			{
				int whosPoints = reader.ReadInt32();
				TreePlayer tp = Main.player[whosPoints].GetModPlayer<TreePlayer>();

				tp.UmbraPoints = reader.ReadInt32();
				tp.partialPoints = reader.ReadInt32();
				tp.nextPoint = reader.ReadInt32();

				if (Main.netMode == NetmodeID.Server)
					UmbraNet.SyncPoints(whosPoints, -1, whoAmI);
			}
			else if (type == "TESync")
			{
				int id = reader.ReadInt32();

				if (TileEntity.ByID.ContainsKey(id))
				{
					TileEntity.ByID[id].NetReceive(reader);

					if (Main.netMode == NetmodeID.Server)
						UmbraNet.SyncTileEntity(id, -1, whoAmI);
				}
				else
				{
					Logger.Error($"Could not find TE: {id} on server!");
				}
			}
		}
	}
}
