namespace yagg_vhf.Aggregator
{

    internal struct MonthlyScore
    {
        public int Score { get; set; }
        public string? Link { get; set; }

        public MonthlyScore()
        {
            Score = 0;
            Link = null;
        }
    }

    internal class OperatorResultsYr
    {

        required public int Year { get; set; }
        required public string Band { get; set; }
        required public string Callsign { get; set; }
        required public bool Latvian { get; set; }
        public MonthlyScore[] MonthlyScores = new MonthlyScore[12];

        public int Total => MonthlyScores.Select(x => x.Score).Sum();
        public int BestN(int count) => MonthlyScores.OrderByDescending( x => x.Score).Take(count).Select(x => x.Score).Sum();

    }
}
