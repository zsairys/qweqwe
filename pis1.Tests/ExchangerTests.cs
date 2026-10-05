using System;
using pis1;
using Xunit;

namespace pis1.Tests
{
    public class ExchangerTests
    {
        [Fact]
        public void Exchanger_AllBranches()
        {
            var date = new DateTime(2026, 1, 1);

            var ex = new Exchanger("RUB", "USD", date, "Сбер", 90);

            Assert.Equal("RUB", ex.From);
            Assert.Equal("USD", ex.To);
            Assert.Equal(date, ex.Date);
            Assert.Equal(90, ex.OwnCourse);
            Assert.Equal("Сбер", ex.Name);
            Assert.True(ex.IsActive);

            Assert.True(ex.Matches("RUB", "USD"));
            Assert.True(ex.Matches("rub", "usd"));
            Assert.False(ex.Matches("EUR", "RUB"));


            Assert.Equal(100.0 / 90.0, ex.Convert(100, "RUB", "USD"), 5);
            Assert.Equal(100.0 * 90.0, ex.Convert(100, "USD", "RUB"), 5);

            Assert.Throws<Exception>(() => ex.Convert(100, "EUR", "GBP"));

            string active = ex.toString();
            Assert.Contains("Сбер", active);
            Assert.Contains("RUB", active);
            Assert.Contains("USD", active);
            Assert.Contains("90", active);
            Assert.DoesNotContain("НЕАКТИВЕН", active);

            var inactive = new Exchanger("RUB", "USD", date, "Тинькофф", 91, false);
            Assert.False(inactive.IsActive);

            string inactiveStr = inactive.toString();
            Assert.Contains("НЕАКТИВЕН", inactiveStr);

            Rate asRate = new Exchanger("RUB", "USD", date, "Альфа", 88);

            Assert.Equal(100.0 / 88.0, asRate.Convert(100, "RUB", "USD"), 5);
            Assert.Contains("Альфа", asRate.toString());
        }
    }
}