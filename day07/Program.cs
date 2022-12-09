namespace Day07
{
    abstract class Entry
    {
        public abstract int Size();
    }

    class Directory : Entry
    {
        public Dictionary<string, Entry> Entries = new ();

        private readonly Directory? Parent;

        public Directory(Directory? parent)
        {
            Parent = parent;
        }

        public override int Size()
        {
            return Entries.Values.Sum(e => e.Size());
        }

        public virtual Directory GetParent()
        {
            return Parent ?? this;
        }
    }

    class Item : Entry
    {
        private readonly int _size;

        public override int Size() => _size;

        public Item(int size)
        {
            _size = size;
        }
    }

    public enum Mode
    {
        NONE,
        LS,
    }

    public static class Program
    {
        public static void Main()
        {
            var lines = File.ReadAllLines("input.txt").Skip(1);

            Directory root = new Directory(null);
            Directory cwd = root;
            Mode mode = Mode.NONE;

            List<Directory> allDirectories = new List<Directory>();
            
            foreach (var line in lines)
            {
                if (line.StartsWith("$"))
                {
                    mode = Mode.NONE;
                    if (line.Contains("ls"))
                    {
                        mode = Mode.LS;
                    }
                    else if (line.Contains("cd"))
                    {
                        var name = line.Substring(5);
                        if (name == "..")
                        {
                            cwd = cwd.GetParent();
                        }
                        else
                        {
                            if (cwd.Entries[name] is Directory d) cwd = d;
                        }
                    }
                }
                else
                {
                    if (mode == Mode.LS)
                    {
                        var parts = line.Split(" ");

                        if (int.TryParse(parts[0], out int val))
                        {
                            cwd.Entries[parts[1]] = new Item(val);
                        }
                        else
                        {
                            var dir = new Directory(cwd);
                            allDirectories.Add(dir);
                            cwd.Entries[parts[1]] = dir;
                        }
                    }
                }
            }

            int sizes = allDirectories.Where(d => d.Size() <= 100000).Sum(d => d.Size());
            int total = 70000000;
            int required = 30000000;
            int available = total - root.Size();
            int needed = required - available;

            var candidates = allDirectories.Where(d => d.Size() >= needed).OrderBy(d => d.Size());
            var smallest = candidates.First();
        }
    }
}