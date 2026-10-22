using MartianRobots.Service.Common.Enums.MartianRobot;
using MartianRobots.Service.Models;

namespace MartianRobots.Service.Tests.Models
{
    public class MartianRobotTests
    {
        [Theory]
        [InlineData(Orientation.North)]
        [InlineData(Orientation.East)]
        [InlineData(Orientation.South)]
        [InlineData(Orientation.West)]
        public void Constructor_SetsStartingOrientation(Orientation orientation)
        {
            var robot = new MartianRobot(orientation, ["f"]);

            Assert.Equal(orientation, robot.Orientation);
        }

        [Fact]
        public void Constructor_SetsNavigationInstructions()
        {
            string[] instructions = ["l", "r", "f"];

            var robot = new MartianRobot(Orientation.North, instructions);

            Assert.Equal(instructions, robot.NavigationInstructions);
        }
    }
}
