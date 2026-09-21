using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace pis1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Rate> cbRates = new List<Rate>();
            List<Exchanger> exchangers = new List<Exchanger>();

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
                    case "1":
                        Rate r = InputCbRate();
                        if (r != null)
                        {
                            cbRates.Add(r);
                            Console.WriteLine("Курс ЦБ добавлен:");
                            Console.WriteLine(r.toString());
                        }
                        break;

                    case "2":
                        ShowRates(cbRates);
                        break;

                    case "3":
                        Exchanger ex = InputExchanger();
                        if (ex != null)
                        {
                            exchangers.Add(ex);
                            Console.WriteLine("Обменник создан:");
                            Console.WriteLine(ex.toString());
                        }
                        break;

                    case "4":
                        DoConversion(cbRates, exchangers);
                        break;

                    case "5":
                        LoadFromFile(cbRates, exchangers);
                        break;

                    case "6":
                        SaveToFile(cbRates, exchangers);
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



        static Rate InputCbRate()
        {
            Console.WriteLine("Введите курс ЦБ в формате: VAL VAL 0,0 2026.01.01 isActive");
            Console.Write("Ввод: ");
            string input = Console.ReadLine();

            try
            {
                string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 5)
                {
                    Console.WriteLine("Неверное количество параметров");
                    return null;
                }

                string v1 = parts[0];
                string v2 = parts[1];
                double c = double.Parse(parts[2].Replace('.', ','));
                DateTime date = DateTime.ParseExact(parts[3], "yyyy.MM.dd",
                                                    CultureInfo.InvariantCulture);
                bool isActive = bool.TryParse(parts[4], out bool result);

                return new Rate(v1, v2, c, date, isActive);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка - {ex.Message}");
                return null;
            }
        }


        static Exchanger InputExchanger()
        {
            Console.WriteLine("Введите данные обменника в формате:");
            Console.WriteLine("  VAL VAL 0,0 2026.01.01 \"Название\" 0,0");
            Console.WriteLine("  (откуда, куда, курс ЦБ для справки, дата, имя в кавычках, СВОЙ курс, активен ли курс)");
            Console.Write("Ввод: ");
            string input = Console.ReadLine();

            try
            {
                int q1 = input.IndexOf('"');
                int q2 = input.IndexOf('"', q1 + 1);
                if (q1 < 0 || q2 < 0)
                {
                    Console.WriteLine("Имя обменника должно быть в кавычках");
                    return null;
                }

                string name = input.Substring(q1 + 1, q2 - q1 - 1);

                string before = input.Substring(0, q1).Trim();
                string after = input.Substring(q2 + 1).Trim();

                string[] parts = before.Split(' ');
                if (parts.Length != 5)
                {
                    Console.WriteLine("Неверное количество параметров до имени");
                    return null;
                }

                string v1 = parts[0];
                string v2 = parts[1];
                double cbCourse = double.Parse(parts[2].Replace('.', ','));
                DateTime date = DateTime.ParseExact(parts[3], "yyyy.MM.dd",
                                                    CultureInfo.InvariantCulture);
                double ownCourse = double.Parse(after.Replace('.', ','));
                string isActive = parts[4];

                return new Exchanger(v1, v2, cbCourse, date, name, ownCourse);
            }
            catch (FormatException)
            {
                Console.WriteLine("Неверный формат данных");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return null;
            }
        }


        static void ShowRates(List<Rate> rates)
        {
            if (rates.Count == 0)
            {
                Console.WriteLine("Список курсов ЦБ пуст");
                return;
            }

            Console.WriteLine("Курсы Центробанка:");
            for (int i = 0; i < rates.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {rates[i].toString()}");
            }
        }

        static void DoConversion(List<Rate> cbRates, List<Exchanger> exchangers)
        {
            if (cbRates.Count == 0 && exchangers.Count == 0)
            {
                Console.WriteLine("Нет ни курсов. Сначала создайте курс (цб или обменника)");
                return;
            }

            Console.WriteLine("Где будет происходить конвертация");
            Console.WriteLine("  0 - по курсу Центробанка");
            for (int i = 0; i < exchangers.Count; i++)
            {
                Console.WriteLine($"  {i + 1} - {exchangers[i].Name}");
            }

            Console.Write("Ваш выбор: ");
            if (!int.TryParse(Console.ReadLine(), out int idx) ||
                idx < 0 || idx > exchangers.Count)
            {
                Console.WriteLine("Неверный выбор");
                return;
            }

            Console.Write("Валюта, из которой конвертируем: ");
            string from = Console.ReadLine();

            Console.Write("Валюта, в которую конвертируем: ");
            string to = Console.ReadLine();

            Rate converter;
            string who;

            if (idx == 0)
            {
                Rate found = null;
                foreach (var r in cbRates)
                {
                    if (r.Matches(from, to) || r.Matches(to, from))
                    {
                        found = r;
                        break;
                    }
                }
                if (found == null)
                {
                    Console.WriteLine($"У ЦБ нет курса для пары {from} <-> {to}");
                    return;
                }
                converter = found;
                who = "Центробанк";
            }
            else
            {
                converter = exchangers[idx - 1];
                who = exchangers[idx - 1].Name;

                if (!converter.Matches(from, to) && !converter.Matches(to, from))
                {
                    Console.WriteLine($"Обменник работает только с парой {converter.From} <-> {converter.To}");
                    return;
                }
            }

            Console.Write("Сумма: ");
            if (!double.TryParse(Console.ReadLine().Replace('.', ','), out double amount)
                || amount <= 0)
            {
                Console.WriteLine("Неверная сумма");
                return;
            }

            double result = converter.Convert(amount, from, to);

            Console.WriteLine();
            Console.WriteLine("---------- Результат");
            Console.WriteLine($"Обменник:    {who}");
            Console.WriteLine($"Валюты:   {from} -> {to}");

            if (converter is Exchanger ex)
            {
                Console.WriteLine($"Курс обменника: {ex.OwnCourse}");
                Console.WriteLine($"Курс ЦБ:        {ex.Course}");
            }
            else
            {
                Console.WriteLine($"Курс ЦБ: {converter.Course}");
            }

            Console.WriteLine($"Сумма:  {amount} {from}");
            Console.WriteLine($"Итог:   {result:F2} {to}");


        }
        static void LoadFromFile(List<Rate> cbRates, List<Exchanger> exchangers)
        {
            Console.Write("Путь к файлу: ");
            string path = Console.ReadLine();
            string[] lines;
            try
            {
                lines = File.ReadAllLines(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка - {ex.Message}");
                return;
            }


            int lineNum = 0;

            foreach (string raw in lines)
            {
                lineNum++;
                string line = raw;

                if (line.Length == 0 )
                    continue;

                try
                {
                    string[] parts = line.Split(' ');
                    if (parts.Length != 5)
                        throw new FormatException("Должно быть 5 полей: чей_курс валюта_из валюта_куда курс дата");

                    string owner = parts[0];
                    string from = parts[1];
                    string to = parts[2];
                    double course = double.Parse(parts[3].Replace('.', ','));
                    DateTime date = DateTime.ParseExact(parts[4], "yyyy.MM.dd", CultureInfo.InvariantCulture);
                    bool isActive = bool.TryParse(parts[5], out bool result);

                    if (owner.Equals("ЦБ", StringComparison.OrdinalIgnoreCase))
                    {
                        cbRates.Add(new Rate(from, to, course, date, isActive));
                    }
                    else
                    {
                        exchangers.Add(new Exchanger(from, to, 0, date, owner, course));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Строка {lineNum}: ошибка - {ex.Message}");
                }
            }
        }

        static void SaveToFile(List<Rate> cbRates, List<Exchanger> exchangers)
        {
            if (cbRates.Count == 0 && exchangers.Count == 0)
            {
                Console.WriteLine("Нечего сохранять");
                return;
            }

            Console.Write("Путь к файлу для сохранения: ");
            string path = Console.ReadLine();

            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Путь не может быть пустым");
                return;
            }

            var lines = new List<string>();

            lines.Add("# чей_курс валюта_из валюта_куда курс дата");
            lines.Add("");

            foreach (var r in cbRates)
            {
                string course = r.Course.ToString(CultureInfo.InvariantCulture);
                string date = r.Date.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture);
                lines.Add($"ЦБ {r.From} {r.To} {course} {date}");
            }

            foreach (var ex in exchangers)
            {
                string course = ex.OwnCourse.ToString(CultureInfo.InvariantCulture);
                string date = ex.Date.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture);
                lines.Add($"\"{ex.Name}\" {ex.From} {ex.To} {course} {date}");
            }

            try
            {
                File.WriteAllLines(path, lines, Encoding.UTF8);
                Console.WriteLine($"Сохранено: курсов ЦБ — {cbRates.Count}, обменников — {exchangers.Count}");
                Console.WriteLine($"Файл: {Path.GetFullPath(path)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось сохранить файл: {ex.Message}");
            }
        }
    }
}