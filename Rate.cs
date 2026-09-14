using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pis1
{
    class Rate
    {
        public string From { get; set; }
        public string To { get; set; }
        public double Course { get; set; }
        public DateTime Date { get; set; }

        public Rate(string v1, string v2, double course, DateTime date)
        {
            From = v1;
            To = v2;
            Course = course;
            Date = date;
        }

        public string toString()
        {
            return $"Курс {From} к {To} равен {Course} на момент {Date.ToString("yyyy.MM.dd")}";
        }
    }
}
