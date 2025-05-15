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

		public List<PassiveEdge> flows = [];

		public List<Passive> Nodes { get; set; }
		public List<PassiveEdge> Edges { get; set; }

		/// <summary>
		/// Adds a connection between two node IDs
		/// </summary>
		/// <param name="start">The start nodes ID</param>
		/// <param name="end">The end nodes ID</param>
		public void Connect(int start, int end)
		{
			if (!Edges.Any(n => n.Start == start && n.End == end || n.Start == end && n.End == start))
			{
				Edges.Add(new(start, end));
				nodesById[start].connections.Add(nodesById[end]);
				nodesById[end].connections.Add(nodesById[start]);
			}

			RegenerateFlows();
		}

		/// <summary>
		/// Removes a connection between two node IDs
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public void Disconnect(int start, int end)
		{
			Edges.RemoveAll(n => n.Start == start && n.End == end || n.Start == end && n.End == start);

			nodesById[start].connections.Remove(nodesById[end]);
			nodesById[end].connections.Remove(nodesById[start]);

			RegenerateFlows();
		}

		/// <summary>
		/// Called on load to properly inform each node of its neighbors
		/// </summary>
		public void RegenrateConnections()
		{
			foreach (Passive node in Nodes)
			{
				node.connections.Clear();
			}

			foreach (PassiveEdge edge in Edges)
			{
				nodesById[edge.Start].connections.Add(nodesById[edge.End]);
				nodesById[edge.End].connections.Add(nodesById[edge.Start]);
			}
		}

		/// <summary>
		/// Regenerates the visual flows for the tree
		/// </summary>
		public void RegenerateFlows()
		{
			flows.Clear();

			if (!nodesById.TryGetValue(0, out Passive startNode) || !startNode.active)
				return;

			HashSet<int> enqueued = new(); // Track which nodes we've queued
			HashSet<(int, int)> addedEdges = new(); // Track which directed flows we’ve added

			Queue<Passive> queue = new();
			queue.Enqueue(startNode);
			enqueued.Add(startNode.ID);

			while (queue.Count > 0)
			{
				Passive current = queue.Dequeue();

				foreach (var neighbor in current.connections)
				{
					if (!neighbor.active)
						continue;

					int from = current.ID;
					int to = neighbor.ID;

					// Only add one flow per edge, from the direction it was discovered
					if (addedEdges.Contains((to, from)) || addedEdges.Contains((from, to)))
						continue;

					flows.Add(new PassiveEdge(from, to));
					addedEdges.Add((from, to));

					if (!enqueued.Contains(to))
					{
						queue.Enqueue(neighbor);
						enqueued.Add(to);
					}
				}
			}
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
				RegenrateConnections();
				RegenerateFlows();
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

			foreach (Passive node in Nodes)
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

			foreach (Passive node in Nodes)
			{
				node.active = false;
			}

			foreach (int id in toActivate)
			{
				if (nodesById.TryGetValue(id, out Passive node) && node.CanBeActive())
					node.active = true;
			}

			CalcDifficulty();
			RegenerateFlows();
		}
	}
}
