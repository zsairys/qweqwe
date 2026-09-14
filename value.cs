using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pis1
{
    class Rate
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public double Course { get; set; }
        public DateTime Date { get; set; }

        public Rate(string v1, string v2, double course, DateTime date)
        {
            Value1 = v1;
            Value2 = v2;
            Course = course;
            Date = date;
        }

        public string Conv()
        {
            return $"Курс {Value1} к {Value2} равен {Course} на момент {Date.ToString("yyyy.MM.dd")}";
        }
    }
}
