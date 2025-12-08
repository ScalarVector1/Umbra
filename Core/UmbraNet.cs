using Terraria.DataStructures;
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
			TreeSystem.tree.SerializeState(packet);
			packet.Send(toClient, ignoreClient);
		}

		public static void SyncPotentiallyNewCustomTree(int toClient = -1, int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			ModPacket packet = Umbra.Instance.GetPacket();
			packet.Write("SyncCustomTree");

			bool hasCustom = TreeSystem.hasCustomTree;
			packet.Write(hasCustom);

			if (hasCustom)
			{
				TreeSystem.tree.SerializeLayout(packet);
			}

			TreeSystem.tree.SerializeState(packet);
			packet.Send(toClient, ignoreClient);
		}

		public static void SyncPoints(int whosPoints, int toClient = -1, int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			ModPacket packet = Umbra.Instance.GetPacket();
			packet.Write("PointsSync");

			packet.Write(whosPoints);

			TreePlayer tp = Main.player[whosPoints].GetModPlayer<TreePlayer>();
			packet.Write(tp.UmbraPoints);
			packet.Write(tp.partialPoints);
			packet.Write(tp.nextPoint);

			packet.Send(toClient, ignoreClient);
		}

		public static void SyncTileEntity(int entityId, int toClient = -1, int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			ModPacket packet = Umbra.Instance.GetPacket();
			packet.Write("TESync");

			packet.Write(entityId);
			TileEntity.ByID[entityId].NetSend(packet);

			packet.Send(toClient, ignoreClient);
		}
	}
}
