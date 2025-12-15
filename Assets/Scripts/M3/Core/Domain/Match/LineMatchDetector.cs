using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace M3.Core.Domain.Match
{
    public sealed class LineMatchDetector : IMatchDetector
    {
        public IReadOnlyList<MatchGroup> Detect(BoardState board)
        {
            var results = new List<MatchGroup>();

            DetectHorizontal(board, results);
            DetectVertical(board, results);
            
            return results;
        }

        // Implementation comes AFTER tests
        private void DetectHorizontal(BoardState board, List<MatchGroup> results)
        {
            for (int y = 0; y < board.Height; y++)
            {
                GemColor? currentColor = null;
                var positions = new List<Position>();

                for (int x = 0; x < board.Width; x++)
                {
                    var gem = board.GetCell(x, y).Gem;
                      
                    if (gem == null)
                    {
                        CacheIfMatch(positions, currentColor, MatchDirection.Horizontal, results);
                        
                        currentColor = null;
                        positions.Clear();
                        continue;
                    }

                    if (gem.Color == currentColor)
                    {
                        positions.Add(new Position(x, y));
                    }
                    else
                    {
                        CacheIfMatch(positions, currentColor, MatchDirection.Horizontal, results);

                        currentColor = gem.Color;
                        positions = new List<Position> { new Position(x, y) };
                    }
                }

                CacheIfMatch(positions, currentColor, MatchDirection.Horizontal, results);
            }
            
        }

        private void DetectVertical(BoardState board, List<MatchGroup> results)
        {
            for (int x = 0; x < board.Width; x++)
            {
                GemColor? currentColor = null;
                var positions = new List<Position>();

                for (int y = 0; y < board.Height; y++)
                {
                    var gem = board.GetCell(x, y).Gem;
                    
                    if (gem == null)
                    {
                        CacheIfMatch(positions, currentColor, MatchDirection.Vertical, results);
                        
                        currentColor = null;
                        positions.Clear();
                        continue;
                    }

                    if (gem.Color == currentColor)
                    {
                        positions.Add(new Position(x, y));
                    }
                    else
                    {
                        CacheIfMatch(positions, currentColor, MatchDirection.Vertical, results);

                        currentColor = gem.Color;
                        positions = new List<Position> { new Position(x, y) };
                    }
                }

                CacheIfMatch(positions, currentColor, MatchDirection.Vertical, results);
            }
        }
        
        private void CacheIfMatch(
            List<Position> positions,
            GemColor? color,
            MatchDirection direction,
            List<MatchGroup> results)
        {
            if (color.HasValue && positions.Count >= 3)
            {
                results.Add(new MatchGroup(color.Value, direction, new List<Position>(positions)));
            }
        }
    }
}