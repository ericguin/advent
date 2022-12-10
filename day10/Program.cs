namespace Day10
{
    public enum Operation
    {
        noop,
        addx
    }

    public record Registers(int X);

    public record Line(Operation op, IEnumerable<string> args);

    public static class Program
    {
        public static readonly Dictionary<Operation, Func<Registers, IEnumerable<string>, IEnumerable<Registers>>> ALU = new ()
        {
            {Operation.noop, doNoop},
            {Operation.addx, doAddx}
        };

        public static List<int> breakpoints = new ()
        {
            20, 60, 100, 140, 180, 220
        };

        public static void Main()
        {
            var input = File.ReadAllLines("input.txt");
            var lines = input.Select(l => l.Parse());
            var reg = new Registers(1);
            var trace = new List<Registers>() { reg };
            trace = trace.Concat(lines.SelectMany(l => {
                var ret = ALU[l.op](reg, l.args).ToList();
                reg = ret.Last();
                return ret;
            }).ToList()).ToList();
            var values = breakpoints.Select(b => new { Point = b, Value = trace.ElementAt(b - 1).X}).ToList();
            var result = values.Sum(v => v.Point * v.Value);
            
            var preload = trace.Take(240).Select((r, i) => new {reg = r, x = i}).ToList();
            var pixels = preload.Select(p => p.reg.Contains(p.x)).Select(p => p ? '#' : '.').ToList();
            var screen = pixels.Chunk(40).ToList();
            var rendered = screen.Select(l => new string(l)).ToList();

            foreach (string l in rendered)
            {
                Console.WriteLine(l);
            }
        }

        public static bool Contains(this Registers reg, int pixel)
        {
            return Math.Abs((pixel % 40) - reg.X) <= 1;
        }

        public static Line Parse(this string line)
        {
            var bits = line.Split(' ');
            return new Line(Enum.Parse<Operation>(bits[0]), bits.Skip(1).ToList());
        }

        public static IEnumerable<Registers> doNoop(Registers current, IEnumerable<string> _)
        {
            yield return current;
        }
        
        public static IEnumerable<Registers> doAddx(Registers current, IEnumerable<string> args)
        {
            yield return current;
            yield return current with {X = current.X + int.Parse(args.First())};
        }
    }
}