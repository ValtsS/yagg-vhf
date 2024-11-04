using CsvHelper.Configuration;
using yagg_vhf.Contract;

namespace yagg_vhf.Parser
{
    internal class QsoRecordMap : ClassMap<QsoRecord>
    {
        public QsoRecordMap()
        {
            Map(m => m.Callsign).Index(1);
            Map(m => m.WWLoc).Index(2);
            Map(m => m.QSOs).Index(3);
            Map(m => m.WWLocs).Index(4);
            Map(m => m.ODX).Index(5);
            Map(m => m.Pnts).Index(6);
        }
    }
}
