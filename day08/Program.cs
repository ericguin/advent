namespace Day08
{
    public class Tree
    {
        public readonly int Height;
        public bool Visible = false;

        public Tree(int height)
        {
            Height = height;
        }
    }

    public static class Program
    {
        public static void Main()
        {
            var input = File.ReadAllLines("input.txt");
            var trees = input.Select(row => row.Select(tree => new Tree((int)(tree - '0'))).ToList()).ToList();

            // Trees on the edge are visible
            trees.First().ForEach(tree => tree.Visible = true);
            trees.Last().ForEach(tree => tree.Visible = true);
            trees.ForEach(row => row.First().Visible = true);
            trees.ForEach(row => row.Last().Visible = true);


            var result = trees.Sum(row => row.Count(tree => tree.Visible));

            // Top down
            for (int col = 1; col < trees.First().Count() - 1; col ++)
            {
                int max = trees.First().ElementAt(col).Height;

                foreach (var row in trees.Skip(1).SkipLast(1))
                {
                    var tree = row.ElementAt(col);

                    if (tree.Height > max)
                    {
                        tree.Visible = true;
                        max = tree.Height;
                    }

                    if (max == 9) break;
                }
            }

            // Bottom up
            for (int col = 1; col < trees.First().Count() - 1; col ++)
            {
                int max = trees.Last().ElementAt(col).Height;

                foreach (var row in trees.Skip(1).SkipLast(1).Reverse())
                {
                    var tree = row.ElementAt(col);

                    if (tree.Height > max)
                    {
                        tree.Visible = true;
                        max = tree.Height;
                    }

                    if (max == 9) break;
                }
            }

            // Left to Right
            foreach (var row in trees.Skip(1).SkipLast(1))
            {
                int max = row.First().Height;

                foreach (var tree in row)
                {
                    if (tree.Height > max)
                    {
                        tree.Visible = true;
                        max = tree.Height;
                    }

                    if (max == 9) break;
                }
            }

            // Right to left
            foreach (var row in trees.Skip(1).SkipLast(1))
            {
                int max = row.Last().Height;

                foreach (var tree in row.AsEnumerable().Reverse())
                {
                    if (tree.Height > max)
                    {
                        tree.Visible = true;
                        max = tree.Height;
                    }

                    if (max == 9) break;
                }
            }

            result = trees.Sum(row => row.Count(tree => tree.Visible));

        }

        public static Tree At(this IEnumerable<IEnumerable<Tree>> trees, int row, int col)
        {
            return trees.ElementAt(row).ElementAt(col);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}