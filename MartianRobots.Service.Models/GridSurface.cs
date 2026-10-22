using MartianRobots.Service.Common.Enums.MartianRobot;
using MartianRobots.Service.Common.Extensions;

namespace MartianRobots.Service.Models
{
    /// <summary>
    /// Represents the grid surface on which Martian robots can move. The grid is defined by its width and height, and consists of individual tiles 
    /// that can be occupied by robots or marked with a "scent" to indicate that a robot has previously fallen off the grid at that location.
    /// </summary>
    /// <param name="width">The width of the grid surface.</param>
    /// <param name="height">The height of the grid surface.</param>
    public class GridSurface(int width, int height)
    {
        private GridSurfaceTile? _occupiedTile;
        private MartianRobot? _martianRobot;
        private const string InvalidOrientation = "Invalid orientation";        

        public bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public void PlaceRobot(MartianRobot martianRobot, int x, int y)
        {
            ResetGridSurface();

            if (!IsWithinBounds(x, y))
            {
                throw new ArgumentOutOfRangeException($"Coordinates ({x}, {y}) are out of bounds for the grid surface.");
            }

            var tile = GridSurfaceTiles[x, y];
            if (tile.IsOccupied)
            {
                throw new InvalidOperationException($"Tile at coordinates ({x}, {y}) is already occupied by another robot.");
            }

            tile.IsOccupied = true;
            _occupiedTile = tile;
            _martianRobot = martianRobot;
        }

        /// <summary>
        /// Executes the navigation instructions for the robot placed on the grid surface. The robot will move according to its
        /// instruction set, and if it attempts to move off the grid, it will leave a "scent" on the last valid tile it occupied. 
        /// The method returns a string indicating the final position and orientation of the robot, or if it has fallen off the grid.
        /// </summary>
        /// <returns></returns>
        public string InstructMartianRobot()
        {
            var xPos = _occupiedTile?.XCoordinate;
            var yPos = _occupiedTile?.YCoordinate;

            if (_martianRobot?.NavigationInstructions == null)
            {
                throw new InvalidOperationException("Navigation instructions have not been set.");
            }

            Console.WriteLine($"Starting position: ({xPos}, {yPos}), Orientation: {_martianRobot.Orientation}");

            foreach (var instruction in _martianRobot?.NavigationInstructions!)
            {
                var currentInstruction = instruction.ToEnum();
                switch (currentInstruction)
                {
                    case Instructions.Left:
                        _martianRobot.Orientation = _martianRobot.Orientation switch
                        {
                            Orientation.North => Orientation.West,
                            Orientation.West => Orientation.South,
                            Orientation.South => Orientation.East,
                            Orientation.East => Orientation.North,
                            _ => throw new InvalidOperationException(InvalidOrientation)
                        };
                        break;
                    case Instructions.Right:
                        _martianRobot.Orientation = _martianRobot.Orientation switch
                        {
                            Orientation.North => Orientation.East,
                            Orientation.East => Orientation.South,
                            Orientation.South => Orientation.West,
                            Orientation.West => Orientation.North,
                            _ => throw new InvalidOperationException(InvalidOrientation)
                        };
                        break;
                    case Instructions.Forward:
                        int currentX = xPos ?? 0;
                        int currentY = yPos ?? 0;

                        int newX = xPos ?? 0;
                        int newY = yPos ?? 0;
                        var currentGridSurfaceTile = GridSurfaceTiles[currentX, currentY];

                        switch (_martianRobot.Orientation)
                        {
                            case Orientation.North:
                                newY++;
                                break;
                            case Orientation.East:
                                newX++;
                                break;
                            case Orientation.South:
                                newY--;
                                break;
                            case Orientation.West:
                                newX--;
                                break;
                        }

                        // If the new position is out of bounds, check if the current tile has a scent. If it does, ignore the move to
                        // prevent the robot from falling off the grid. The condition won't be met if the new coordinates are within bounds,
                        // regardless of the grid surface tile having a scent or not. 
                        if (!IsWithinBounds(newX, newY) && currentGridSurfaceTile.HasScent)
                        {
                            continue; // Skip the forward instruction if the current tile has a scent and the robot would fall off the grid.
                        }

                        if (IsWithinBounds(newX, newY))
                        {
                            // Move the robot to the new tile
                            GridSurfaceTiles[xPos ?? 0, yPos ?? 0].IsOccupied = false; // Free the old tile
                            GridSurfaceTiles[newX, newY].IsOccupied = true; // Occupy the new tile
                            xPos = newX;
                            yPos = newY;
                        }
                        else
                        {
                            // Robot has fallen off the grid, leave a scent on the last valid tile
                            GridSurfaceTiles[xPos ?? 0, yPos ?? 0].HasScent = true;
                            return $"Ending Position: {xPos} {yPos} {_martianRobot.Orientation} LOST";
                        }
                        break;
                }
                Console.WriteLine(currentInstruction.AsDescription() + " - Martian Robot Position: " + xPos + ", " + yPos + ", Orientation: " + _martianRobot.Orientation);
                PrintGridSurface();
            }
            return $"Ending Position: {xPos} {yPos} {_martianRobot.Orientation}";
        }

        #region Private Helper Methods

        private static GridSurfaceTile[,] Initialise(int width, int height)
        {
            var tiles = new GridSurfaceTile[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y] = new GridSurfaceTile(x, y);
                }
            }

            return tiles;
        }

        private void PrintGridSurface()
        {
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    var tile = GridSurfaceTiles[x, y];
                    if (tile.IsOccupied)
                    {
                        Console.Write("R "); // R for Robot
                    }
                    else if (tile.HasScent)
                    {
                        Console.Write("S "); // S for Scent
                    }
                    else
                    {
                        Console.Write(". "); // . for empty tile
                    }
                }
                Console.WriteLine();
            }
        }

        private void ResetGridSurface()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridSurfaceTiles[x, y].IsOccupied = false;
                }
            }
        }

        #endregion Private Helper Methods

        #region Properties

        public GridSurfaceTile[,] GridSurfaceTiles { get; set; } = Initialise(width, height);

        #endregion 
    }
}