using System;

namespace pis1
{
    class Rate
    {
        public string From { get; set; }
        public string To { get; set; }
        public double Course { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
        public Rate(string v1, string v2, double course, DateTime date, bool isActive)
        {
            From = v1;
            To = v2;
            Course = course;
            Date = date;
            IsActive = isActive;
        }
        public virtual string toString()
        {
            return $"[ЦБ РФ] Курс {From} к {To} = {Course} на {Date:yyyy.MM.dd}";
        }

        public bool Matches(string from, string to)
        {
            return From.ToUpper() == from.ToUpper()
                && To.ToUpper() == to.ToUpper();
        }

        public virtual double Convert(double amount, string from, string to)
        {

            if (From.ToUpper() == from.ToUpper() &&
                To.ToUpper() == to.ToUpper())
            {
                return amount / Course;
            }


            if (From.ToUpper() == to.ToUpper() &&
                To.ToUpper() == from.ToUpper())
            {
                return amount * Course;
            }

            throw new Exception($"Курс {From}->{To} не подходит для {from}->{to}");
        }
    }
}