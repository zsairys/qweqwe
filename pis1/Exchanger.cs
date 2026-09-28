using System;

namespace pis1
{
    public class Exchanger : Rate
    {
        public string Name { get; set; }
        public double OwnCourse { get; set; }

        public Exchanger(string v1, string v2, DateTime date,
                         string name, double ownCourse,
                         bool isActive = true)
            : base(v1, v2, ownCourse, date, isActive)
        {
            Name = name;
            OwnCourse = ownCourse;
        }

        public override string toString()
        {
            string status = IsActive ? "" : " [НЕАКТИВЕН]";
            return $"[{Name}] {From}->{To}: курс {OwnCourse} на {Date:yyyy.MM.dd}{status}";
        }

        public override double Convert(double amount, string from, string to)
        {
            if (From.ToUpper() == from.ToUpper() &&
                To.ToUpper() == to.ToUpper())
                return amount / OwnCourse;

            if (From.ToUpper() == to.ToUpper() &&
                To.ToUpper() == from.ToUpper())
                return amount * OwnCourse;

            throw new Exception($"Курс {From}->{To} не подходит для {from}->{to}");
        }
    }
}
