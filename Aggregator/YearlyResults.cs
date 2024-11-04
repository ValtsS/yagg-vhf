using yagg_vhf.Contract;

namespace yagg_vhf.Aggregator
{
    internal class YearlyResults
    {
        private readonly Config config;
        public int Year { get; set; }

        public Dictionary<string, List<OperatorResultsYr>> ResultsPerband = new();

        public IEnumerable<string> GetBands() => ResultsPerband.Keys;

        public OperatorResultsYr[] LatvianScores(string band) => ResultsPerband[band].Where(x => x.Latvian).OrderBy(x => x.BestN(config.BestMonths)).ToArray();
        public OperatorResultsYr[] InternationalScores(string band) => ResultsPerband[band].Where(x => !x.Latvian).OrderBy(x => x.BestN(config.BestMonths)).ToArray();

        public YearlyResults(int Year, IEnumerable<BandResults> bandResults, Config configuration)
        {
            this.config = configuration;
            this.Year = Year;
            var H = new Dictionary<string, OperatorResultsYr>();

            foreach (BandResults br in bandResults.Where(x => x.date.Year == Year))
            {
                foreach (var op in br.Data.Select(x => new { x.Callsign, x.Pnts, br.Latvian }))
                {
                    var key = $"{br.Band}|{ op.Callsign.ToUpper()}";
                    if (!H.ContainsKey(key))
                        H[key] = new OperatorResultsYr()
                        {
                            Band = br.Band,
                            Callsign = op.Callsign,
                            Year = br.date.Year,
                            Latvian = op.Latvian,
                        };
                    H[key].MonthlyScores[br.date.Month-1] = op.Pnts;
                }
            }

            var oprs = H.Select(x => x.Value).GroupBy(x => x.Band);

            foreach(var kv in oprs)
            {
                if (!ResultsPerband.ContainsKey(kv.Key))
                    ResultsPerband[kv.Key] = [];
                ResultsPerband[kv.Key].AddRange(kv);
            }

            foreach (var kv in ResultsPerband)
                kv.Value.Sort((a, b) => b.BestN(configuration.BestMonths).CompareTo(a.BestN(configuration.BestMonths)));


        }
    }
}
