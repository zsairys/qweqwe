using System;
using System.Collections.Generic;

namespace pis1
{


    class Program
    {
        static void Main(string[] args)
        {
            List<Value> list = new List<Value>();
            Console.WriteLine("Ввести название валют (2 строки), курс (дробное число), дата. для выхода введите пустую строку");

            while (true)
            {
                string s = Console.ReadLine();
                if (s == "") { break; }

                string[] prop = s.Split(',');
                list.Add(new Value
                {
                    Name = prop[0],
                    NameRu = prop[1],
                    Course = Convert.ToDouble(prop[2]),
                    Date = Convert.ToDateTime(prop[3])
                });
            }
        }
    }
}