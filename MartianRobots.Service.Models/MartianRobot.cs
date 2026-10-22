using MartianRobots.Service.Common.Enums.MartianRobot;

namespace MartianRobots.Service.Models
{
    /// <summary>
    /// Represents a Martian Robot that can be placed on the grid surface, move around, and potentially fall off the grid.
    /// </summary>
    /// <param name="startingOrientation">The orientation in which the robot starts.</param>
    /// <param name="navigationInstructions">The set of navigation instructions for the robot.</param>
    public class MartianRobot(Orientation startingOrientation, string[] navigationInstructions)
    {
        public Orientation? Orientation { get; set; } = startingOrientation;

        public string[] NavigationInstructions { get; set; } = navigationInstructions;
    }
}
