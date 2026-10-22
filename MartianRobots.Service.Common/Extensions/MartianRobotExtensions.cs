using MartianRobots.Service.Common.Enums.MartianRobot;

namespace MartianRobots.Service.Common.Extensions
{
    public static class MartianRobotExtensions
    {
        public static Instructions ToEnum(this string instruction)
        {
            return instruction.ToLower() switch
            {
                "l" => Instructions.Left,
                "r" => Instructions.Right,
                "f" => Instructions.Forward,
                _ => throw new ArgumentException($"Invalid instruction: {instruction}")
            };
        }

        public static string AsDescription(this Instructions instruction)
        {
            return instruction switch
            {
                Instructions.Left => "Rotated Left",
                Instructions.Right => "Rotated Right",
                Instructions.Forward => "Moved Forward",
                _ => throw new ArgumentException($"Invalid instruction: {instruction}")
            };
        }
    }
}
