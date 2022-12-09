namespace Day08
{
    using TreeType = IEnumerable<IEnumerable<Tree>>;

    public record Tree(int Height);

    public record Location(int Row, int Col);

    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public static class Program
    {
        public static Dictionary<Direction, Func<Location, Location>> Advancers = new ()
        {
            {Direction.UP, (Location loc) => {return loc with {Row = loc.Row - 1};}},
            {Direction.DOWN, (Location loc) => {return loc with {Row = loc.Row + 1};}},
            {Direction.LEFT, (Location loc) => {return loc with {Col = loc.Col - 1};}},
            {Direction.RIGHT, (Location loc) => {return loc with {Col = loc.Col + 1};}},
        };

        public static void Main()
        {
            var input = File.ReadAllLines("input.txt");
            var trees = input.Select(row => row.Select(tree => new Tree((int)(tree - '0'))));

            var indexed = trees.SelectMany((row, row_idx) =>
                row.Select((tree, col_idx) =>
                    new { Loc = new Location(row_idx, col_idx)})).ToList();
            
            var visible = indexed.Count(l => trees.IsVisible(l.Loc, out int _));
            var highest = indexed.Select(l => {trees.IsVisible(l.Loc, out int score); return score;}).Max();
        }

        public static bool IsVisible(this TreeType trees, Location loc, out int score)
        {
            var ret = trees.IsVisibleFrom(loc, Direction.UP, out int up);
            ret |= trees.IsVisibleFrom(loc, Direction.DOWN, out int down);
            ret |= trees.IsVisibleFrom(loc, Direction.LEFT, out int left);
            ret |= trees.IsVisibleFrom(loc, Direction.RIGHT, out int right);

            score = up * down * left * right;

            return ret;
        }

        public static bool IsVisibleFrom(this TreeType trees, Location loc, Direction dir, out int score)
        {
            score = 0;
            var current = loc;
            var me = trees.At(loc).Height;
            bool ret = true;

            while (trees.Advance(ref current, dir))
            {
                var other = trees.At(current).Height;
                score ++;

                if (other >= me)
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }

        public static bool Advance(this TreeType tree, ref Location loc, Direction dir)
        {
            if (tree.OnEdge(loc)) return false;
            
            loc = Advancers[dir](loc);
            return true;
        }

        public static Tree At(this TreeType trees, Location loc)
        {
            return trees.ElementAt(loc.Row).ElementAt(loc.Col);
        }

        public static bool OnEdge(this TreeType trees, Location loc)
        {
            var rows = trees.Count();
            var cols = trees.First().Count();

            return (loc.Row == 0 || loc.Row == rows - 1
                || loc.Col == 0 || loc.Col == cols - 1);
        }
    }
}