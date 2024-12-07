namespace yagg_vhf.Contract
{
    internal class QsoResultsRecord
    {
        public required int Position { get; set; }
        public required string Callsign { get; set; }
        public required string WWLoc { get; set; }
        public required int QSOs { get; set; }
        public required int WWLocs { get; set; }
        public required int ODX { get; set; }
        public required int Pnts { get; set; }

        public QsoRecord[] Details { get; set; } = [];
    }
}
