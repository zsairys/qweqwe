using System;
using System.Collections.Generic;
using System.Globalization;

namespace pis1
{
   
    class Program
    {
        static void Main(string[] args)
        {
            List<Rate> rates = new List<Rate>();

            while (true)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Ввести курс");
                Console.WriteLine("2. Показать все курсы");
                Console.WriteLine("0. Выйти из программы");
                Console.WriteLine();

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Rate newRate = InputRate();
                        if (newRate != null)
                        {
                            rates.Add(newRate);
                            Console.WriteLine("Курс добавлен");
                            Console.WriteLine(newRate.Conv());
                        }
                        break;

                    case "2":
                        ShowRates(rates);
                        break;

                    case "0":
                        Console.WriteLine("Выход из программы");
                        return;

                    default:
                        Console.WriteLine("Неверный выбор");
                        break;
                }
            }
        }


        static Rate InputRate()
        {
            Console.WriteLine("Введите данные в формате: VAL VAL 0,0 2026.01.01");
            Console.Write("Ввод: ");
            string input = Console.ReadLine();

            try
            {
                string[] parts = input.Split(' ');

                if (parts.Length != 4)
                {
                    Console.WriteLine("Неверное количество параметров");
                    return null;
                }

                string v1 = parts[0];
                string v2 = parts[1];
                double c = double.Parse(parts[2]);
                DateTime date = DateTime.Parse(parts[3]);

                return new Rate(v1, v2, c, date);
            }
            catch (FormatException)
            {
                Console.WriteLine("Неверный формат данных!");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка - {ex.Message}");
                return null;
            }
        }

        static void ShowRates(List<Rate> rates)
        {
            if (rates.Count == 0)
            {
                Console.WriteLine("Список курсов пуст");
                return;
            }

            Console.WriteLine("Все курсы");
            for (int i = 0; i < rates.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {rates[i].Conv()}");
            }
        }
    }
}