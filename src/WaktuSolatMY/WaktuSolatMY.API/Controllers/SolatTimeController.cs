using Microsoft.AspNetCore.Mvc;
using WaktuSolatMY.Application.Services;

namespace WaktuSolatMY.API.Controllers
{
    public class SolatTimeController : BaseController
    {
        private readonly IJakimSolatService _jakimSolatService;

        public SolatTimeController(IJakimSolatService jakimSolatService)
        {
            _jakimSolatService = jakimSolatService;
        }

        [HttpGet("{zone}")]
        public async Task<IActionResult> FindByZone(string zone, CancellationToken cancellationToken = default)
        {
            var result = await _jakimSolatService.GetWakTuSolatByZone(zone, cancellationToken).ConfigureAwait(false);
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{zone}/{date}")]
        public async Task<IActionResult> FindByZoneAndDate(string zone, string date, CancellationToken cancellationToken = default)
        {
            var result = await _jakimSolatService.GetWakTuSolatByZoneAndDate(zone, date, cancellationToken).ConfigureAwait(false);
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("/location/{location}")]
        public async Task<IActionResult> FindByLocation(string location, CancellationToken cancellationToken = default)
        {
            var result = await _jakimSolatService.GetWakTuSolatByLocation(location, cancellationToken).ConfigureAwait(false);
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// To sync with JAKIM server and save the response to our storage
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Sync(CancellationToken cancellationToken = default)
        {
            await _jakimSolatService.GetSyncSolatAsync(cancellationToken).ConfigureAwait(false);
            return Ok();
        }
    }
}
