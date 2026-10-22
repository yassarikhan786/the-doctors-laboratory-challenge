using MartianRobots.Service.Common.Enums.MartianRobot;
using MartianRobots.Service.Models;

namespace MartianRobots.Service
{
    public class Program
    {
        static void Main()
        {
            // Create the GridSurface, giving it a width and height. Since the top right corner of the grid is (5, 3), the width is 6 and
            // the height is 4 (using zero-based indexing).
            var gridSurface = new GridSurface(6, 4);
            var martianRobotConfigs = new Dictionary<(int xCoordinate, int yCoordinate), MartianRobot>
            {
                { (1, 1), new MartianRobot(Orientation.East, ["r", "f", "r", "f", "r", "f", "r", "f"]) },
                { (3, 2), new MartianRobot(Orientation.North, ["f", "r", "r", "f", "l", "l", "f", "f", "r", "r", "f", "l", "l"]) },
                { (0, 3), new MartianRobot(Orientation.West, ["l", "l", "f", "f", "f", "l", "f", "l", "f", "l"]) }
            };

            foreach (var martianRobotConfig in martianRobotConfigs)
            {
                gridSurface.PlaceRobot(martianRobotConfig.Value, martianRobotConfig.Key.xCoordinate, martianRobotConfig.Key.yCoordinate);
                Console.WriteLine(gridSurface.InstructMartianRobot());
                Console.WriteLine();
            }

            Console.Read();
        }
    }
}
