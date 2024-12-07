using yagg_vhf.Aggregator;
using yagg_vhf.Output.JSON;

namespace yagg_vhf.Contract
{
    internal class CompleteResultSet
    {
        private readonly MonthResults[] monthResults;
        private readonly YearlyResults[] yearlyResults;

        public CompleteResultSet(BandResults[] data, Config configuration)
        {
            monthResults = [.. data.GroupBy(x => x.date).Select(x => new MonthResults(x.Key, x.ToArray())).OrderBy(x => x.Year).ThenBy(x => x.Month)];
            yearlyResults = [.. data.GroupBy(x => x.date.Year).Select(x => new YearlyResults(x.Key, x, configuration))];


        }


        public MonthResults[] MonthResults { get => monthResults; }
        public YearlyResults[] YearlyResults { get => yearlyResults; }

        

    }
}
