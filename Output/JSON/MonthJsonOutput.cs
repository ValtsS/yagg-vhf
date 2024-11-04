using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using yagg_vhf.Aggregator;

namespace yagg_vhf.Output.JSON
{



    internal class MonthJsonOutput
    {
        public static string SerializeMonthResults(MonthResults monthResults)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,  // Makes JSON output more readable
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,  // Ignores null values
                Converters = { new DateOnlyJsonConverter() }  // Custom converter for DateOnly type
            };

            return JsonSerializer.Serialize(monthResults, options);
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
