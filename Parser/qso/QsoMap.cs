using CsvHelper.Configuration;
using yagg_vhf.Contract;

namespace yagg_vhf.Parser.qso
{
    internal class QsoMap : ClassMap<QsoRecord>
    {
        public QsoMap()
        {

            Map(m => m.Callsign).Index(0);
            Map(m => m.WWLoc).Index(1);
            Map(m => m.UTC).Index(2);
            Map(m => m.Corresp).Index(3);
            Map(m => m.Sent).Index(4);
            Map(m => m.Recvd).Index(5);
            Map(m => m.nWWLoc).Index(6);
            Map(m => m.Mode).Index(7);
            Map(m => m.Band).Index(8);
            Map(m => m.QRB).Index(9);
            Map(m => m.Pnt).Index(10);
            Map(m => m.nWWLoc4).Index(11);
            Map(m => m.Remark).Index(12);

        }
    }
}
