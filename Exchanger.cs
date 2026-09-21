using System;

namespace pis1
{
    class Exchanger : Rate
    {
        public string Name { get; set; }
        public double OwnCourse { get; set; }

        public Exchanger(string v1, string v2, double cbCourse, DateTime date,
                         string name, double ownCourse)
            : base(v1, v2, cbCourse, date, IsActive)
        {
            Name = name;
            OwnCourse = ownCourse;
        }

        public override string toString()
        {
            double diff = OwnCourse - Course;
            string sign = diff >= 0 ? "+" : "";
            return $"[{Name}] {From}->{To}: курс {OwnCourse} " +
                   $"(ЦБ: {Course}, отклонение: {sign}{diff:F2}) на {Date:yyyy.MM.dd}";
        }

        public override double Convert(double amount, string from, string to)
        {
            if (From.ToUpper() == from.ToUpper() &&
                To.ToUpper() == to.ToUpper())
            {
                return amount / OwnCourse;
            }

            if (From.ToUpper() == to.ToUpper() &&
                To.ToUpper() == from.ToUpper())
            {
                return amount * OwnCourse;
            }

            throw new Exception($"Курс {From}->{To} не подходит для {from}->{to}");
        }
    }
}