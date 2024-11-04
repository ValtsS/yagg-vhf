namespace yagg_vhf
{
    internal class Config
    {
        public string TargetFiles = "Rezultati.csv";
        public string SplitLV_Int = "========================================================================";
        public string DateRE = @"^(?<month>\w+), (?<year>20\d{2})\r?\n";
        public string BandRE = @"^\=+\r?\n\w+ (?<band>[\d]+ \wHz)\r?\n=+";
        public string LatvianMarker = @"LATVIJAS STACIJAS".ToUpper();
        public string CSV_Start = @";Callsign;WWLoc;QSOs;WWLocs;ODX;Pnts";
        public string CSV_Delimiter = ";";
    }
}
