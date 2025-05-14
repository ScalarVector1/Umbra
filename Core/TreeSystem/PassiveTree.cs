using System.Collections.Generic;
using System.Linq;

namespace Umbra.Core.TreeSystem
{
	internal class PassiveEdge
	{
		public int Start { get; set; }
		public int End { get; set; }

		public PassiveEdge(int start, int end)
		{
			Start = start;
			End = end;
		}
	}

	internal class PassiveTree
	{
		public Dictionary<int, Passive> nodesById;
		public Dictionary<(int, int), Passive> nodesByLocation;

		public int difficulty;

		public List<Passive> Nodes { get; set; }
		public List<PassiveEdge> Edges { get; set; }

		/// <summary>
		/// Adds a connection between two node IDs
		/// </summary>
		/// <param name="start">The start nodes ID</param>
		/// <param name="end">The end nodes ID</param>
		public void Connect(int start, int end)
		{
			Edges.Add(new(start, end));
		}

		/// <summary>
		/// Inserts a new passive, returns its ID
		/// </summary>
		/// <param name="toAdd"></param>
		public int Insert(Passive toAdd, int x, int y)
		{
			int nextID = 0;

			while (Nodes.Any(n => n.ID == nextID))
			{
				nextID++;
			}

			Passive newPassive = toAdd.Clone();
			newPassive.ID = nextID;
			newPassive.X = x;
			newPassive.Y = y;

			Nodes.Add(newPassive);
			GenerateDicts();

			return nextID;
		}

		/// <summary>
		/// Attempts to remove the node with the given ID and all connections to and from it.
		/// </summary>
		/// <param name="id"></param>
		public void Remove(int id)
		{
			if (nodesById.ContainsKey(id))
			{
				Nodes.Remove(nodesById[id]);
				Edges.RemoveAll(n => n.Start == id || n.End == id);

				GenerateDicts();
			}
		}

		/// <summary>
		/// Generates a lookup dictionary of nodes by ID after loading the tree data
		/// </summary>
		public void GenerateDicts()
		{
			nodesById = [];
			nodesByLocation = [];

			foreach (Passive node in Nodes)
			{
				nodesById[node.ID] = node;
				nodesByLocation[(node.X, node.Y)] = node;
			}
		}

		/// <summary>
		/// Recalculates the total tree difficulty
		/// </summary>
		public void CalcDifficulty()
		{
			difficulty = 0;

            foreach (var node in Nodes)
            {
				if (node.active)
					difficulty += node.difficulty;
            }
        }

		/// <summary>
		/// Returns a list of integers representing the IDs of active nodes, used to serialize
		/// the tree state.
		/// </summary>
		/// <returns></returns>
		public List<int> GetActiveIDs()
		{
			var active = new List<int>();

			foreach (Passive node in Nodes)
			{
				if (node.active)
					active.Add(node.ID);
			}

			return active;
		}

		/// <summary>
		/// Applies a list of IDs to be activated, sued to deserialize the tree state. Should only
		/// be called after GenerateDicts has run.
		/// </summary>
		/// <param name="toActivate"></param>
		public void ApplyActiveIDs(List<int> toActivate)
		{
			if (nodesById is null)
				throw new Exception("ApplyActiveIDs was called before GenerateDicts. Please make sure not to run this before GenerateDicts is called!");

			foreach (int id in toActivate)
			{
				if (nodesById.TryGetValue(id, out Passive node))
					node.active = true;
			}

			CalcDifficulty();
		}
	}
}
