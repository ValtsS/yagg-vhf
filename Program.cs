using System.ComponentModel.Design;
using yagg_vhf.Aggregator;
using yagg_vhf.Contract;
using yagg_vhf.Parser;
using static System.Net.Mime.MediaTypeNames;

namespace yagg_vhf
{

    internal class CompleteResultSet
    {
        private readonly MonthResults[] monthResults;
        private readonly YearlyResults[] yearlyResults;

        public CompleteResultSet(BandResults[] data, Config configuration) {
            monthResults = [.. data.GroupBy(x => x.date).Select(x => new MonthResults(x.Key, x.ToArray())).OrderBy( x => x.Year).ThenBy( x => x.Month)];
            yearlyResults = [.. data.GroupBy(x => x.date.Year).Select(x => new YearlyResults(x.Key, x, configuration))];
        }

        public MonthResults[] MonthResults { get => monthResults;  }
        public YearlyResults[] YearlyResults { get => yearlyResults; }
    }



    internal class Program
    {

        static string pwd()
        {
            return Directory.GetCurrentDirectory();
        }

        static void Main(string[] args)
        {



            var conf = new Config();

            if (args.Length > 0)
            {

                IEnumerable<String> matchingFilePaths2 = System.IO.Directory.EnumerateFiles(pwd(), conf.TargetFiles, System.IO.SearchOption.AllDirectories);
                var data = matchingFilePaths2.Select( file => new ResultsParser(File.ReadAllText(file), conf, file)).SelectMany( parser => parser.scores).ToArray();



                var results = new CompleteResultSet(data, conf);


            }
        }
    }
}
