using MartianRobots.Service.Common.Enums.MartianRobot;
using MartianRobots.Service.Common.Extensions;

namespace MartianRobots.Service.Tests.Extensions
{
    public class MartianRobotExtensionsTests
    {
        #region ToEnum

        [Theory]
        [InlineData("l", Instructions.Left)]
        [InlineData("r", Instructions.Right)]
        [InlineData("f", Instructions.Forward)]
        public void ToEnum_ValidLowercaseInstruction_ReturnsMatchingEnum(string instruction, Instructions expected)
        {
            var result = instruction.ToEnum();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("L", Instructions.Left)]
        [InlineData("R", Instructions.Right)]
        [InlineData("F", Instructions.Forward)]
        public void ToEnum_ValidUppercaseInstruction_IsCaseInsensitive(string instruction, Instructions expected)
        {
            var result = instruction.ToEnum();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("x")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("left")]
        [InlineData("ff")]
        [InlineData(" f")]
        public void ToEnum_InvalidInstruction_ThrowsArgumentException(string instruction)
        {
            var exception = Assert.Throws<ArgumentException>(() => instruction.ToEnum());

            Assert.Equal($"Invalid instruction: {instruction}", exception.Message);
        }

        #endregion ToEnum

        #region AsDescription

        [Theory]
        [InlineData(Instructions.Left, "Rotated Left")]
        [InlineData(Instructions.Right, "Rotated Right")]
        [InlineData(Instructions.Forward, "Moved Forward")]
        public void AsDescription_ValidInstruction_ReturnsDescription(Instructions instruction, string expected)
        {
            var result = instruction.AsDescription();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void AsDescription_UndefinedInstruction_ThrowsArgumentException()
        {
            var undefined = (Instructions)99;

            var exception = Assert.Throws<ArgumentException>(() => undefined.AsDescription());

            Assert.Equal("Invalid instruction: 99", exception.Message);
        }

        #endregion AsDescription
    }
}
