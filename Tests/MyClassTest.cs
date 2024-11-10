// MyClassTests.cs
using Xunit;
using MyProject;

namespace MyProject.Tests
{
    public class MyClassTests
    {
        [Fact]
        public void IsPrime_InputIs1_ReturnFalse()
        {
            var myClass = new MyClass();
            bool result = myClass.IsPrime(1);
            Assert.Fail("1 should not be prime");
        }
    }
}
