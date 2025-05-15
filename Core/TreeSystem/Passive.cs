using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Terraria.Localization;
using Umbra.Content.Passives;

namespace Umbra.Core.TreeSystem
{
	public abstract class Passive
	{
		public bool active;

		public string nameKey;
		public string tooltipKey;

		public int difficulty;
		public Asset<Texture2D> texture;
		public int size;

		public float opacity = 1;

		public List<Passive> connections = [];

		public int ID { get; set; }

		public int Cost { get; set; }

		public int X { get; set; }
		public int Y { get; set; }

		[JsonIgnore]
		public string Name => Language.GetOrRegister(nameKey).Value;
		[JsonIgnore]
		public string Tooltip => Language.GetOrRegister(tooltipKey).Value;

		[JsonIgnore]
		public Vector2 TreePos => new(X * 16, Y * 16);
		[JsonIgnore]
		public int Width => size == 0 ? 38 : size == 1 ? 50 : size == 2 ? 58 : 38;
		[JsonIgnore]
		public int Height => size == 0 ? 38 : size == 1 ? 50 : size == 2 ? 58 : 38;

		public Passive()
		{
			nameKey = $"Mods.Umbra.Passives.{GetType().Name}.Name";
			tooltipKey = $"Mods.Umbra.Passives.{GetType().Name}.Tooltip";

			SetDefaults();
		}

		public virtual void SetDefaults() 
		{
			difficulty = 1;
			texture = Assets.GUI.PassiveFrameTiny;
		}

		/// <summary>
		/// If this passive can ever be active given the current game state.
		/// </summary>
		/// <returns></returns>
		public virtual bool CanBeActive() 
		{
			return true;
		}

		public virtual void BuffPlayer(Player player) { }

		public virtual void OnEnemySpawn(NPC npc) { }

		public virtual void Update() { }

		public void Draw(SpriteBatch spriteBatch, Vector2 center)
		{
			Texture2D tex = texture?.Value ?? Assets.GUI.PassiveFrameTiny.Value;

			Color color = Color.DimGray;

			if (CanAllocate(Main.LocalPlayer))
				color = Color.Lerp(Color.Gray, Color.LightGray, (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.5f + 0.5f);

			if (active)
			{
				color = Color.White;
				spriteBatch.Draw(Assets.GUI.GlowAlpha.Value, center, null, new Color(180, 120, 255, 0) * opacity, 0, Assets.GUI.GlowAlpha.Size() / 2f, 0.5f + size * 0.1f, 0, 0);
			}

			spriteBatch.Draw(tex, center, null, color * opacity, 0, tex.Size() / 2f, 1, 0, 0);
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
				connections.Any(n => n.active);
				
				// For directed version
				//(tree.Edges.Any(n => tree.nodesById[n.Start].active && n.End == ID) || !tree.Edges.Any(n => n.End == ID));
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
			ModContent.GetInstance<TreeSystem>().tree.RegenerateFlows();
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
				!connections.Any(n => n.active && !n.HasPathToStartWithout(this));
				// For directed version
				//!tree.Edges.Any(n => tree.nodesById[n.End].active && n.Start == ID && !tree.Edges.Any(a => a != n && a.End == n.End && tree.nodesById[a.Start].active));
		}

		public bool HasPathToStartWithout(Passive excluded)
		{
			HashSet<Passive> visited = new();
			return HasPathToStartWithoutInternal(this, excluded, visited);
		}

		private bool HasPathToStartWithoutInternal(Passive current, Passive excluded, HashSet<Passive> visited)
		{
			if (current == null || current == excluded || !current.active || visited.Contains(current))
				return false;

			if (current is StartPoint)
				return true;

			visited.Add(current);

			foreach (var connection in current.connections)
			{
				if (HasPathToStartWithoutInternal(connection, excluded, visited))
					return true;
			}

			return false;
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
			ModContent.GetInstance<TreeSystem>().tree.RegenerateFlows();
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
			var clone = MemberwiseClone() as Passive;
			clone.connections = new();
			return clone;
		}
	}
}
