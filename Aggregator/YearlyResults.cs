using yagg_vhf.Contract;
using yagg_vhf.Parser.rezultati;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace yagg_vhf.Aggregator
{
    internal class YearlyResults
    {
        private readonly Config config;
        public int Year { get; set; }

        protected Dictionary<string, List<OperatorResultsYr>> ResultsPerband = new();

        public IEnumerable<string> GetBands() => ResultsPerband.Keys;

        public OperatorResultsYr[] LatvianScores(string band) => ResultsPerband[band].Where(x => x.Latvian).OrderBy(x => x.BestN(config.BestMonths)).ToArray();
        public OperatorResultsYr[] InternationalScores(string band) => ResultsPerband[band].Where(x => !x.Latvian).OrderBy(x => x.BestN(config.BestMonths)).ToArray();

        public int BestMonths => config.BestMonths;

        public Dictionary<string, List<QsoRecord>> QSODetails = new();

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

                    H[key].MonthlyScores[br.date.Month - 1] = new MonthlyScore()
                    {
                        Score = op.Pnts,
                        Link = $"{br.Band.GetFirstWord()}.{this.Year}.{br.date.Month}.{op.Callsign}" //50.2023.12.31.YL2AO
                    };
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


            // QSOs

            var contestQSOs = bandResults.GroupBy(g => g.UID)
                .ToDictionary(x => x.Key, x => x.SelectMany(y => y.Data).GroupBy(z => z.Callsign)
                .ToDictionary(t => t.Key, t => t.ToArray()));

            foreach (var kv in contestQSOs)
            {
                foreach (var vk in kv.Value)
                {
                    var key = $"{kv.Key}.{vk.Key}";
                    if (!QSODetails.ContainsKey(key))
                        QSODetails[key] = new();


                    var notPretty = kv.Key.Split('.');
                    var band = int.Parse(notPretty[0]);
                    var baseDate = config.DetectDate(band, int.Parse(notPretty[1]), int.Parse(notPretty[2]));

                    if ( baseDate == null)
                    {
                        throw new ParsingException($"Failed to detect date of contest {kv.Key}");
                    }

                    var entries = vk.Value.SelectMany(z => z.Details);

                    foreach (var entry in entries.Where(x => x.DateTimeUTC == null))
                    {
                        entry.DateTimeUTC = baseDate.Value;
                        var hrs = int.Parse(entry.UTC[..2]);
                        entry.DateTimeUTC = entry.DateTimeUTC.Value.AddHours(hrs);
                        var mins = int.Parse(entry.UTC[2..]);
                        entry.DateTimeUTC = entry.DateTimeUTC.Value.AddMinutes(mins);

                    }

                    QSODetails[key].AddRange(entries);
                }
            }


        }
    }
}
