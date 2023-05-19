using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using CrosswordGenerator.Generator.Interfaces;
using CrosswordGenerator.Generator.Models;

namespace CrosswordGenerator.Generator
{
    public class PlacementFinder : IPlacementFinder
    {
        public IEnumerable<Placement> FindWordPlacements(Dictionary<Point, LetterBlock> blocksDict, Word word)
        {
            var placements = new List<Placement>();
            var points = blocksDict.Keys
                .OrderBy(point => point.Y)
                .ThenBy(point => point.X).ToList();

            var boardSize = GetBoardSize(blocksDict);

            for (var i = 0; i < word.WordLength; i++)
            {
                foreach (var point in points)
                {
                    var block = blocksDict[point];
                    if (block.Character != word.CharArray[i]) continue;
                    if (WordCanBePlacedVertically(blocksDict, word, point, i + 1, out var placement, boardSize.Y))
                    {
                        if (placement != null) placements.Add(placement);
                    }
                    else if (WordCanBePlacedHorizontally(blocksDict, word, point, i + 1, out placement, boardSize.X))
                    {
                        if (placement != null) placements.Add(placement);
                    }
                }
            }
            return placements;
        }

        private static bool WordCanBePlacedHorizontally(Dictionary<Point, LetterBlock> blocksDict, Word word,
            Point coordinates, int letterIndex, out Placement placement, int xSize)
        {
            placement = new Placement(word, true);
            var currentCoordinates = coordinates;

            // If there are blocks to the left or right of the current block
            var left = GetPointWithOffset(currentCoordinates, -1, 0);
            var right = GetPointWithOffset(currentCoordinates, 1, 0);
            if (!OutOfBounds(blocksDict, left) && blocksDict.ContainsKey(left) ||
                !OutOfBounds(blocksDict, right) && blocksDict.ContainsKey(right))
            {
                return false;
            }

            placement.LetterBlocks[letterIndex - 1] =
                new LetterBlock(word.CharArray[letterIndex - 1], coordinates);

            for (var i = letterIndex - 1; i > 0; i--) //check all preceding letters
            {
                currentCoordinates.X--;
                if (currentCoordinates.X < 1)
                {
                    placement.Expansion.Left++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(blocksDict, currentCoordinates, word.CharArray, i, -1, "L", new Point(currentCoordinates.X - 1, currentCoordinates.Y), placement))
                {
                    return false;
                }
                placement.LetterBlocks[i - 1] = new LetterBlock(word.CharArray[i - 1], new Point(currentCoordinates.X, currentCoordinates.Y));
            }

            currentCoordinates = new Point(coordinates.X, coordinates.Y);

            for (var i = letterIndex + 1; i <= word.WordLength; i++) //check all succeeding letters
            {
                currentCoordinates.X++;
                if (currentCoordinates.X > xSize)
                {
                    placement.Expansion.Right++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(blocksDict, currentCoordinates, word.CharArray, i, 1, "R", new Point(currentCoordinates.X + 1, currentCoordinates.Y), placement))
                {
                    return false;
                }
                placement.LetterBlocks[i - 1] = new LetterBlock(word.CharArray[i - 1], new Point(currentCoordinates.X, currentCoordinates.Y));
            }

            if (placement.Expansion.Left > 0)
            {
                foreach (var letterBlock in placement.LetterBlocks)
                {
                    letterBlock.Coordinates.X += placement.Expansion.Left;
                }
            }

            return true;
        }

        private static bool WordCanBePlacedVertically(Dictionary<Point, LetterBlock> blocksDict, Word word,
            Point currentCoordinates, int letterIndex, out Placement placement, int ySize)
        {
            placement = new Placement(word, false);
            var coordinates = currentCoordinates;

            // If there are blocks above or below the current block
            var below = GetPointWithOffset(coordinates, 0, 1);
            var above = GetPointWithOffset(coordinates, 0, -1);
            if (!OutOfBounds(blocksDict, below) && blocksDict.ContainsKey(above) ||
                !OutOfBounds(blocksDict, above) && blocksDict.ContainsKey(below))
            {
                return false;
            }

            placement.LetterBlocks[letterIndex - 1] = new LetterBlock(word.CharArray[letterIndex - 1], currentCoordinates);


            for (var i = letterIndex - 1; i > 0; i--) //check all preceding letters
            {
                coordinates.Y--;
                if (coordinates.Y < 1)
                {
                    placement.Expansion.Up++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(blocksDict, coordinates, word.CharArray, i, -1, "U", new Point(coordinates.X, coordinates.Y - 1), placement))
                {
                    return false;
                }
                placement.LetterBlocks[i - 1] = new LetterBlock(word.CharArray[i - 1], new Point(coordinates.X, coordinates.Y));
            }

            coordinates = new Point(currentCoordinates.X, currentCoordinates.Y);
            for (var i = letterIndex + 1; i <= word.WordLength; i++) //check all succeeding letters
            {
                coordinates.Y++;
                if (coordinates.Y > ySize)
                {
                    placement.Expansion.Down++;
                    placement.NewLetters++;
                }
                else if (!LetterCanBePlaced(blocksDict, coordinates, word.CharArray, i, 1, "D", new Point(coordinates.X, coordinates.Y + 1), placement))
                {
                    return false;
                }
                placement.LetterBlocks[i - 1] = new LetterBlock(word.CharArray[i - 1], new Point(coordinates.X, coordinates.Y));
            }

            if (placement.Expansion.Up > 0)
            {
                foreach (var letterBlock in placement.LetterBlocks)
                {
                    letterBlock.Coordinates.Y += placement.Expansion.Up;
                }
            }

            return true;
        }

        private static bool LetterCanBePlaced(Dictionary<Point, LetterBlock> blocksDict,
            Point coordinates, IReadOnlyList<char> letters, int letterIndex, int nextLetterStep, string direction,
            Point nextPoint, Placement placement)
        {
            var letter = letters[letterIndex - 1];

            var blockOccupied = blocksDict.TryGetValue(new Point(coordinates.X, coordinates.Y), out var block);
            var nextBlockOccupied = blocksDict.TryGetValue(new Point(nextPoint.X, nextPoint.Y), out var nextBlock);

            if (blockOccupied)
            {
                if (block.Character != letter)
                {
                    return false;
                }

                return !nextBlockOccupied;
            }

            var nextLetter = letterIndex + nextLetterStep > letters.Count || letterIndex + nextLetterStep < 1
                ? ' '
                : letters[letterIndex + nextLetterStep - 1];

            //Fail if the above/below or left/right blocks have letters when direction is horizontal/vertical
            if (direction == "L" || direction == "R")
            {
                var below = GetPointWithOffset(coordinates, 0, 1);
                var above = GetPointWithOffset(coordinates, 0, -1);
                if (!OutOfBounds(blocksDict, above) && blocksDict.ContainsKey(above) || !OutOfBounds(blocksDict, below) && blocksDict.ContainsKey(below))
                {
                    return false;
                }
            }
            else
            {
                var right = GetPointWithOffset(coordinates, 1, 0);
                var left = GetPointWithOffset(coordinates, -1, 0);
                if (!OutOfBounds(blocksDict, right) && blocksDict.ContainsKey(right) || !OutOfBounds(blocksDict, left) && blocksDict.ContainsKey(left))
                {
                    return false;
                }
            }

            placement.NewLetters++;

            return
                // True if next two blocks are empty
                char.IsWhiteSpace(nextLetter) && !nextBlockOccupied ||
                // True if next block is empty or next block has same character as next letter
                !char.IsWhiteSpace(nextLetter) && (!nextBlockOccupied || nextBlock.Character == nextLetter);
        }

        private static bool OutOfBounds(Dictionary<Point, LetterBlock> blocksDict, Point coordinates)
        {
            return !(coordinates.X <= blocksDict.Keys.Max(p => p.X) && 
                     coordinates.X > 0 &&
                     coordinates.Y <= blocksDict.Keys.Max(p => p.Y) && 
                     coordinates.Y > 0); 
        }

        private static Point GetBoardSize(Dictionary<Point, LetterBlock> blocksDict)
        {
            return new Point(blocksDict.Keys.Max(x => x.X), blocksDict.Keys.Max(y => y.Y));
        }

        private static Point GetPointWithOffset(Point point, int xOffset, int yOffset)
        {
            var newPoint = point;
            newPoint.Offset(xOffset, yOffset);
            return newPoint;

        }
    }
}