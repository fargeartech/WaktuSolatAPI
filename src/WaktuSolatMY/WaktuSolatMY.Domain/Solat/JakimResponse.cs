namespace WaktuSolatMY.Domain.Solat
{
    public sealed class JakimResponse
    {
        public JakimResponse()
        {
            this.prayerTime = Array.Empty<PrayerTime>();
        }

        public IReadOnlyList<PrayerTime> prayerTime { get; set; }
        public string status { get; set; }
        public string serverTime { get; set; }
        public string periodType { get; set; }
        public string lang { get; set; }
        public string zone { get; set; }
        public string bearing { get; set; }
    }

    public sealed class PrayerTime
    {
        public string hijri { get; set; }
        public string date { get; set; }
        public string day { get; set; }
        public string imsak { get; set; }
        public string fajr { get; set; }
        public string syuruk { get; set; }
        public string dhuhr { get; set; }
        public string asr { get; set; }
        public string maghrib { get; set; }
        public string isha { get; set; }
    }
}
