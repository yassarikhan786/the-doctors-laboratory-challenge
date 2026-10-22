namespace MartianRobots.Service.Models
{
    /// <summary>
    /// Represents a single tile on the grid surface, which can be occupied by a robot or marked 
    /// as a "scent" to indicate that a robot has previously fallen off the grid at that location.
    /// </summary>
    public class GridSurfaceTile(int xCoordinate, int yCoordinate)
    {
        #region Properties

        /// <summary>
        /// X Coordinate of the tile on the grid surface. This represents the horizontal position of the tile.
        /// </summary>
        public int XCoordinate { get; set; } = xCoordinate;

        /// <summary>
        /// Y Coordinate of the tile on the grid surface. This represents the vertical position of the tile.
        /// </summary>
        public int YCoordinate { get; set; } = yCoordinate;

        /// <summary>
        /// Whether the tile is currently occupied by a robot. If true, a robot is present on this tile; if false, the tile is unoccupied.
        /// </summary>
        public bool IsOccupied { get; set; }

        /// <summary>
        /// Whether the tile has a "scent" indicating that a robot has previously fallen off the grid at this location.
        /// </summary>
        public bool HasScent { get; set; }

        #endregion Properties
    }
}
