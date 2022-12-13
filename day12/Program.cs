using System.Diagnostics.CodeAnalysis;

namespace Day12
{
    public static class Program
    {
        public record Position(int x, int y);
        
        public enum Direction { U, D, L, R }

        public static readonly Dictionary<Direction, Func<Position, Position>> Moves = new ()
        {
            {Direction.U, (p) => p with {y = p.y - 1}},
            {Direction.D, (p) => p with {y = p.y + 1}},
            {Direction.L, (p) => p with {x = p.x - 1}},
            {Direction.R, (p) => p with {x = p.x + 1}},
        };

        public class PathCompare : IEqualityComparer<List<Position>>
        {
            public bool Equals(List<Position>? x, List<Position>? y)
            {
                return x?.Last() == y?.Last();
            }

            public int GetHashCode([DisallowNull] List<Position> obj)
            {
                return 0;
            }
        }

        public static void Main()
        {
            var input = File.ReadAllLines("input.txt");
            var end = new Position(0, 0);
            var map = input.Select((l, y) => l.Select((c, x) =>
            {
                int ret = c - 'a';
                if (c == 'S')
                {
                    ret = -1;
                }
                if (c == 'E')
                {
                    end = new Position(x, y);
                    ret = 'z' - 'a' + 1;
                }
                return ret;
            } ).ToArray()).ToArray();

            object answerLock = new object();
            List<int> answers = new List<int>();

            Parallel.ForEach(map.SelectMany((r, y) => r.Select((i, x) => new {Height = i, Pos = new Position(x, y)}).Where(i => i.Height <= 0)), (start) =>
            {
                List<List<Position>> InFlight = new List<List<Position>>()
                {
                    new (){start.Pos}
                };

                bool found = false;
                // Let's try to avoid recursion?
                while (!found)
                {
                    List<List<Position>> iter = new List<List<Position>>();
                    bool moves = false;

                    foreach (var path in InFlight)
                    {
                        // Grab the valid moves from here
                        var valid = map.ValidMovesFrom(path.Last());
                        valid.RemoveAll(v => path.Contains(v));

                        if (valid.Any()) moves = true;

                        var paths = valid.Select(v => path.AsEnumerable().Append(v).ToList()).ToList();

                        if (valid.Any(v => v == end))
                        {
                            found = true;
                        }

                        iter.AddRange(paths);
                    }

                    iter = iter.Distinct(new PathCompare()).ToList();

                    InFlight = iter;

                    if (!moves) break;
                }

                if (found)
                {
                    // -1 for starting position, doesn't count as a move
                    int answer = InFlight.First().Count() - 1;
                    Console.WriteLine($"Answer is {answer}");
                    lock(answerLock)
                    {
                        answers.Add(answer);
                    }
                }
            });
        
            var answer2 = answers.OrderBy(a => a).First();
        }

        public static List<Position> ValidMovesFrom(this int[][] map, Position p)
        {
            var vals = Enum.GetValues<Direction>().ToList();
            return Enum.GetValues<Direction>().ToList()
                .Select(d => map.TakeInDirection(p, d)).ToList()
                .Where(n => n.Item2)
                .Where(n => map.CanMove(p, n.Item1))
                .Select(n => n.Item1)
                .ToList();
        }

        public static int At(this int[][] map, Position p)
        {
            return map[p.y][p.x];
        }

        public static bool CanMove(this int[][] map, Position from, Position to)
        {
            int fh = map.At(from);
            int th = map.At(to);

            return (th <= fh) || (th - fh == 1);
        }

        public static (Position, bool) TakeInDirection(this int[][] map, Position p, Direction d)
        {
            var moved = Moves[d](p);
            return (moved, map.ValidLocation(moved));
        }

        public static bool ValidLocation(this int[][] map, Position p)
        {
            return p.x >= 0 && p.x < map[0].Length && p.y >= 0 && p.y < map.Length;
        }
    }
}