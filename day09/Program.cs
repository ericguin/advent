namespace Day09
{
    public record Position(int x, int y);
    public record Board(IEnumerable<Position> rope);
    public record Move(Direction dir, int dist);

    public class BoardPrinter
    {
        int max_x = 0;
        int min_x = 0;
        int max_y = 0;
        int min_y = 0;

        Dictionary<Position, char> map = new();

        public BoardPrinter()
        {

        }

        public void FeedPosition(Position p, char indicator = 'x')
        {
            if (!map.ContainsKey(p))
            {
                map[p] = indicator;
                max_x = Math.Max(p.x, max_x);
                min_x = Math.Min(p.x, min_x);
                max_y = Math.Max(p.y, max_y);
                min_y = Math.Min(p.y, min_y);
            }
        }

        public void Print()
        {
            for (int y = max_y; y >= min_y; y --)
            {
                for (int x = min_x; x <= max_x; x ++)
                {
                    Position p = new Position(x, y);

                    if (map.TryGetValue(p, out char ind))
                    {
                        Console.Write(ind);
                    }
                    else
                    {
                        Console.Write('.');
                    }
                }
                Console.WriteLine();
            }
        }
    }

    public enum Direction { U, D, L, R }

    public static class Program
    {
        public static readonly Dictionary<Direction, Func<Position, Position>> Move = new ()
        {
            {Direction.U, (p) => p with {y = p.y + 1}},
            {Direction.D, (p) => p with {y = p.y - 1}},
            {Direction.L, (p) => p with {x = p.x - 1}},
            {Direction.R, (p) => p with {x = p.x + 1}},
        };

        public static void Main()
        {
            var input = File.ReadAllLines("input.txt");
            var moves = input.Select(l => l.ToMove());

            var board = new Board(Enumerable.Range(1, 10).Select(_ => new Position(0, 0)));
            var positions = moves.SelectMany(m => m.ApplyMove(ref board)).ToList();
            var result = positions.Distinct();
            var count = result.Count();
        }

        public static IEnumerable<Position> ApplyMove(this Move m, ref Board b)
        {
            List<Position> ret = new ();

            foreach (var _ in Enumerable.Range(0, m.dist))
            {
                b = b.MoveHead(m.dir);
                ret.Add(b.rope.Last());
            }

            return ret;
        }

        public static Board MoveHead(this Board b, Direction dir)
        {
            var rope = new List<Position>();
            var head = Move[dir](b.rope.First());
            rope.Add(head);
            
            foreach (var step in b.rope.Skip(1).Select((k, i) => new {Knot = k, Index = i}))
            {
                var p = step.Knot;
                p = p.MoveTo(head, dir);
                rope.Add(p);
                head = p;
            }

            var ret = new Board(rope);
            return ret;
        }

        public static void Print(this Board b)
        {
            BoardPrinter bp = new BoardPrinter();
            bp.FeedPosition(new Position(0, 0), 'S');

            foreach (var p in b.rope.Select((k, i) => new {Knot = k, Index = i}))
            {
                bp.FeedPosition(p.Knot, p.Index.ToString()[0]);
            }
        
            bp.Print();
        }

        public static bool IsAdjacentTo(this Position me, Position them)
        {
            return me.x.IsWithin(them.x) && me.y.IsWithin(them.y);
        }

        public static Position MoveTo(this Position me, Position them, Direction dir)
        {
            if (!me.IsAdjacentTo(them))
            {
                return me with {x = me.x.Towards(them.x), y = me.y.Towards(them.y)};
            }

            return me;
        }

        public static Move ToMove(this string move)
        {
            var bits = move.Split(' ');
            var dir = Enum.Parse<Direction>(bits[0]);
            var dist = int.Parse(bits[1]);
            return new Move(dir, dist);
        }

        public static bool IsWithin(this int me, int them, int range = 1)
        {
            return Math.Abs(me - them) <= range;
        }

        public static int Towards(this int me, int them)
        {
            if (me == them) return me;
            return (them > me) ? me + 1 : me - 1;
        }
    }
}