using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace pis1
{

    public class RateManager
    {
        private readonly List<Rate> cbRates = new List<Rate>();
        private readonly List<Exchanger> exchangers = new List<Exchanger>();

        public int CbCount => cbRates.Count;
        public int ExchangerCount => exchangers.Count;


        public void AddCbRate(Rate r)
        {
            if (r == null) throw new ArgumentNullException(nameof(r));
            cbRates.Add(r);
        }

        public void AddExchanger(Exchanger e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            exchangers.Add(e);
        }


        public IReadOnlyList<Rate> GetCbRates() => cbRates;
        public IReadOnlyList<Exchanger> GetExchangers() => exchangers;
        public bool IsEmpty() => cbRates.Count == 0 && exchangers.Count == 0;

        public Rate FindCbRate(string from, string to)
        {
            foreach (var r in cbRates)
            {
                if (!r.IsActive) continue;
                if (r.Matches(from, to) || r.Matches(to, from))
                    return r;
            }
            return null;
        }

        public Exchanger GetExchangerAt(int index)
        {
            if (index < 0 || index >= exchangers.Count) return null;
            return exchangers[index];
        }

        public double Convert(Rate converter, double amount, string from, string to)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            if (!converter.IsActive)
                throw new Exception($"Курс {converter.From}->{converter.To} неактивен");

            if (!converter.Matches(from, to) && !converter.Matches(to, from))
                throw new Exception(
                    $"Объект работает только с парой {converter.From} <-> {converter.To}");

            return converter.Convert(amount, from, to);
        }


        public (int cb, int ex, List<string> errors) LoadFromFile(string path)
        {
            var errors = new List<string>();
            int loadedCb = 0;
            int loadedEx = 0;

            if (!File.Exists(path))
                throw new FileNotFoundException("Файл не найден", path);

            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            int lineNum = 0;

            foreach (string raw in lines)
            {
                lineNum++;
                string line = raw.Trim();

                if (line.Length == 0 || line.StartsWith("#"))
                    continue;

                try
                {
                    string owner;
                    string rest;

                    if (line.StartsWith("\""))
                    {
                        int q2 = line.IndexOf('"', 1);
                        if (q2 < 0) throw new FormatException("не закрыта кавычка в имени");
                        owner = line.Substring(1, q2 - 1);
                        rest = line.Substring(q2 + 1).Trim();
                    }
                    else
                    {
                        int sp = line.IndexOf(' ');
                        if (sp < 0) throw new FormatException("не хватает полей");
                        owner = line.Substring(0, sp);
                        rest = line.Substring(sp + 1).Trim();
                    }

                    string[] parts = rest.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 4)
                        throw new FormatException("должно быть 4 поля после имени");

                    string from = parts[0];
                    string to = parts[1];
                    double course = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    DateTime date = DateTime.ParseExact(parts[3], "yyyy.MM.dd",
                                                        CultureInfo.InvariantCulture);

                    if (owner.Equals("ЦБ", StringComparison.OrdinalIgnoreCase) ||
                        owner.Equals("ЦБРФ", StringComparison.OrdinalIgnoreCase) ||
                        owner.Equals("CB", StringComparison.OrdinalIgnoreCase))
                    {
                        cbRates.Add(new Rate(from, to, course, date));
                        loadedCb++;
                    }
                    else
                    {
                        exchangers.Add(new Exchanger(from, to, date, owner, course));
                        loadedEx++;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Строка {lineNum}: ошибка — {ex.Message}");
                }
            }

            return (loadedCb, loadedEx, errors);
        }

        public void SaveToFile(string path)
        {
            var lines = new List<string>();

            lines.Add("# чей_курс валюта_из валюта_куда курс дата");
            lines.Add("# строки, начинающиеся с #, при чтении пропускаются");
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

            File.WriteAllLines(path, lines, Encoding.UTF8);
        }
    }
}