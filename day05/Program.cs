using System.Text.RegularExpressions;

namespace Day05
{
    public static class Program
    {
        public record Move(int from, int count, int to);

        private static readonly Regex moveRegex = new Regex(@"move (?<count>[\d]+) from (?<from>[\d]+) to (?<to>[\d]+)", RegexOptions.Compiled);

        public static void Main()
        {
            var lines = File.ReadAllLines("input.txt");

            var map = lines.TakeWhile(line => !string.IsNullOrEmpty(line))
                .SkipLast(1)
                .Select(l => l.Chunk(4)
                    .Select(c => new string(c)
                        .Trim().Trim('[').Trim(']')));

            var steps = lines.Skip(map.Count() + 2);

            var stacks = map.ParseMap();
            var moves = steps.Select(s => ParseMove(s));
        
            foreach (var move in moves)
            {
                move.PerformMoveOn(stacks);
            }

            var tops = stacks.Select(s => s.First()).Aggregate((l, r) => l + r);
        }

        public static void PerformMoveOn(this Move move, List<List<string>> stack)
        {
            var moving = stack[move.from].Take(move.count).ToList();
            stack[move.from].RemoveRange(0, move.count);
            stack[move.to] = moving.Concat(stack[move.to]).ToList();
        }

        public static Move ParseMove(string move)
        {
            var match = moveRegex.Match(move);
            return new Move(int.Parse(match.Groups["from"].ToString()) - 1,
                int.Parse(match.Groups["count"].ToString()),
                int.Parse(match.Groups["to"].ToString()) - 1);
        }

        public static List<List<string>> ParseMap(this IEnumerable<IEnumerable<string>> map)
        {
            var ret = new List<List<string>>(map.First().Count());

            for (int column = 0; column < map.First().Count(); column ++)
            {
                var stack = new List<string>();
                ret.Add(stack);

                foreach (var iter in map)
                {
                    var crate = iter.ElementAt(column);

                    if (!string.IsNullOrEmpty(crate))
                    {
                        stack.Add(crate);
                    }
                }
            }

            return ret;
        }
    }
}