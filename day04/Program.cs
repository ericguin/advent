namespace MyApp // Note: actual namespace depends on the project name.
{
    public static class Program
    {
        public record Assignment(int start, int end);

        static void Main(string[] args)
        {
            var test1 = new Range(1, 4);
            var test2 = new Range(2, 5);
            var lines = File.ReadAllLines("input.txt");
            var groups = lines.Select(_ => _.ParseGroup());
            var contain = groups.Count(_ => CompleteOverlap(_[0], _[1]));
            var overlap = groups.Count(_ => AnyOverlap(_[0], _[1]));
        }

        public static bool Contains(this Assignment a, Assignment b)
        {
            return a.start <= b.start && a.end >= b.end;
        }

        public static bool Overlap(this Assignment a, Assignment b)
        {
            return (b.start >= a.start && b.start <= a.end) || (b.end >= a.start && b.end <= a.end);
        }

        public static bool CompleteOverlap(Assignment one, Assignment two)
        {
            return one.Contains(two) || two.Contains(one);
        }

        public static bool AnyOverlap(Assignment one, Assignment two)
        {
            return one.Overlap(two) || two.Overlap(one);
        }

        public static List<Assignment> ParseGroup(this string line)
        {
            var groups = line.Split(",");
            return groups.Select(_ => _.ParseAssignment()).ToList();
        }

        public static Assignment ParseAssignment(this string assigment)
        {
            var range = assigment.Split("-");
            return new Assignment(int.Parse(range[0]), int.Parse(range[1]));
        }
    }
}