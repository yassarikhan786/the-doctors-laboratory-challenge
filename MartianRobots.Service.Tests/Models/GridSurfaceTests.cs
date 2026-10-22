using MartianRobots.Service.Common.Enums.MartianRobot;
using MartianRobots.Service.Models;

namespace MartianRobots.Service.Tests.Models
{
    public class GridSurfaceTests
    {
        // Matches the sample grid used in Program.cs: upper-right corner (5, 3).
        private const int SampleWidth = 6;
        private const int SampleHeight = 4;

        private static GridSurface CreateSampleGrid() => new(SampleWidth, SampleHeight);

        private static string Run(GridSurface grid, int x, int y, Orientation orientation, params string[] instructions)
        {
            grid.PlaceRobot(new MartianRobot(orientation, instructions), x, y);
            return grid.InstructMartianRobot();
        }

        #region Construction

        [Fact]
        public void Constructor_CreatesTileArrayOfRequestedSize()
        {
            var grid = new GridSurface(6, 4);

            Assert.Equal(6, grid.GridSurfaceTiles.GetLength(0));
            Assert.Equal(4, grid.GridSurfaceTiles.GetLength(1));
        }

        [Fact]
        public void Constructor_InitialisesEveryTileWithMatchingCoordinates()
        {
            var grid = new GridSurface(3, 2);

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 2; y++)
                {
                    var tile = grid.GridSurfaceTiles[x, y];
                    Assert.NotNull(tile);
                    Assert.Equal(x, tile.XCoordinate);
                    Assert.Equal(y, tile.YCoordinate);
                    Assert.False(tile.IsOccupied);
                    Assert.False(tile.HasScent);
                }
            }
        }

        #endregion Construction

        #region IsWithinBounds

        [Theory]
        [InlineData(0, 0)]
        [InlineData(5, 3)]
        [InlineData(0, 3)]
        [InlineData(5, 0)]
        [InlineData(2, 2)]
        public void IsWithinBounds_CoordinatesOnGrid_ReturnsTrue(int x, int y)
        {
            Assert.True(CreateSampleGrid().IsWithinBounds(x, y));
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(6, 0)]
        [InlineData(0, 4)]
        [InlineData(6, 4)]
        [InlineData(-1, -1)]
        public void IsWithinBounds_CoordinatesOffGrid_ReturnsFalse(int x, int y)
        {
            Assert.False(CreateSampleGrid().IsWithinBounds(x, y));
        }

        #endregion IsWithinBounds

        #region PlaceRobot

        [Fact]
        public void PlaceRobot_ValidCoordinates_OccupiesTile()
        {
            var grid = CreateSampleGrid();

            grid.PlaceRobot(new MartianRobot(Orientation.North, []), 2, 1);

            Assert.True(grid.GridSurfaceTiles[2, 1].IsOccupied);
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(6, 0)]
        [InlineData(0, 4)]
        public void PlaceRobot_OutOfBounds_ThrowsArgumentOutOfRangeException(int x, int y)
        {
            var grid = CreateSampleGrid();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                grid.PlaceRobot(new MartianRobot(Orientation.North, []), x, y));
        }

        [Fact]
        public void PlaceRobot_SecondRobot_ClearsPreviousRobotsTile()
        {
            var grid = CreateSampleGrid();
            grid.PlaceRobot(new MartianRobot(Orientation.North, []), 1, 1);

            grid.PlaceRobot(new MartianRobot(Orientation.North, []), 3, 2);

            Assert.False(grid.GridSurfaceTiles[1, 1].IsOccupied);
            Assert.True(grid.GridSurfaceTiles[3, 2].IsOccupied);
        }

        [Fact]
        public void PlaceRobot_OnSameTileAsPreviousRobot_DoesNotThrow()
        {
            // PlaceRobot resets occupancy before placing, so only one robot is ever on the grid at a time.
            var grid = CreateSampleGrid();
            grid.PlaceRobot(new MartianRobot(Orientation.North, []), 1, 1);

            var exception = Record.Exception(() => grid.PlaceRobot(new MartianRobot(Orientation.North, []), 1, 1));

            Assert.Null(exception);
            Assert.True(grid.GridSurfaceTiles[1, 1].IsOccupied);
        }

        [Fact]
        public void PlaceRobot_DoesNotClearScents()
        {
            var grid = CreateSampleGrid();
            Run(grid, 0, 3, Orientation.North, "f"); // falls off, leaves scent at (0, 3)

            grid.PlaceRobot(new MartianRobot(Orientation.North, []), 2, 2);

            Assert.True(grid.GridSurfaceTiles[0, 3].HasScent);
        }

        #endregion PlaceRobot

        #region InstructMartianRobot - guard clauses

        [Fact]
        public void InstructMartianRobot_NoRobotPlaced_ThrowsInvalidOperationException()
        {
            var grid = CreateSampleGrid();

            var exception = Assert.Throws<InvalidOperationException>(() => grid.InstructMartianRobot());

            Assert.Equal("Navigation instructions have not been set.", exception.Message);
        }

        [Fact]
        public void InstructMartianRobot_NullInstructions_ThrowsInvalidOperationException()
        {
            var grid = CreateSampleGrid();
            grid.PlaceRobot(new MartianRobot(Orientation.North, null!), 0, 0);

            Assert.Throws<InvalidOperationException>(() => grid.InstructMartianRobot());
        }

        [Fact]
        public void InstructMartianRobot_InvalidInstruction_ThrowsArgumentException()
        {
            var grid = CreateSampleGrid();
            grid.PlaceRobot(new MartianRobot(Orientation.North, ["f", "x"]), 0, 0);

            Assert.Throws<ArgumentException>(() => grid.InstructMartianRobot());
        }

        [Fact]
        public void InstructMartianRobot_EmptyInstructions_ReturnsStartingPosition()
        {
            var result = Run(CreateSampleGrid(), 2, 1, Orientation.West);

            Assert.Equal("Ending Position: 2 1 West", result);
        }

        #endregion InstructMartianRobot - guard clauses

        #region InstructMartianRobot - rotation

        [Theory]
        [InlineData(Orientation.North, Orientation.West)]
        [InlineData(Orientation.West, Orientation.South)]
        [InlineData(Orientation.South, Orientation.East)]
        [InlineData(Orientation.East, Orientation.North)]
        public void InstructMartianRobot_Left_RotatesAnticlockwise(Orientation start, Orientation expected)
        {
            var result = Run(CreateSampleGrid(), 1, 1, start, "l");

            Assert.Equal($"Ending Position: 1 1 {expected}", result);
        }

        [Theory]
        [InlineData(Orientation.North, Orientation.East)]
        [InlineData(Orientation.East, Orientation.South)]
        [InlineData(Orientation.South, Orientation.West)]
        [InlineData(Orientation.West, Orientation.North)]
        public void InstructMartianRobot_Right_RotatesClockwise(Orientation start, Orientation expected)
        {
            var result = Run(CreateSampleGrid(), 1, 1, start, "r");

            Assert.Equal($"Ending Position: 1 1 {expected}", result);
        }

        [Theory]
        [InlineData("l")]
        [InlineData("r")]
        public void InstructMartianRobot_FourRotationsSameDirection_ReturnsToStartingOrientation(string rotation)
        {
            var result = Run(CreateSampleGrid(), 1, 1, Orientation.North, rotation, rotation, rotation, rotation);

            Assert.Equal("Ending Position: 1 1 North", result);
        }

        [Fact]
        public void InstructMartianRobot_UpdatesRobotOrientation()
        {
            var grid = CreateSampleGrid();
            var robot = new MartianRobot(Orientation.North, ["r"]);
            grid.PlaceRobot(robot, 1, 1);

            grid.InstructMartianRobot();

            Assert.Equal(Orientation.East, robot.Orientation);
        }

        [Fact]
        public void InstructMartianRobot_UppercaseInstructions_AreAccepted()
        {
            var result = Run(CreateSampleGrid(), 1, 1, Orientation.North, "F", "R", "F");

            Assert.Equal("Ending Position: 2 2 East", result);
        }

        #endregion InstructMartianRobot - rotation

        #region InstructMartianRobot - movement

        [Theory]
        [InlineData(Orientation.North, 2, 3)]
        [InlineData(Orientation.East, 3, 2)]
        [InlineData(Orientation.South, 2, 1)]
        [InlineData(Orientation.West, 1, 2)]
        public void InstructMartianRobot_Forward_MovesOneTileInFacingDirection(Orientation orientation, int expectedX, int expectedY)
        {
            var result = Run(CreateSampleGrid(), 2, 2, orientation, "f");

            Assert.Equal($"Ending Position: {expectedX} {expectedY} {orientation}", result);
        }

        [Fact]
        public void InstructMartianRobot_Forward_MovesOccupancyToNewTile()
        {
            var grid = CreateSampleGrid();

            Run(grid, 2, 2, Orientation.East, "f");

            Assert.False(grid.GridSurfaceTiles[2, 2].IsOccupied);
            Assert.True(grid.GridSurfaceTiles[3, 2].IsOccupied);
        }

        [Fact]
        public void InstructMartianRobot_MultipleMoves_OnlyFinalTileOccupied()
        {
            var grid = CreateSampleGrid();

            Run(grid, 0, 0, Orientation.North, "f", "f", "r", "f", "f");

            var occupied = grid.GridSurfaceTiles.Cast<GridSurfaceTile>().Where(t => t.IsOccupied).ToList();
            Assert.Single(occupied);
            Assert.Equal((2, 2), (occupied[0].XCoordinate, occupied[0].YCoordinate));
        }

        [Fact]
        public void InstructMartianRobot_TraversesToFarCorner()
        {
            var result = Run(CreateSampleGrid(), 0, 0, Orientation.East, "f", "f", "f", "f", "f", "l", "f", "f", "f");

            Assert.Equal("Ending Position: 5 3 North", result);
        }

        #endregion InstructMartianRobot - movement

        #region InstructMartianRobot - falling off and scents

        [Theory]
        [InlineData(2, 3, Orientation.North)]
        [InlineData(5, 1, Orientation.East)]
        [InlineData(2, 0, Orientation.South)]
        [InlineData(0, 1, Orientation.West)]
        public void InstructMartianRobot_MovingOffEdge_ReturnsLostAtLastValidPosition(int x, int y, Orientation orientation)
        {
            var result = Run(CreateSampleGrid(), x, y, orientation, "f");

            Assert.Equal($"Ending Position: {x} {y} {orientation} LOST", result);
        }

        [Theory]
        [InlineData(2, 3, Orientation.North)]
        [InlineData(5, 1, Orientation.East)]
        [InlineData(2, 0, Orientation.South)]
        [InlineData(0, 1, Orientation.West)]
        public void InstructMartianRobot_MovingOffEdge_LeavesScentOnLastValidTile(int x, int y, Orientation orientation)
        {
            var grid = CreateSampleGrid();

            Run(grid, x, y, orientation, "f");

            Assert.True(grid.GridSurfaceTiles[x, y].HasScent);
            Assert.Equal(1, grid.GridSurfaceTiles.Cast<GridSurfaceTile>().Count(t => t.HasScent));
        }

        [Fact]
        public void InstructMartianRobot_AfterLost_RemainingInstructionsAreIgnored()
        {
            var grid = CreateSampleGrid();
            var robot = new MartianRobot(Orientation.North, ["f", "r", "r", "f"]);
            grid.PlaceRobot(robot, 1, 3);

            var result = grid.InstructMartianRobot();

            Assert.Equal("Ending Position: 1 3 North LOST", result);
            Assert.Equal(Orientation.North, robot.Orientation);
        }

        [Fact]
        public void InstructMartianRobot_ScentOnTile_PreventsSubsequentRobotFallingOff()
        {
            var grid = CreateSampleGrid();
            Run(grid, 3, 3, Orientation.North, "f");

            var result = Run(grid, 3, 3, Orientation.North, "f");

            Assert.Equal("Ending Position: 3 3 North", result);
        }

        [Fact]
        public void InstructMartianRobot_ScentOnTile_RobotContinuesWithLaterInstructions()
        {
            var grid = CreateSampleGrid();
            Run(grid, 3, 3, Orientation.North, "f");

            var result = Run(grid, 3, 2, Orientation.North, "f", "f", "r", "f");

            Assert.Equal("Ending Position: 4 3 East", result);
        }

        [Fact]
        public void InstructMartianRobot_ScentOnTile_DoesNotBlockMovesThatStayOnGrid()
        {
            var grid = CreateSampleGrid();
            Run(grid, 3, 3, Orientation.North, "f");

            var result = Run(grid, 3, 3, Orientation.East, "f");

            Assert.Equal("Ending Position: 4 3 East", result);
        }

        [Fact]
        public void InstructMartianRobot_ScentOnTile_ProtectsInAnyOffGridDirection()
        {
            // Scents are per tile, not per direction: a scent left by falling north also protects a westward fall from the corner.
            var grid = CreateSampleGrid();
            Run(grid, 0, 3, Orientation.North, "f");

            var result = Run(grid, 0, 3, Orientation.West, "f");

            Assert.Equal("Ending Position: 0 3 West", result);
        }

        [Fact]
        public void InstructMartianRobot_ScentOnDifferentTile_DoesNotProtectRobot()
        {
            var grid = CreateSampleGrid();
            Run(grid, 3, 3, Orientation.North, "f");

            var result = Run(grid, 2, 3, Orientation.North, "f");

            Assert.Equal("Ending Position: 2 3 North LOST", result);
            Assert.True(grid.GridSurfaceTiles[2, 3].HasScent);
        }

        [Fact]
        public void InstructMartianRobot_SingleTileGrid_AnyForwardMoveIsLost()
        {
            var result = Run(new GridSurface(1, 1), 0, 0, Orientation.South, "f");

            Assert.Equal("Ending Position: 0 0 South LOST", result);
        }

        #endregion InstructMartianRobot - falling off and scents

        #region Sample scenario

        [Fact]
        public void InstructMartianRobot_SampleInput_ProducesExpectedOutput()
        {
            // Mirrors the robots configured in Program.cs, run in order on the same grid so scents carry over.
            var grid = CreateSampleGrid();

            var first = Run(grid, 1, 1, Orientation.East, "r", "f", "r", "f", "r", "f", "r", "f");
            var second = Run(grid, 3, 2, Orientation.North, "f", "r", "r", "f", "l", "l", "f", "f", "r", "r", "f", "l", "l");
            var third = Run(grid, 0, 3, Orientation.West, "l", "l", "f", "f", "f", "l", "f", "l", "f", "l");

            Assert.Equal("Ending Position: 1 1 East", first);
            Assert.Equal("Ending Position: 3 3 North LOST", second);
            Assert.Equal("Ending Position: 2 3 South", third);
        }

        [Fact]
        public void InstructMartianRobot_SampleThirdRobotOnFreshGrid_IsLost()
        {
            // Without the scent left by the second robot, the third robot falls off at (3, 3).
            var result = Run(CreateSampleGrid(), 0, 3, Orientation.West, "l", "l", "f", "f", "f", "l", "f", "l", "f", "l");

            Assert.Equal("Ending Position: 3 3 North LOST", result);
        }

        #endregion Sample scenario
    }
}
