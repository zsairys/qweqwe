using System;
using Xunit;


namespace pis1.Tests
{
    public class RateTests
    {
        [Fact]
        public void Matches_SamePair_ReturnsTrue()
        {
            var r = new Rate("USD", "RUB", 92.5, new DateTime(2026, 1, 1));

            Assert.True(r.Matches("USD", "RUB"));
        }

        [Fact]
        public void Matches_DifferentPair_ReturnFalse()
        {
            var r = new Rate("USD", "RUB", 92.5, new DateTime(2026, 1, 1));

            Assert.False(r.Matches("EUR", "RUB"));
        }

        [Fact]
        public void Matches_IgnoresCase()
        {
            var r = new Rate("USD", "RUB", 92.5, new DateTime(2026, 1, 1));

            Assert.True(r.Matches("usd", "rub"));
            Assert.True(r.Matches("Usd", "Rub"));
        }

        [Fact]
        public void Convert_DirectDirection_Divides()
        {
            var r = new Rate("RUB", "USD", 70, new DateTime(2026, 1, 1));
            double result = r.Convert(100, "RUB", "USD");
            Assert.Equal(100.0 / 70.0, result, 5);
        }

        [Fact]
        public void Convert_ReverseDirection_Multiplies()
        {
            var r = new Rate("RUB", "USD", 70, new DateTime(2026, 1, 1));
            double result = r.Convert(100, "USD", "RUB");
            Assert.Equal(100.0 * 70.0, result, 5);
        }

        [Fact]
        public void Convert_WrongPair_Throws()
        {
            var r = new Rate("RUB", "USD", 70, new DateTime(2026, 1, 1));
            Assert.Throws<Exception>(() => r.Convert(100, "EUR", "GBP"));
        }

        [Fact]
        public void IsActive_DefaultTrue()
        {
            var r = new Rate("USD", "RUB", 92.5, new DateTime(2026, 1, 1));
            Assert.True(r.IsActive);
        }

        [Fact]
        public void ToString_InactiveRate_ContainsMarker()
        {
            var r = new Rate("USD", "RUB", 92.5, new DateTime(2026, 1, 1), false);

            Assert.Contains("Õ≈¿ “»¬≈Õ", r.toString());
        }
    }
}