namespace yagg_vhf.Contract
{
    internal class BandResults
    {
        public DateOnly date { get; set; }
        public required string Band { get; set; }
        public bool Latvian { get; set; }
        public required QsoRecord[] Data { get; set; }

    }
}
