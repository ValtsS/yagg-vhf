using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
using System.Runtime.CompilerServices;
using yagg_vhf.Contract;
using yagg_vhf.Parser.rezultati;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace yagg_vhf.Parser.qso
{

    internal class QsoParser
    {
        private readonly Config config;

        private Dictionary<string, QsoRecord[]> qsoLog = new();
        internal Dictionary<string, QsoRecord[]> QsoLog { get => qsoLog; }

        public QsoParser(string contents, Config config, string id)
        {
            this.config = config;
            Console.WriteLine($"\tLoading QSOs for CSV {id}");
            Parse(contents);
        }

        private void Parse(string contents)
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = config.QSO_Delimiter,
                MissingFieldFound = null,
                IgnoreBlankLines = true,
                ShouldSkipRecord = row => string.IsNullOrWhiteSpace(row.Row.ToString())

            };

            using (var reader = new StringReader(contents))
            {
                using (var csv = new CsvReader(reader, csvConfig))
                {

                    csv.Context.RegisterClassMap<QsoMap>();

                    Console.Write($"\t\tParsing QSOs...");
                    try
                    {
                        var data = csv.GetRecords<QsoRecord>().ToArray();
                        HandleMGM(data);
                        qsoLog = data.GroupBy(x => x.Callsign).ToDictionary(x => x.Key, x => x.ToArray());
                        Console.WriteLine($"got {data.Length} total QSOs for {qsoLog.Count} callsigns");
                    }
                    catch
                    {
                        Console.WriteLine($"\nOffending QSO:\n{contents}");
                        throw;
                    }
                }
            }
        }

        private static void HandleMGM(QsoRecord[] data)
        {
            foreach (var record in data)
            {
                if (record.Mode.ToUpper()=="RTTY" ||  record.Mode.ToUpper()==string.Empty)
                {
                    record.Mode = "MGM";
                }
            }
        }


    }
}
