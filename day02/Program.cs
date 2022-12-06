namespace Day2
{
    public enum Choice
    {
        ROCK = 1,
        PAPER = 2,
        SCISSORS = 3,
    }

    public enum Outcome
    {
        LOSS = 0,
        TIE = 3,
        WIN = 6,
    }

    public record Play(Choice them, Choice you);
    public record Rule(Choice win, Choice lose);

    static class Program
    {
        static readonly Dictionary<char, Choice> choice_map = new (){
            {'A', Choice.ROCK},
            {'B', Choice.PAPER},
            {'C', Choice.SCISSORS},
            {'X', Choice.ROCK},
            {'Y', Choice.PAPER},
            {'Z', Choice.SCISSORS},
        };

        static readonly Dictionary<char, Outcome> intent_map = new (){
            {'X', Outcome.LOSS},
            {'Y', Outcome.TIE},
            {'Z', Outcome.WIN}
        };

        static readonly Dictionary<Choice, Rule> rules_map = new (){
            {Choice.ROCK, new(Choice.SCISSORS, Choice.PAPER)},
            {Choice.PAPER, new(Choice.ROCK, Choice.SCISSORS)},
            {Choice.SCISSORS, new(Choice.PAPER, Choice.ROCK)},
        };

        public static int Main()
        {
            var stuff = File.ReadAllLines("input.txt");
            uint score = 0;
            uint calc_score = 0;

            foreach (var line in stuff)
            {
                var play = line.ParsePlay();
                var calc = line.CalculatePlay();
                score += play.ScorePlay();
                calc_score += calc.ScorePlay();
            }

            Console.WriteLine(score);
            Console.WriteLine(calc_score);

            return 0;
        }

        public static Outcome RunPlay(this Play play)
        {
            if (play.you == play.them) return Outcome.TIE;
            else if (rules_map[play.you].win == play.them) return Outcome.WIN;
            else return Outcome.LOSS;
        }

        public static uint ScorePlay(this Play play)
        {
            uint score = 0;
            score += (uint)play.RunPlay();
            score += (uint)play.you;
            return score;
        }

        public static Choice ParseChoice(this string bit)
        {
            return choice_map[bit.ToUpper()[0]];
        }

        public static Outcome ParseIntent(this string bit)
        {
            return intent_map[bit.ToUpper()[0]];
        }

        public static Play ParsePlay(this string line)
        {
            var bits = line.Split(' ');
            return new Play(bits[0].ParseChoice(), bits[1].ParseChoice());
        }

        public static Play CalculatePlay(this string line)
        {
            var bits = line.Split(' ');
            var them = bits[0].ParseChoice();
            var intent = bits[1].ParseIntent();

            var you = Choice.ROCK;
            if (intent == Outcome.TIE)
            {
                you = them;
            }
            else if (intent == Outcome.WIN)
            {
                you = rules_map[them].lose;
            }
            else
            {
                you = rules_map[them].win;
            }

            return new Play(them, you);
        }
    }
}
