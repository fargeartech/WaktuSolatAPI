using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using WaktuSolatMY.Application.Constants;
using WaktuSolatMY.Application.Extensions;
using WaktuSolatMY.Domain.Solat;

namespace WaktuSolatMY.Application.Services
{
    public interface IJakimSolatService
    {
        Task GetSyncSolatAsync(CancellationToken cancellationToken = default);
        Task<PrayerTime> GetWakTuSolatByZoneAndDate(string zone, string date, CancellationToken cancellationToken = default);
        Task<HashSet<JakimZoneResponse>> GetWakTuSolatByLocation(string location, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PrayerTime>> GetWakTuSolatByZone(string zone, CancellationToken cancellationToken = default);
    }

    public sealed class JakimSolatService : IJakimSolatService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly string _directoryPath;

        public JakimSolatService(IHttpClientFactory clientFactory, IHostEnvironment hostingEnvironment)
        {
            _clientFactory = clientFactory;
            _hostingEnvironment = hostingEnvironment;
            _directoryPath = _hostingEnvironment.ContentRootPath + "/Jakim";
        }

        public async Task<IReadOnlyList<PrayerTime>> GetWakTuSolatByZone(string zone, CancellationToken cancellationToken = default)
        {
            string directoryPath = $"{_directoryPath}/{zone}.json";
            var allZones = await GetAllZones(cancellationToken).ConfigureAwait(false);

            string state = string.Empty;
            string description = string.Empty;

            for (int i = 0; i < allZones.Count; i++)
            {
                var zoneList = allZones[i].ZonesList;
                for (int y = 0; y < zoneList.Count; y++)
                {
                    if (zoneList[y].Code == zone)
                    {
                        state = allZones[i].State;
                        description = zoneList[y].Description;
                    }

                }
            }
            //"01-Jan-2023"
            var rawFile = await File.ReadAllTextAsync(directoryPath, cancellationToken).ConfigureAwait(false);
            var jakimRaw = JsonSerializer.Deserialize<JakimResponse>(rawFile);
            if (jakimRaw is null)
                return null;

            jakimRaw.state = state;
            jakimRaw.desc = description;
            return jakimRaw.prayerTime;
        }

        public async Task<PrayerTime> GetWakTuSolatByZoneAndDate(string zone, string date, CancellationToken cancellationToken = default)
        {
            string directoryPath = $"{_directoryPath}/{zone}.json";
            var allZones = await GetAllZones(cancellationToken).ConfigureAwait(false);

            string state = string.Empty;
            string description = string.Empty;

            for (int i = 0; i < allZones.Count; i++)
            {
                var zoneList = allZones[i].ZonesList;
                for (int y = 0; y < zoneList.Count; y++)
                {
                    if (zoneList[y].Code == zone)
                    {
                        state = allZones[i].State;
                        description = zoneList[y].Description;
                    }

                }
            }
            //"01-Jan-2023"
            var rawFile = await File.ReadAllTextAsync(directoryPath, cancellationToken).ConfigureAwait(false);
            var jakimRaw = JsonSerializer.Deserialize<JakimResponse>(rawFile);
            DateTime filterDate;
            if (DateTime.TryParseExact(date,
                                       Zones.Date_FormatSupported.Lists(),
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out filterDate))
            {
                var result = jakimRaw
                    .prayerTime
                    .FirstOrDefault(x => x.date == filterDate.ToString("dd-MMM-yyyy"));

                result.state = state;
                result.desc = description;

                return result;
            }
            return null;
        }

        public async Task<HashSet<JakimZoneResponse>> GetWakTuSolatByLocation(string location, CancellationToken cancellationToken = default)
        {
            var result = new HashSet<JakimZoneResponse>();
            var allZones = await GetAllZones(cancellationToken).ConfigureAwait(false);
            int allZonesCount = allZones.Count;
            for (int i = 0; i < allZonesCount; i++)
            {
                var zoneList = allZones[i].ZonesList;
                int zoneListCount = zoneList.Count;
                for (int y = 0; y < zoneListCount; y++)
                {
                    if (zoneList[y].Description.ToLowerInvariant().Contains(location.ToLowerInvariant()) || allZones[i].State.ToLowerInvariant().Contains(location.ToLowerInvariant()))
                    {
                        result.Add(allZones[i]);
                    }
                }
            }
            return result;
        }

        public async Task GetSyncSolatAsync(CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task>
            {
                SiteCrawler(cancellationToken),
                JakimTimeSync(cancellationToken)
            };

            await Task.WhenAll(tasks);
        }

        #region "private function"
        private async Task<IReadOnlyList<JakimZoneResponse>> GetAllZones(CancellationToken cancellationToken = default)
        {
            var result = new List<JakimZoneResponse>();
            var directoryPath = $"{_directoryPath}/zone.json";
            if (!File.Exists(directoryPath))
                return Array.Empty<JakimZoneResponse>();

            var jsonStr = await File.ReadAllTextAsync(directoryPath, cancellationToken);
            return JsonSerializer.Deserialize<List<JakimZoneResponse>>(jsonStr);
        }


        private async Task GetWakTuSolatByZoneAsync(string zone, CancellationToken cancellationToken = default)
        {
            var client = _clientFactory.CreateClient(Zones.ClientName.JAKIM);
            var result = await client.GetStringAsync($"?r=esolatApi/takwimsolat&period=year&zone={zone}",
                                                     cancellationToken).ConfigureAwait(false);
            string directoryPath = _directoryPath;
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string pathZone = $"{directoryPath}/{zone}.json";
            await File.WriteAllTextAsync(pathZone, result, cancellationToken).ConfigureAwait(false);
        }

        private async Task JakimTimeSync(CancellationToken cancellationToken = default)
        {
            var zones = Zones.Lists();
            int count = zones.Count;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < count; i++)
            {
                if (GetWakTuSolatByZoneAsync(zones[i], cancellationToken) is not null)
                    tasks.Add(GetWakTuSolatByZoneAsync(zones[i], cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task SiteCrawler(CancellationToken cancellation = default)
        {
            const string url = Zones.BaseUrls.JAKIM_Crawl;
            WebClient client = new WebClient();
            var jakimZones = new List<JakimZoneResponse>();
            try
            {
                var result = await client.DownloadStringTaskAsync(new Uri(url)).ConfigureAwait(false);

                string pattern = @"<select id=""inputZone"" class=""form-control"">([\w\W]*?)</select>";
                MatchCollection matches = Regex.Matches(result, pattern);
                pattern = @"<optgroup([\w\W]*?)</optgroup>";
                MatchCollection stateMatches = Regex.Matches(matches[0].Groups[1].Value, pattern);

                var stateJson = new Dictionary<string, Dictionary<string, string>>();
                int countStateMathes = stateMatches.Count;

                for (int i = 0; i < countStateMathes; i++)
                {
                    var zoneState = new JakimZoneResponse();
                    Match stateMatch = stateMatches[i];
                    const string statePattern = @"label=""([\w\W]*?)""";
                    Match stateNameMatch = Regex.Match(stateMatch.Value, statePattern);
                    string state = stateNameMatch.Groups[1].Value;
                    const string zonePattern = @"<option([\w\W]*?)</option>";
                    MatchCollection zoneMatches = Regex.Matches(stateMatch.Value, zonePattern);
                    int zoneMatchCount = zoneMatches.Count;
                    var zoneJson = new Dictionary<string, string>();
                    var zoneList = new List<ZoneInfo>();
                    for (int y = 0; y < zoneMatchCount; y++)
                    {
                        Match zoneMatch = zoneMatches[y];
                        const string zoneCodePattern = @"value='([\w\W]*?)'";
                        Match zoneCodeMatch = Regex.Match(zoneMatch.Value, zoneCodePattern);
                        string zoneCode = zoneCodeMatch.Groups[1].Value;
                        string zoneName = (zoneMatch.Value.Split(" - ")[1]).StripTags();
                        zoneJson[zoneCode] = zoneName;
                        zoneList.Add(new ZoneInfo
                        {
                            Code = zoneCode,
                            Description = zoneName,
                        });
                    }
                    stateJson[state] = zoneJson;
                    zoneState.State = state;
                    zoneState.ZonesList.AddRange(zoneList);
                    jakimZones.Add(zoneState);
                }

                string json = JsonSerializer.Serialize(jakimZones);
                await File.WriteAllTextAsync($"{_directoryPath}/zone.json", json, cancellation).ConfigureAwait(false);
            }
            finally
            {
                client.Dispose();
            }
        }
        #endregion
    }
}
