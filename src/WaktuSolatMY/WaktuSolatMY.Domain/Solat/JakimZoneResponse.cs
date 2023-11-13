namespace WaktuSolatMY.Domain.Solat
{
    public sealed class JakimZoneResponse
    {
        public JakimZoneResponse()
        {
            ZonesList = new List<ZoneInfo>();
        }

        public string State { get; set; }
        public List<ZoneInfo> ZonesList { get; set; }
    }

    public sealed class ZoneInfo
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
