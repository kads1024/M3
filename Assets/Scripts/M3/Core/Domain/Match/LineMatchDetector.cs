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
        
        private void DetectHorizontal(BoardState board, List<MatchGroup> results)
        {
            for (int y = 0; y < board.Height; y++)
            {
                GemColor? currentColor = null;
                var runPositions = new List<Position>();

                for (int x = 0; x < board.Width; x++)
                {
                    var cell = board.GetCell(x, y);

                    if (cell.IsEmpty)
                    {
                        FlushRunIfValid(runPositions, currentColor, MatchDirection.Horizontal, results);
                        currentColor = null;
                        runPositions.Clear();
                        continue;
                    }

                    var gemColor = cell.Gem.Color;

                    if (currentColor == gemColor)
                    {
                        runPositions.Add(cell.Position);
                    }
                    else
                    {
                        FlushRunIfValid(runPositions, currentColor, MatchDirection.Horizontal, results);
                        currentColor = gemColor;
                        runPositions.Clear();
                        runPositions.Add(cell.Position);
                    }
                }

                // End-of-row flush
                FlushRunIfValid(runPositions, currentColor, MatchDirection.Horizontal, results);
            }
            
        }

        private void DetectVertical(BoardState board, List<MatchGroup> results)
        {
            for (int x = 0; x < board.Width; x++)
            {
                GemColor? currentColor = null;
                var runPositions = new List<Position>();

                for (int y = 0; y < board.Height; y++)
                {
                    var cell = board.GetCell(x, y);

                    if (cell.IsEmpty)
                    {
                        FlushRunIfValid(runPositions, currentColor, MatchDirection.Vertical, results);
                        currentColor = null;
                        runPositions.Clear();
                        continue;
                    }

                    var gemColor = cell.Gem.Color;

                    if (currentColor == gemColor)
                    {
                        runPositions.Add(cell.Position);
                    }
                    else
                    {
                        FlushRunIfValid(runPositions, currentColor, MatchDirection.Vertical, results);
                        currentColor = gemColor;
                        runPositions.Clear();
                        runPositions.Add(cell.Position);
                    }
                }

                // End-of-column flush
                FlushRunIfValid(runPositions, currentColor, MatchDirection.Vertical, results);
            }
        }
        
        private static void FlushRunIfValid(
            List<Position> positions,
            GemColor? color,
            MatchDirection direction,
            List<MatchGroup> results)
        {
            if (color == null)
                return;

            if (positions.Count >= 3)
            {
                results.Add(new MatchGroup(
                    color.Value,
                    direction,
                    new List<Position>(positions)
                ));
            }
        }
    }
}