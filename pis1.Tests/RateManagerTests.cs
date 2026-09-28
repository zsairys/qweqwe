using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace pis1.Tests
{
    public class RateManagerTests
    {
        [Fact]
        public void AddCbRate_AddsToList()
        {
            var m = new RateManager();
            var r = new Rate("USD", "RUB", 92.5, new DateTime(2026, 1, 1));
            m.AddCbRate(r);

            Assert.Equal(1, m.CbCount);
            Assert.Same(r, m.GetCbRates()[0]);
        }

        [Fact]
        public void AddCbRate_Null_Throws()
        {
            var m = new RateManager();  
            Assert.Throws<ArgumentNullException>(() => m.AddCbRate(null));
        }

        [Fact]
        public void AddExchanger_Null_Throws()
        {
            var m = new RateManager();
            Assert.Throws<ArgumentNullException>(() => m.AddExchanger(null));
        }

        [Fact]
        public void IsEmpty_TrueForNewManager()
        {
            var m = new RateManager();
            Assert.True(m.IsEmpty());
        }

        [Fact]
        public void IsEmpty_FalseAfterAdding()
        {
            var m = new RateManager();
            m.AddCbRate(new Rate("USD", "RUB", 92.5, new DateTime(2026, 1, 1)));

            Assert.False(m.IsEmpty());
        }

        [Fact]
        public void FindCbRate_FindsActiveRate()
        {
            var m = new RateManager();
            m.AddCbRate(new Rate("USD", "RUB", 92.5, new DateTime(2026, 1, 1)));

            var found = m.FindCbRate("USD", "RUB");

            Assert.NotNull(found);
            Assert.Equal("USD", found.From);
        }

        [Fact]
        public void FindCbRate_FindsReverseDirection()
        {
            var m = new RateManager();
            m.AddCbRate(new Rate("USD", "RUB", 92.5, new DateTime(2026, 1, 1)));
            var found = m.FindCbRate("RUB", "USD");

            Assert.NotNull(found);
        }

        [Fact]
        public void FindCbRate_SkipsInactive()
        {
            var m = new RateManager();
            m.AddCbRate(new Rate("USD", "RUB", 92.5, new DateTime(2026, 1, 1), false));

            var found = m.FindCbRate("USD", "RUB");

            Assert.Null(found);
        }

        [Fact]
        public void Convert_ThroughCb_Works()
        {
            var m = new RateManager();
            var r = new Rate("RUB", "USD", 70, new DateTime(2026, 1, 1));
            m.AddCbRate(r);

            double result = m.Convert(r, 100, "RUB", "USD");

            Assert.Equal(100.0/70.0, result, 5);
        }

        [Fact]
        public void Convert_InactiveRate_Throws()
        {
            var m = new RateManager();
            var r = new Rate("RUB", "USD", 70, new DateTime(2026, 1, 1), false);
            m.AddCbRate(r);

            Assert.Throws<Exception>(() => m.Convert(r, 100, "RUB", "USD"));
        }
    }
}