namespace Chatbot.Tests.Extensions
{
    using Chatbot.Extensions;
    using Xunit;

    public class NumberExtensionsTests
    {
        [Theory]
        [InlineData(1000, "1,000.00")]
        [InlineData(1434343430, "1,434,343,430.00")]
        [InlineData(-3434343343, "-3,434,343,343.00")]
        [InlineData(0, "0.00")]
        [InlineData(-1, "-1.00")]
        [InlineData(-1.256, "-1.26")]
        [InlineData(-1.254, "-1.25")]
        [InlineData(1.255, "1.26")]
        [InlineData(1.254, "1.25")]
        public void ToFormattedNumber_Always_ReturnFormattedNumber(double number, string expected)
        {
            var actual = number.ToFormattedNumber();

            Assert.Equal(expected, actual);
        }
    }
}