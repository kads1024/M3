using System.Collections.Generic;
using System.Linq;

namespace M3.Core.Domain.Match
{
    public sealed class MatchClassifier
    {
        public IReadOnlyList<ClassifiedMatch> Classify(
            IReadOnlyList<MatchGroup> lineMatches)
        {
            if (lineMatches == null || lineMatches.Count == 0)
                return new List<ClassifiedMatch>();

            var results = new List<ClassifiedMatch>();

            // 1. Group by color first (different colors can never merge)
            var matchesByColor = lineMatches.GroupBy(m => m.Color);

            foreach (var colorGroup in matchesByColor)
            {
                var clusters = BuildClusters(colorGroup.ToList());

                foreach (var cluster in clusters)
                {
                    var pattern = DeterminePattern(cluster);

                    results.Add(new ClassifiedMatch(
                        colorGroup.Key,
                        pattern,
                        cluster.ToList()
                    ));
                }
            }

            return results;
        }

        // ----------------------------
        // Cluster building
        // ----------------------------

        private static List<HashSet<Position>> BuildClusters(
            List<MatchGroup> matches)
        {
            var clusters = new List<HashSet<Position>>();

            foreach (var match in matches)
            {
                var matchPositions = new HashSet<Position>(match.Positions);

                HashSet<Position> mergedCluster = null;

                foreach (var cluster in clusters)
                {
                    if (cluster.Overlaps(matchPositions))
                    {
                        cluster.UnionWith(matchPositions);
                        mergedCluster = cluster;
                        break;
                    }
                }

                if (mergedCluster == null)
                {
                    clusters.Add(matchPositions);
                }
                else
                {
                    // Handle cascading merges (cluster overlaps another cluster)
                    MergeOverlappingClusters(clusters);
                }
            }

            return clusters;
        }

        private static void MergeOverlappingClusters(
            List<HashSet<Position>> clusters)
        {
            for (int i = 0; i < clusters.Count; i++)
            {
                for (int j = i + 1; j < clusters.Count; j++)
                {
                    if (clusters[i].Overlaps(clusters[j]))
                    {
                        clusters[i].UnionWith(clusters[j]);
                        clusters.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        // ----------------------------
        // Pattern detection
        // ----------------------------

        private static MatchPattern DeterminePattern(
            HashSet<Position> positions)
        {
            var byX = positions.GroupBy(p => p.X).ToList();
            var byY = positions.GroupBy(p => p.Y).ToList();

            var verticalLines = byX.Where(g => g.Count() >= 3).ToList();
            var horizontalLines = byY.Where(g => g.Count() >= 3).ToList();

            if (verticalLines.Count > 0 && horizontalLines.Count > 0)
            {
                // Find intersection point
                foreach (var vLine in verticalLines)
                {
                    foreach (var hLine in horizontalLines)
                    {
                        var intersection = vLine
                            .Select(v => v)
                            .FirstOrDefault(v =>
                                hLine.Any(h => h.X == v.X && h.Y == v.Y));

                        if (intersection.X != 0 || intersection.Y != 0)
                        {
                            bool isVerticalEndpoint =
                                IsEndpoint(intersection, vLine);

                            bool isHorizontalEndpoint =
                                IsEndpoint(intersection, hLine);

                            // Endpoint of both → L-shape
                            if (isVerticalEndpoint && isHorizontalEndpoint)
                                return MatchPattern.LShape;

                            // Interior of at least one → T-shape
                            return MatchPattern.TShape;
                        }
                    }
                }
            }

            return MatchPattern.Line;
        }

        private static bool IsEndpoint(
            Position intersection,
            IEnumerable<Position> line)
        {
            var ordered = line
                .OrderBy(p => p.X)
                .ThenBy(p => p.Y)
                .ToList();

            return intersection.Equals(ordered.First()) ||
                   intersection.Equals(ordered.Last());
        }
    }
}
