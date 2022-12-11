using System.Text.RegularExpressions;
using System.Numerics;

namespace Day11
{
    public enum Operation
    {
        MULT,
        ADD,
        SQUARE
    }

    public record Monkey(ulong id, List<BigInteger> items, Operation op, ulong op_mag, ulong test, ulong tmonk, ulong fmonk);
    public record TargetMonkeys(Monkey tmonk, Monkey fmonk);

    public static class Program
    {
        public static readonly Regex gigareg = new Regex(@"Monkey (?<monkey>[^:]+):\s+Starting items: (?<starting>[^\n]+)\s+Operation: new = old (?<op>[^\n]+)\s+Test: divisible by (?<div>[\d]+)\s+If true: throw to monkey (?<tmonk>[\d]+)\s+If false: throw to monkey (?<fmonk>[\d]+)", RegexOptions.Compiled);

        public static readonly Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>> Operations = new ()
        {
            {Operation.MULT, (x, y) => x * y},
            {Operation.ADD, (x, y) => x + y},
            {Operation.SQUARE, (x, _) => x * x},
        };

        public static readonly List<ulong> poulongs = new ()
        {
            1, 20, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000
        };

        public static void Main()
        {
            var input = File.ReadAllText("example.txt");
            var monkeys = gigareg.Matches(input).Select(m => (m.Parse(), (ulong)0)).ToList();

            var count = 10000;
            //var count = 20;

            foreach (var round in Enumerable.Range(1, count))
            {
                monkeys = monkeys.RunRound();

                if (poulongs.Contains((ulong)round))
                {
                    monkeys.ForEach(m =>
                    {
                        Console.WriteLine($"After round {round} monkey {m.Item1.id} has inspected {m.Item2} items");
                    });

                    Console.WriteLine();
                }
            }

            var ordered = monkeys.OrderBy(m => m.Item2).Reverse().Take(2).Select(m => m.Item2).Aggregate((x, y) => x * y);
        }

        public static List<(Monkey, ulong)> RunRound(this List<(Monkey, ulong)> monkeys)
        {
            var ret = new List<(Monkey, ulong)>();

            foreach (var m in monkeys)
            {
                var moves = m.Item1.items.Select(i => new { o = i, n = m.Item1.InspectItem(i, false)}).ToList()
                    .Select(i => new {Item = i, To = m.Item1.PassItem(i.n)}).ToList();
                ulong inspections = m.Item2 + (ulong)moves.Count();

                foreach (var move in moves)
                {
                    m.Item1.items.Remove(move.Item.o);
                    monkeys[(int)move.To].Item1.items.Add(move.Item.n);
                }

                ret.Add((m.Item1, inspections));
            }
            
            return ret;
        }

        public static Monkey Parse(this Match m)
        {
            var op = m.Groups["op"].Value;
            var mag = op.Substring(2);
            var items = new List<BigInteger>();
            var parsed = m.Groups["starting"].Value.Split(',').Select(v => BigInteger.Parse(v));
            items.AddRange(parsed);
            return new Monkey(ulong.Parse(m.Groups["monkey"].Value),
                items,
                mag == "old" ? Operation.SQUARE : (op[0] == '*' ? Operation.MULT : Operation.ADD),
                mag == "old" ? 0 : ulong.Parse(mag),
                ulong.Parse(m.Groups["div"].Value),
                ulong.Parse(m.Groups["tmonk"].Value),
                ulong.Parse(m.Groups["fmonk"].Value));
        }

        public static BigInteger InspectItem(this Monkey m, BigInteger item, bool divide = true)
        {
            var inspect = Operations[m.op](item, m.op_mag);
            return divide ? inspect / 3 : inspect;
        }

        public static ulong PassItem(this Monkey m, BigInteger item)
        {
            if ((item % m.test) == 0)
            {
                return m.tmonk;
            }
            else
            {
                return m.fmonk;
            }
        }
    }
}