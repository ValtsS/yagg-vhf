using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yagg_vhf.Contract;

namespace yagg_vhf.Aggregator
{
    internal class MonthResults
    {
        private readonly BandResults[] sortedResults;

        public int Year { get; set; }
        public int Month { get; set; }

        public BandResults[] SortedResults => sortedResults;
        public MonthResults(DateOnly dateOnly, IEnumerable<BandResults> bandResults)
        {
            Year = dateOnly.Year;
            Month = dateOnly.Month;
            sortedResults = bandResults.Where(x => x.date == dateOnly).OrderBy(x => x.Band).ThenBy(y => !y.Latvian).ToArray();
        }


    }
}
