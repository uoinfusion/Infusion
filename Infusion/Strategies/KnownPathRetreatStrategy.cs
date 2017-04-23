using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.Packets;

namespace Infusion.Strategies
{
    public class KnownPathRetreatStrategy
    {
        private readonly ushort safeDistance;
        private readonly Stack<Direction> approachPath;
        private readonly Stack<Direction> reaproachPath = new Stack<Direction>();
        private Direction? lastRetreatDirection;

        public KnownPathRetreatStrategy(ushort safeDistance) : this(safeDistance, Array.Empty<Direction>())
        {
        }

        public KnownPathRetreatStrategy(ushort safeDistance, IEnumerable<Direction> approachPath)
        {
            this.safeDistance = safeDistance;
            this.approachPath = new Stack<Direction>(approachPath);
        }

        public Direction? NextDirection(Location3D currentLocation, Location3D targetLocation)
        {
            targetLocation = targetLocation.WithZ(0);
            currentLocation = currentLocation.WithZ(0);
            var targetVector = targetLocation - currentLocation;

            if (targetVector.Length < safeDistance)
            {
                if (approachPath.Any())
                {
                    var approachDirection = approachPath.Pop();
                    reaproachPath.Push(approachDirection);
                    lastRetreatDirection = approachDirection.Opposite();
                    return lastRetreatDirection.Value;
                }

                if (lastRetreatDirection.HasValue)
                    return lastRetreatDirection.Value;
                throw new InvalidOperationException("Cannot retreat, no known direction.");
            }

            return null;
        }

        public Direction[] GetReaproachPath() => reaproachPath.ToArray();
    }
}
