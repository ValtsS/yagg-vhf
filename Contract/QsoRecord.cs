namespace yagg_vhf.Contract
{
    internal class QsoRecord
    {
        public required string Callsign { get; set; }
        public required string WWLoc { get; set; }
        public required string UTC { get; set; }
        public required string Corresp { get; set; }
        public required string Sent { get; set; }
        public required string Recvd { get; set; }
        public required string nWWLoc { get; set; }
        public required string Mode { get; set; }
        public required string Band { get; set; }
        public required string QRB { get; set; }
        public required string Pnt { get; set; }
        public required string nWWLoc4 { get; set; }
        public required string Remark { get; set; }

        public required DateTime? DateTimeUTC { get; set; }

    }
}
