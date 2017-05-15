using System;
using System.Collections.Generic;
using System.Linq;
using Infusion.Packets;
using Priority_Queue;

namespace Infusion.Strategies
{
    public class PathFinder
    {
        private readonly IWorldMap map;

        public PathFinder(IWorldMap map)
        {
            this.map = map;
        }

        public Direction[] FindPath(Location2D start, Location2D goal, ushort distance = 0)
        {
            var frontier = new GenericPriorityQueue<QueueNode, int>(65535);
            frontier.Enqueue(new QueueNode(start), 0);

            var cameFrom = new Dictionary<Location2D, Location2D?>();
            var costSofar = new Dictionary<Location2D, int>();

            cameFrom[start] = null;
            costSofar[start] = 0;

            Location2D? current = null;
            var found = false;

            while (frontier.Count > 0)
            {
                current = frontier.Dequeue().Location;

                if (current == goal)
                {
                    found = true;
                    break;
                }

                if (distance > 0)
                {
                    if (current.Value.GetDistance(goal) <= distance)
                    {
                        found = true;
                        break;
                    }
                }

                foreach (var next in GetNeighbors(current.Value))
                {
                    var newCost = costSofar[current.Value] + 1;
                    if (!costSofar.ContainsKey(next) || newCost < costSofar[next])
                    {
                        costSofar[next] = newCost;
                        var priority = newCost + Heuristic(goal, next);
                        frontier.Enqueue(new QueueNode(next), priority);
                        cameFrom[next] = current;
                    }
                }
            }

            var reversePath = new List<Direction>();

            if (!found)
                return null;

            while (current.HasValue)
            {
                var prev = cameFrom[current.Value];

                if (prev.HasValue)
                    reversePath.Add((current.Value - prev.Value).ToDirection());

                current = prev;
            }

            reversePath.Reverse();

            return reversePath.ToArray();
        }

        private int Heuristic(Location2D goal, Location2D next)
        {
            var dx = Math.Abs(next.X - goal.X);
            var dy = Math.Abs(next.Y - goal.Y);

            return dx + dy - Math.Min(dx, dy);
        }

        private Location2D[] GetNeighbors(Location2D current)
        {
            var neighbors = new Tuple<int, int>[8];

            neighbors[0] = new Tuple<int, int>(current.X - 1, current.Y - 1);
            neighbors[1] = new Tuple<int, int>(current.X - 1, current.Y);
            neighbors[2] = new Tuple<int, int>(current.X - 1, current.Y + 1);
            neighbors[3] = new Tuple<int, int>(current.X, current.Y - 1);
            neighbors[4] = new Tuple<int, int>(current.X, current.Y + 1);
            neighbors[5] = new Tuple<int, int>(current.X + 1, current.Y - 1);
            neighbors[6] = new Tuple<int, int>(current.X + 1, current.Y);
            neighbors[7] = new Tuple<int, int>(current.X + 1, current.Y + 1);

            return neighbors.Where(n => n.Item1 >= ushort.MinValue && n.Item2 >= ushort.MinValue &&
                                        n.Item1 <= ushort.MaxValue && n.Item2 <= ushort.MaxValue)
                .Select(n => new Location2D((ushort) n.Item1, (ushort) n.Item2))
                .Where(l => map.IsPassable(l))
                .ToArray();
        }

        private class QueueNode : GenericPriorityQueueNode<int>
        {
            public QueueNode(Location2D location)
            {
                Location = location;
            }

            public Location2D Location { get; }
        }
    }
}