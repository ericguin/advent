namespace Day09
{
    public record Position(int x, int y);
    public record Board(Position head, Position tail);
    public record Move(Direction dir, int dist);

    public enum Direction { U, D, L, R }

    public static class Program
    {
        public static readonly Dictionary<Direction, Func<Position, Position>> Move = new ()
        {
            {Direction.U, (p) => p with {y = p.y - 1}},
            {Direction.D, (p) => p with {y = p.y + 1}},
            {Direction.L, (p) => p with {x = p.x - 1}},
            {Direction.R, (p) => p with {x = p.x + 1}},
        };

        public static void Main()
        {
            var input = File.ReadAllLines("input.txt");
            var moves = input.Select(l => l.ToMove());

            var board = new Board(new (0, 0), new (0, 0));
            var positions = moves.SelectMany(m => m.ApplyMove(ref board));
            var result = positions.Distinct().Count();
        }

        public static IEnumerable<Position> ApplyMove(this Move m, ref Board b)
        {
            List<Position> ret = new ();

            foreach (var _ in Enumerable.Range(0, m.dist))
            {
                b = b.MoveHead(m.dir);
                ret.Add(b.tail);
            }

            return ret;
        }

        public static Board MoveHead(this Board b, Direction dir)
        {
            var head = Move[dir](b.head);
            var tail = b.tail;
            if (!b.tail.IsAdjacentTo(head))
            {
                tail = b.head;
            }
            return new Board(head, tail);
        }

        public static bool IsAdjacentTo(this Position me, Position them)
        {
            return me.x.IsWithin(them.x) && me.y.IsWithin(them.y);
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
    }
}