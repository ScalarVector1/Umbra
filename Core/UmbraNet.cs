using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Core
{
	internal static class UmbraNet
	{
		public static void RequestTreeOnJoin(int toClient = -1, int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			ModPacket packet = Umbra.Instance.GetPacket();
			packet.Write("RequestTreeOnJoin");
			packet.Send(toClient, ignoreClient);
		}

		public static void SyncTree(int toClient = -1, int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			ModPacket packet = Umbra.Instance.GetPacket();
			packet.Write("SyncTree");
			TreeSystem.tree.Serialize(packet);
			packet.Send(toClient, ignoreClient);
		}
	}
}
