using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using yagg_vhf.Aggregator;
using yagg_vhf.Contract;

namespace yagg_vhf.Output.JSON
{


    internal class OperatorData {

        public string OperatorName { get; set; }
        public MonthlyScore[] Scores { get; set; }

        public int BestScore;

        public OperatorData(OperatorResultsYr opYrData, int bestCount)
        {
            this.OperatorName = opYrData.Callsign;
            this.BestScore = opYrData.BestN(bestCount);

            this.Scores = opYrData.MonthlyScores
                        .Concat([ new MonthlyScore() { Score = this.BestScore }, new MonthlyScore() { Score = opYrData.Total }])
                .ToArray();

        }

    }


    internal class BandResults
    {

        public string Band {  get; set; }

        public OperatorData[] LatvianData { get; set; }
        public OperatorData[] IntData { get; set; }

        public BandResults(string Band, OperatorResultsYr[] latvian, OperatorResultsYr[] international, int bestCount) {

            this.Band = Band;
            this.LatvianData = latvian.Select( x =>  new OperatorData(x, bestCount)).OrderByDescending(x => x.BestScore).ToArray();
            this.IntData = international.Select(x => new OperatorData(x, bestCount)).OrderByDescending(x => x.BestScore).ToArray();

        }


    }

    internal class JsonDataYear
    {
        public int Year { get; set; }
        public int BestMonthsCount { get; set; }
        public BandResults[] results { get; set; }

        public JsonDataYear(YearlyResults yearlyResults)
        {
            this.Year = yearlyResults.Year;
            this.BestMonthsCount = yearlyResults.BestMonths;
            this.results = yearlyResults.GetBands().OrderBy(x => x).Select(band =>
                new BandResults(band, yearlyResults.LatvianScores(band), yearlyResults.InternationalScores(band), BestMonthsCount)
            ).ToArray();
        }

    }


    internal class JsonData
    {

        public JsonData(CompleteResultSet data)
        {
            this.Years = data.YearlyResults.Select(x => new JsonDataYear(x)).ToArray();
        }

        public JsonDataYear[] Years { get; set; }
    }


    internal class JsonDataDetails
    {
        public int Year { get; set; }

        public JsonDataDetails(CompleteResultSet data, int year)
        {
            this.Year = year;
            QSOs = new();
            foreach (var kv in data.YearlyResults.Where(x => x.Year == year).SelectMany(x => x.QSODetails))
            {
                var entries = kv.Value.ToArray();

                QSOs[kv.Key] = entries;
            }
        }

        public Dictionary<string, QsoRecord[]> QSOs { get; set; }
    }



    internal class JsonOutput: Output
    {

        private readonly JsonData data;
        private readonly Dictionary<int,JsonDataDetails> details;

        public JsonOutput(CompleteResultSet complete) : base(complete)
        {
            data = new JsonData(complete);
            details = complete.YearlyResults.Select(yr => new JsonDataDetails(complete, yr.Year)).ToDictionary(x => x.Year, x => x);
        }

        public static string SerializeData(JsonData data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,  // Makes JSON output more readable
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,  // Ignores null values
                Converters = { new DateOnlyJsonConverter() }  // Custom converter for DateOnly type
            };

            return JsonSerializer.Serialize(data, options);
        }

        public static string SerializeData(JsonDataDetails data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,  // Makes JSON output more readable
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,  // Ignores null values
                Converters = { new DateOnlyJsonConverter() }  // Custom converter for DateOnly type
            };

            return JsonSerializer.Serialize(data, options);
        }

        public override void Produce()
        {
            var a = SerializeData(this.data);
            File.WriteAllText("results.json", a);

            foreach (var yearDet in this.details)
            {
                var b = SerializeData(yearDet.Value);
                File.WriteAllText($"{yearDet.Key}.json", b);
            }


        }
    }

    // Custom converter for DateOnly type
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateOnly.ParseExact(reader.GetString()!, Format);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }

}
