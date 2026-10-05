using System;
using System.IO;
using pis1;
using Xunit;

namespace pis1.Tests
{
    public class RateManagerTests
    {
        [Fact]
        public void Manager_AddAndInspect()
        {
            var m = new RateManager();

            Assert.True(m.IsEmpty());
            Assert.Equal(0, m.CbCount);
            Assert.Equal(0, m.ExchangerCount);

            m.AddCbRate(new Rate("RUB", "USD", 70, new DateTime(2026, 1, 1)));
            m.AddExchanger(new Exchanger("RUB", "USD", new DateTime(2026, 1, 1), "Сбер", 90));

            Assert.False(m.IsEmpty());
            Assert.Equal(1, m.CbCount);
            Assert.Equal(1, m.ExchangerCount);

            Assert.Single(m.GetCbRates());
            Assert.Single(m.GetExchangers());

            Assert.NotNull(m.GetExchangerAt(0));
            Assert.Null(m.GetExchangerAt(5));
            Assert.Null(m.GetExchangerAt(-1));

            Assert.Throws<ArgumentNullException>(() => m.AddCbRate(null));
            Assert.Throws<ArgumentNullException>(() => m.AddExchanger(null));
        }

        [Fact]
        public void Manager_FindCbRate_AllCases()
        {
            var m = new RateManager();
            m.AddCbRate(new Rate("RUB", "USD", 70, new DateTime(2026, 1, 1)));
            m.AddCbRate(new Rate("EUR", "RUB", 100, new DateTime(2026, 1, 1), false));

            Assert.NotNull(m.FindCbRate("RUB", "USD"));
            Assert.NotNull(m.FindCbRate("USD", "RUB"));
            Assert.Null(m.FindCbRate("EUR", "RUB"));
            Assert.Null(m.FindCbRate("GBP", "JPY"));
        }

        [Fact]
        public void Manager_Convert_AllCases()
        {
            var m = new RateManager();

            var cb = new Rate("RUB", "USD", 70, new DateTime(2026, 1, 1));
            var ex = new Exchanger("RUB", "USD", new DateTime(2026, 1, 1), "Сбер", 90);
            var inactive = new Rate("EUR", "RUB", 100, new DateTime(2026, 1, 1), false);

            m.AddCbRate(cb);
            m.AddCbRate(inactive);
            m.AddExchanger(ex);

            Assert.Equal(100.0 / 70.0, m.Convert(cb, 100, "RUB", "USD"), 5);
            Assert.Equal(100.0 / 90.0, m.Convert(ex, 100, "RUB", "USD"), 5);

            Assert.Throws<Exception>(() => m.Convert(inactive, 100, "EUR", "RUB"));
            Assert.Throws<Exception>(() => m.Convert(cb, 100, "GBP", "JPY"));
            Assert.Throws<ArgumentNullException>(() => m.Convert(null, 100, "RUB", "USD"));
        }

        [Fact]
        public void Manager_SaveAndLoad_RoundTrip()
        {
            var m1 = new RateManager();
            m1.AddCbRate(new Rate("RUB", "USD", 70, new DateTime(2026, 1, 1)));
            m1.AddCbRate(new Rate("EUR", "RUB", 100, new DateTime(2026, 1, 1), false));
            m1.AddExchanger(new Exchanger("RUB", "USD", new DateTime(2026, 1, 1),
                                          "Сбер", 90));

            string path = Path.GetTempFileName();
            try
            {
                m1.SaveToFile(path);

                var m2 = new RateManager();
                var (cb, ex, errors) = m2.LoadFromFile(path);

                Assert.Equal(2, cb);
                Assert.Equal(1, ex);
                Assert.Empty(errors);

                Assert.Equal(2, m2.CbCount);
                Assert.Equal(1, m2.ExchangerCount);

                Assert.True(m2.GetCbRates()[0].IsActive);
                Assert.False(m2.GetCbRates()[1].IsActive);

                Assert.Equal("Сбер", m2.GetExchangers()[0].Name);
                Assert.Equal(90, m2.GetExchangers()[0].OwnCourse);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Manager_Load_HandlesBadLines()
        {
            string path = Path.GetTempFileName();
            try
            {
                File.WriteAllLines(path, new[]
                {
                    "# комментарий",
                    "",
                    "ЦБ RUB USD 70 2026.01.01 true",
                    "плохая строка без полей",
                    "ЦБ EUR RUB 100 2026.01.01 false",
                    "\"Сбер\" RUB USD 90 2026.01.01 true"
                });

                var m = new RateManager();
                var (cb, ex, errors) = m.LoadFromFile(path);

                Assert.Equal(2, cb);
                Assert.Equal(1, ex);
                Assert.Single(errors);
                Assert.Contains("Строка 4", errors[0]);
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}