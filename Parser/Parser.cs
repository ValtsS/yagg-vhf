using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using yagg_vhf.Contract;

namespace yagg_vhf.Parser
{
    internal class ResultsParser
    {
        private readonly Config config;

        public BandResults[] scores { get; set; }

        private DateOnly? DetectDate(string str)
        {

            var lines = ResultsParserHelpers.GetNLines(str, 10);
            var RE = new Regex(config.DateRE, RegexOptions.Multiline);
            var captureCollections = RE.Matches(lines).Where(c => c.Success);
            var names = new MonthNames();

            foreach (Match capt in captureCollections)
            {
                var month = capt.Groups["month"].Value.ToUpper();
                var yr = int.Parse(capt.Groups["year"].Value);
                if (names.TryGetValue(month, out int monthIdx))
                {
                    return new DateOnly(yr, monthIdx, 1);
                }
            }

            return null;
        }

        List<string> ExtractAllScoresTexts(string text)
        {

            List<string> results = new();

            bool active = false;

            var sb = new StringBuilder();

            using (var sr = new StringReader(text))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {

                    if (!active && ResultsParserHelpers.IsSplitter(line, 7))
                        active = true;

                    if (active)
                        sb.AppendLine(line);


                    if (active && string.IsNullOrWhiteSpace(line))
                    {
                        results.Add(sb.ToString());
                        sb.Clear();
                        active = false;
                    }
                }

            }

            if (sb.Length > 0)
                results.Add(sb.ToString());

            return results;

        }

        private string DetectBand(string text)
        {
            var RE = new Regex(config.BandRE, RegexOptions.Multiline);

            var match = RE.Match(text);
            if (match != null && match.Success)
            {
                return match.Groups["band"].Value;
            }


            return null;
        }


        private string extractCSV(string text)
        {

            using (var reader = new StringReader(text))
            {
                string line;
                int prevPos = 0;
                StringBuilder sb = null;
                while ((line = reader.ReadLine()) != null)
                {

                    if (line.StartsWith(config.CSV_Start))
                    {
                        sb = new StringBuilder();
                    }

                    if (string.IsNullOrWhiteSpace(line))
                        return sb == null ? string.Empty : sb.ToString();

                    if (sb != null)
                        sb.AppendLine(line);

                }

            }

            return string.Empty;

        }


        internal BandResults parseScoreText(string text, DateOnly date)
        {

            var band = DetectBand(text);

            if (band != null)
            {
                var score = new BandResults()
                {
                    date = date,
                    Band = band,
                    Latvian = text.ToUpper().Contains(config.LatvianMarker),
                    Data = []
                };

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = config.CSV_Delimiter,
                    MissingFieldFound = null,
                    IgnoreBlankLines = true,
                    ShouldSkipRecord = row => string.IsNullOrWhiteSpace(row.Row.ToString())
                };

                using (var reader = new StringReader(extractCSV(text)))
                {
                    using (var csv = new CsvReader(reader, csvConfig))
                    {
                        Console.Write($"\t\tParsing {band}...");
                        try
                        {
                            csv.Context.RegisterClassMap<QsoRecordMap>();
                            score.Data = csv.GetRecords<QsoRecord>().ToArray();
                        }
                        catch
                        {
                            Console.WriteLine($"\nOffending csv:\n{extractCSV(text)}");
                            File.WriteAllText("problem.csv", extractCSV(text));
                            throw;
                        }
                        finally
                        {
                            Console.WriteLine($"got {score.Data?.Length} entries");
                        }

                    }
                }


                return score;


            }

            return null;
        }

        public ResultsParser(string text, Config config, string fileName)
        {
            Console.WriteLine($"Parsing {fileName}");

            this.config = config;
            var idx = text.IndexOf(config.SplitLV_Int);
            var LV = text.Substring(0, idx);
            var ENG = text.Substring(idx + config.SplitLV_Int.Length);


            var date = DetectDate(ENG);

            if (date == null)
                throw new ParsingException($"Failed to detect month and year");

            Console.WriteLine($"\tDate:{date}");

            var LV_scoreTexts = ExtractAllScoresTexts(LV);
            var EN_scoreTexts = ExtractAllScoresTexts(ENG);

            scores = LV_scoreTexts.Select(x => parseScoreText(x, date.Value)).Where(x => x != null).
                Union(
            EN_scoreTexts.Select(x => parseScoreText(x, date.Value)).Where(x => x != null)).ToArray();

            if (scores.Length != 8 && scores.Length != 10)
            {
                throw new ParsingException($"Not all scores found");
            }

        }

    }

}
