using yagg_vhf.Parser.rezultati;

namespace yagg_vhf.Contract
{
    internal class BandResults
    {
        public DateOnly date { get; set; }
        public required string Band { get; set; }
        public bool Latvian { get; set; }
        public required QsoResultsRecord[] Data { get; set; }

        public string UID => $"{Band.GetFirstWord()}.{date.Year}.{date.Month}";

        public void LoadQSODetails(Dictionary<string, QsoRecord[]> relatedQSOs, Dictionary<string, Config.MapTo> CallsignsRemap)
        {
            foreach(var dq in Data)
            {


                var relatedCallsigns = new HashSet<string>();
                relatedCallsigns.Add(dq.Callsign);

                bool updated = true;

                while (updated)
                {
                    updated = false;
                    foreach (var kv in CallsignsRemap)
                    {
                        if (relatedCallsigns.Contains(kv.Key) && (relatedCallsigns.Add(kv.Value.callsign))) updated = true;

                        if (relatedCallsigns.Contains(kv.Value.callsign)
                            && (relatedCallsigns.Add(kv.Key))) updated = true;

                    }
                }


                dq.Details = relatedCallsigns.Select(cs => relatedQSOs.GetValueOrDefault(cs, [])).SelectMany(q => q)
                    .Where(x => this.Band.StartsWith(x.Band + " ")).ToArray();

                if (dq.Details.Length < 1)
                    throw new ParsingException($"Failed to locate QSO details for callsign {dq.Callsign} and its aliases {string.Join(",", relatedCallsigns)} for band {Band}");
            }

        }

    }
}
