using System;
using pis1;
using Xunit;

namespace pis1.Tests
{
    public class RateTests
    {
        [Fact]
        public void Rate_AllBehaviors()
        {
            var r = new Rate("RUB", "USD", 70, new DateTime(2026, 1, 1));

            Assert.True(r.Matches("RUB", "USD"));
            Assert.True(r.Matches("rub", "usd"));
            Assert.False(r.Matches("EUR", "USD"));

            Assert.Equal(100.0 / 70.0, r.Convert(100, "RUB", "USD"), 5);
            Assert.Equal(100.0 * 70.0, r.Convert(100, "USD", "RUB"), 5);

            Assert.Throws<Exception>(() => r.Convert(100, "EUR", "GBP"));

            Assert.Contains("RUB", r.toString());
            Assert.Contains("USD", r.toString());
            Assert.Contains("70", r.toString());

            Assert.True(r.IsActive);

            Rate inactive = new Rate("RUB", "USD", 70, new DateTime(2026, 1, 1), false);
            Assert.False(inactive.IsActive);
            Assert.Contains("Õ≈¿ “»¬≈Õ", inactive.toString());
        }

        [Theory]
        [InlineData("RUB", "USD", true)]
        [InlineData("rub", "usd", true)]
        [InlineData("Rub", "Usd", true)]
        [InlineData("EUR", "USD", false)]
        [InlineData("RUB", "EUR", false)]
        [InlineData("", "", false)]
        public void Rate_Matches_Various(string from, string to, bool expected)
        {
            var r = new Rate("RUB", "USD", 70, new DateTime(2026, 1, 1));
            Assert.Equal(expected, r.Matches(from, to));
        }
    }
}