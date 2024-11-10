using Xunit;
using Flap;

namespace Flap.Tests
{
    public class ButterflyTests
    {
        [Fact]
        public void FindHome_ReturnsPosition()
        {
            // Arrange
            var position = (10f, 5f, 0f);
            var isabella = new Butterfly("Delicious nectar", position);

            // Act
            var home = isabella.FindHome();

            // Assert
            Assert.Equal(position, home);
        }
    }
}
