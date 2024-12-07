namespace yagg_vhf
{
    internal class Config
    {
        public string TargetFiles = "Rezultati.csv";
        public string SplitLV_Int = "========================================================================";
        public string DateRE = @"^(?<month>\w+), (?<year>20\d{2})\r?\n";
        public string BandRE = @"^\=+\r?\n\w+ (?<band>[\d]+ \wHz)\r?\n=+";
        public string LatvianMarker = @"LATVIJAS STACIJAS".ToUpper();
        public string[] CSV_Start = { @";Callsign;WWLoc;QSOs;WWLocs;ODX;Pnts", @";Callsign;WWLoc;QSO;nWWLoc;ODX;Points" };
        public string CSV_Delimiter = ";";
        public string QSO_Delimiter = "\t";
        public int BestMonths = 9;

        public Dictionary<string, string> CallsignsRemap = new Dictionary<string, string>()
        {
            { "YL3AOI", "YL3OI" },
            { "YL3GU/P", "YL3GU" }
        };


        // Band to Date
        public Dictionary<int, Tuple<int, DayOfWeek>> DateHelp = new Dictionary<int, Tuple<int, DayOfWeek>>()
        {
            { 144, new (0, DayOfWeek.Tuesday)  },
            { 432, new (1, DayOfWeek.Tuesday)  },
            { 50, new (1, DayOfWeek.Thursday)  },
            { 1296, new (2, DayOfWeek.Tuesday)  },
            { 70, new (3, DayOfWeek.Thursday)  },
        };

        public DateTime? DetectDate(int band, int year, int month)
        {

            DateTime d = new(year, month, 1);
            int counter = 0;

            var target = DateHelp[band];

            for (int i = 1; i <= 32; i++)
            {
                if (d.DayOfWeek == target.Item2 )
                {
                    if (counter == target.Item1)
                    {
                        return d.Date;
                    }
                    counter++;
                }
                d = d.AddDays(1);
            }


            return null;
        }

    }
}
