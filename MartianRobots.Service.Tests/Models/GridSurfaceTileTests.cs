using MartianRobots.Service.Models;

namespace MartianRobots.Service.Tests.Models
{
    public class GridSurfaceTileTests
    {
        [Fact]
        public void Constructor_SetsCoordinates()
        {
            var tile = new GridSurfaceTile(3, 7);

            Assert.Equal(3, tile.XCoordinate);
            Assert.Equal(7, tile.YCoordinate);
        }

        [Fact]
        public void Constructor_NewTile_IsUnoccupiedAndHasNoScent()
        {
            var tile = new GridSurfaceTile(0, 0);

            Assert.False(tile.IsOccupied);
            Assert.False(tile.HasScent);
        }

        [Fact]
        public void Properties_CanBeUpdated()
        {
            var tile = new GridSurfaceTile(0, 0)
            {
                IsOccupied = true,
                HasScent = true
            };

            Assert.True(tile.IsOccupied);
            Assert.True(tile.HasScent);
        }
    }
}
