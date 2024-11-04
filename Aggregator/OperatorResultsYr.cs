namespace yagg_vhf.Aggregator
{
    internal class OperatorResultsYr
    {

        required public int Year { get; set; }
        required public string Band { get; set; }
        required public string Callsign { get; set; }
        required public bool Latvian { get; set; }
        public int[] MonthlyScores = new int[12];

        public int Total => MonthlyScores.Sum();
        public int BestN(int count) => MonthlyScores.OrderByDescending( x => x).Take(count).Sum();

    }
}
