namespace Day3
{
    record Rucksack(List<Compartment> Compartments);
    record Compartment(string Contents);

    public static class Program
    {
        public static void Main()
        {
            var lines = File.ReadAllLines("input.txt");
            var rucks = lines.Select(_ => _.Divide());
            var common = rucks.Select(_ => _.FindCommon());
            var score = common.ScoreCommon();
            Console.WriteLine($"The score is {score}");

            var groups = rucks.Chunk(3);
            var group_common = groups.Select(_ => _.FindCommon());
            var group_score = group_common.ScoreCommon();
        }

        static List<char> FindCommon(this Rucksack[] group)
        {
            return group.Select(_ => _.Compartments.SelectMany(_ => _.Contents))
                .Aggregate((l, r) => l.Intersect(r).ToList()).ToList();
        }

        static Rucksack Divide(this string ruck)
        {
            var one = ruck.Substring(0, ruck.Length / 2);
            var two = ruck.Substring(ruck.Length / 2);

            return new(new(){new(one), new(two)});
        }

        static List<char> FindCommon(this Rucksack ruck)
        {
            return ruck.Compartments[0].Contents.Intersect(ruck.Compartments[1].Contents).ToList();
        }

        static int ScoreCommon(this IEnumerable<List<Char>> common)
        {
            int ret = 0;

            foreach (var union in common)
            {
                foreach (var c in union)
                {
                    if (char.IsLower(c))
                    {
                        ret += c - 'a' + 1;
                    }
                    else
                    {
                        ret += c - 'A' + 27;
                    }
                }
            }

            return ret;
        }
    }
}
