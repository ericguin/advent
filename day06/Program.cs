namespace Day06
{
    public static class Program
    {
        public static readonly int size = 14;

        public static void Main()
        {
            var input = File.ReadAllText("input.txt");

            int i;
            string search;
            for (i = size - 1; i < input.Length; i ++)
            {
                search = input.Substring(i - (size - 1), size);
                if (search.Distinct().Count() == size)
                {
                    break;
                }
            }
            
            i++;
        }
    }
}