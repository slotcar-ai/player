using System;
using Xunit;
using FluentAssertions;

namespace Player.Tests
{
    public class HelloWorldTest
    {
        [Fact]
        public void XunitShouldWork()
        {
            var theBoolean = true;

            theBoolean.Should().BeTrue();
        }
    }
}
