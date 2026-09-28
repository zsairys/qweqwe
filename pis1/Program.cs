using System;
using System.Collections.Generic;
using System.Globalization;

namespace pis1
{
    class Program
    {
        static readonly RateManager manager = new RateManager();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Ввести курс Центробанка");
                Console.WriteLine("2. Показать курсы Центробанка");
                Console.WriteLine("3. Создать обменник");
                Console.WriteLine("4. Конвертировать валюту");
                Console.WriteLine("5. Загрузить курсы из файла");
                Console.WriteLine("6. Сохранить курсы в файл");
                Console.WriteLine("0. Выйти");
                Console.WriteLine();

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": HandleAddCbRate(); break;
                    case "2": HandleShowRates(); break;
                    case "3": HandleAddExchanger(); break;
                    case "4": HandleConvert(); break;
                    case "5": HandleLoad(); break;
                    case "6": HandleSave(); break;
                    case "0": Console.WriteLine("Выход из программы"); return;
                    default: Console.WriteLine("Неверный выбор"); break;
                }
            }
        }

        static void HandleAddCbRate()
        {
            Console.WriteLine("Введите курс ЦБ: VAL VAL 0,0 2026.01.01");
            Console.Write("Ввод: ");
            string input = Console.ReadLine();

            try
            {
                string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 4)
                {
                    Console.WriteLine("Неверное количество параметров");
                    return;
                }

                string v1 = parts[0];
                string v2 = parts[1];
                double c = double.Parse(parts[2].Replace('.', ','));
                DateTime date = DateTime.ParseExact(parts[3], "yyyy.MM.dd",
                                                    CultureInfo.InvariantCulture);

                var r = new Rate(v1, v2, c, date);
                manager.AddCbRate(r);

                Console.WriteLine("Курс ЦБ добавлен:");
                Console.WriteLine(r.toString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Неверный формат данных!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка - {ex.Message}");
            }
        }

        static void HandleShowRates()
        {
            var rates = manager.GetCbRates();
            if (rates.Count == 0)
            {
                Console.WriteLine("Список курсов ЦБ пуст");
                return;
            }

            Console.WriteLine("Курсы Центробанка:");
            for (int i = 0; i < rates.Count; i++)
                Console.WriteLine($"{i + 1}. {rates[i].toString()}");
        }

        static void HandleAddExchanger()
        {
            Console.WriteLine("Введите обменник: VAL VAL 2026.01.01 \"Название\" 0,0");
            Console.Write("Ввод: ");
            string input = Console.ReadLine();

            try
            {
                int q1 = input.IndexOf('"');
                int q2 = input.IndexOf('"', q1 + 1);
                if (q1 < 0 || q2 < 0)
                {
                    Console.WriteLine("Имя обменника должно быть в кавычках");
                    return;
                }

                string name = input.Substring(q1 + 1, q2 - q1 - 1);

                string before = input.Substring(0, q1).Trim();
                string after = input.Substring(q2 + 1).Trim();

                string[] parts = before.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3)
                {
                    Console.WriteLine("Неверное количество параметров до имени");
                    return;
                }

                string v1 = parts[0];
                string v2 = parts[1];
                DateTime date = DateTime.ParseExact(parts[2], "yyyy.MM.dd",
                                                    CultureInfo.InvariantCulture);
                double ownCourse = double.Parse(after.Replace('.', ','));

                var ex = new Exchanger(v1, v2, date, name, ownCourse);
                manager.AddExchanger(ex);

                Console.WriteLine("Обменник создан:");
                Console.WriteLine(ex.toString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Неверный формат данных!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка - {ex.Message}");
            }
        }


        static void HandleConvert()
        {
            if (manager.IsEmpty())
            {
                Console.WriteLine("Нет ни курсов ЦБ, ни обменников.");
                return;
            }

            Console.WriteLine("У кого конвертируем?");
            Console.WriteLine("  0 - по курсу Центробанка");
            for (int i = 0; i < manager.ExchangerCount; i++)
                Console.WriteLine($"  {i + 1} - {manager.GetExchangers()[i].Name}");

            Console.Write("Ваш выбор: ");
            if (!int.TryParse(Console.ReadLine(), out int idx) ||
                idx < 0 || idx > manager.ExchangerCount)
            {
                Console.WriteLine("Неверный выбор");
                return;
            }

            Console.Write("Валюта, из которой конвертируем: ");
            string from = Console.ReadLine().Trim();

            Console.Write("Валюта, в которую конвертируем: ");
            string to = Console.ReadLine().Trim();

            Rate converter;
            string who;

            if (idx == 0)
            {
                converter = manager.FindCbRate(from, to);
                if (converter == null)
                {
                    Console.WriteLine($"У ЦБ нет курса для пары {from} <-> {to}");
                    return;
                }
                who = "Центробанк";
            }
            else
            {
                converter = manager.GetExchangerAt(idx - 1);
                who = ((Exchanger)converter).Name;
            }

            Console.Write("Сумма: ");
            if (!double.TryParse(Console.ReadLine().Replace('.', ','), out double amount)
                || amount <= 0)
            {
                Console.WriteLine("Неверная сумма");
                return;
            }

            try
            {
                double result = manager.Convert(converter, amount, from, to);

                Console.WriteLine();
                Console.WriteLine("------ Результат ------");
                Console.WriteLine($"Кто:    {who}");
                Console.WriteLine($"Пара:   {from} -> {to}");

                if (converter is Exchanger ex)
                    Console.WriteLine($"Курс обменника: {ex.OwnCourse}");
                else
                    Console.WriteLine($"Курс ЦБ: {converter.Course}");

                Console.WriteLine($"Сумма:  {amount} {from}");
                Console.WriteLine($"Итог:   {result:F2} {to}");
                Console.WriteLine("-----------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void HandleLoad()
        {
            Console.Write("Путь к файлу: ");
            string path = Console.ReadLine().Trim();

            try
            {
                var (cb, ex, errors) = manager.LoadFromFile(path);

                foreach (var err in errors)
                    Console.WriteLine(err);

                Console.WriteLine($"Загружено: курсов ЦБ — {cb}, обменников — {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось загрузить: {ex.Message}");
            }
        }

        static void HandleSave()
        {
            if (manager.IsEmpty())
            {
                Console.WriteLine("Нечего сохранять");
                return;
            }

            Console.Write("Путь к файлу для сохранения: ");
            string path = Console.ReadLine().Trim();

            try
            {
                manager.SaveToFile(path);
                Console.WriteLine($"Сохранено: ЦБ — {manager.CbCount}, обменников — {manager.ExchangerCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось сохранить: {ex.Message}");
            }
        }
    }
}