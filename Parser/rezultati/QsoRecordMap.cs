using CsvHelper.Configuration;
using yagg_vhf.Contract;

namespace yagg_vhf.Parser.rezultati
{
    internal class QsoRecordMap : ClassMap<QsoResultsRecord>
    {
        public QsoRecordMap()
        {
            Map(m => m.Callsign).Index(1);
            Map(m => m.WWLoc).Index(2);
            Map(m => m.QSOs).Index(3).Name("QSO", "QSOs");
            Map(m => m.WWLocs).Index(4).Name("WWLocs", "nWWLoc");
            Map(m => m.ODX).Index(5);
            Map(m => m.Pnts).Index(6).Name("Pnts", "Points");

        }
    }

}
