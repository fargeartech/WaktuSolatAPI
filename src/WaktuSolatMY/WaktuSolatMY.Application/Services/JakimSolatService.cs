using Microsoft.Extensions.Hosting;
using System.Text.Json;
using WaktuSolatMY.Application.Constants;
using WaktuSolatMY.Domain.Solat;

namespace WaktuSolatMY.Application.Services
{
    public interface IJakimSolatService
    {
        Task<JakimResponse> GetWakTuSolatByZone(string zone, CancellationToken cancellationToken = default);
        Task GetSyncSolatAsync(CancellationToken cancellationToken = default);
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

        public async Task<JakimResponse> GetWakTuSolatByZone(string zone, CancellationToken cancellationToken = default)
        {
            var directoryPath = $"{_directoryPath}/{zone}.json";

            if (!File.Exists(directoryPath))
                return null;

            var rawFile = await File.ReadAllTextAsync(directoryPath,cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<JakimResponse>(rawFile);
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

        public async Task GetSyncSolatAsync(CancellationToken cancellationToken = default)
        {
            try
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
            catch (Exception)
            {
                //add logging functionality later
                throw;
            }
        }
    }
}
