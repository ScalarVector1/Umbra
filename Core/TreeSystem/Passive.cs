using ReLogic.Content;
using System.Linq;
using System.Text.Json.Serialization;
using Terraria.Localization;

namespace Umbra.Core.TreeSystem
{
	internal abstract class Passive
	{
		public bool active;

		public int difficulty;
		public Asset<Texture2D> texture;

		public int ID { get; set; }

		public int Cost { get; set; }

		public int X { get; set; }
		public int Y { get; set; }
		public int Size { get; set; }

		public virtual string NameKey => $"Mods.Umbra.Passives.{GetType().Name}.Name";
		public virtual string TooltipKey => $"Mods.Umbra.Passives.{GetType().Name}.Tooltip";

		public string Name => Language.GetOrRegister(NameKey).Value;
		public string Tooltip => Language.GetOrRegister(TooltipKey).Value;

		public Vector2 TreePos => new(X * 16, Y * 16);
		public int Width => Size == 0 ? 38 : Size == 1 ? 50 : Size == 2 ? 58 : 38;
		public int Height => Size == 0 ? 38 : Size == 1 ? 50 : Size == 2 ? 58 : 38;

		public Passive()
		{
			SetDefaults();
		}

		public virtual void SetDefaults() 
		{
			difficulty = 1;
			texture = Assets.GUI.PassiveFrameTiny;
		}

		public virtual void BuffPlayer(Player player) { }

		public virtual void OnEnemySpawn(NPC npc) { }

		public void Draw(SpriteBatch spriteBatch, Vector2 center)
		{
			Texture2D tex = texture?.Value ?? Assets.GUI.PassiveFrameTiny.Value;

			Color color = Color.DimGray;

			if (CanAllocate(Main.LocalPlayer))
				color = Color.Lerp(Color.Gray, Color.LightGray, (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.5f + 0.5f);

			if (active)
			{
				color = Color.White;
				spriteBatch.Draw(Assets.GUI.GlowAlpha.Value, center, null, new Color(180, 120, 255, 0), 0, Assets.GUI.GlowAlpha.Size() / 2f, 0.5f + Size * 0.1f, 0, 0);
			}

			spriteBatch.Draw(tex, center, null, color, 0, tex.Size() / 2f, 1, 0, 0);
		}

		/// <summary>
		/// Called on load to generate the tree edges
		/// </summary>
		/// <param name="all"></param>
		public void Connect(int otherID)
		{
			ModContent.GetInstance<TreeSystem>().tree.Connect(ID, otherID);
		}

		/// <summary>
		/// If this passive is able to be allocated or not
		/// </summary>
		/// <returns></returns>
		public virtual bool CanAllocate(Player player)
		{
			TreeSystem TreeSystem = ModContent.GetInstance<TreeSystem>();
			PassiveTree tree = TreeSystem.tree;

			return
				!active &&
				player.GetModPlayer<TreePlayer>().UmbraPoints >= Cost &&
				(tree.Edges.Any(n => tree.nodesById[n.Start].active && n.End == ID) || !tree.Edges.Any(n => n.End == ID));
		}

		/// <summary>
		/// Allocates this passive and consumes its cost from the given player
		/// </summary>
		/// <param name="player"></param>
		private void Allocate(Player player)
		{
			player.GetModPlayer<TreePlayer>().UmbraPoints -= Cost;
			active = true;

			ModContent.GetInstance<TreeSystem>().tree.CalcDifficulty();
		}

		/// <summary>
		/// Tries to allocate this passive, returns if it was successful or not
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public bool TryAllocate(Player player)
		{
			if (CanAllocate(player))
			{
				Allocate(player);
				return true;
			}

			return false;
		}

		/// <summary>
		/// If this passive can be refunded or not
		/// </summary>
		/// <returns></returns>
		public virtual bool CanDeallocate(Player player)
		{
			TreeSystem TreeSystem = ModContent.GetInstance<TreeSystem>();
			PassiveTree tree = TreeSystem.tree;

			return
				active &&
				!tree.Edges.Any(n => tree.nodesById[n.End].active && n.Start == ID && !tree.Edges.Any(a => a != n && a.End == n.End && tree.nodesById[a.Start].active));
		}

		/// <summary>
		/// Deallocates this passive and refunds half its cost to the given player.
		/// </summary>
		/// <param name="player"></param>
		private void Deallocate(Player player)
		{
			int refundAmount = Cost / 2;
			player.GetModPlayer<TreePlayer>().UmbraPoints += refundAmount;
			active = false;

			ModContent.GetInstance<TreeSystem>().tree.CalcDifficulty();
		}

		/// <summary>
		/// Tries to deallocate this passive, returns if it was successful or not.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public bool TryDeallocate(Player player)
		{
			if (CanDeallocate(player))
			{
				Deallocate(player);
				return true;
			}

			return false;
		}

		public Passive Clone()
		{
			return MemberwiseClone() as Passive;
		}
	}
}
