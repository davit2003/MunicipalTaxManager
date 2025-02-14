using Microsoft.AspNetCore.Mvc;
using MunicipalTaxManager.Models;
using MunicipalTaxManager.Repositories;

namespace MunicipalTaxManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxController : ControllerBase
    {
        private readonly ITaxRecordRepository _repository;

        public TaxController(ITaxRecordRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<decimal>> GetTax(
            [FromQuery] string municipality,
            [FromQuery] DateTime date)
        {
            var matchingRecords = await _repository.GetByMunicipalityAndDateAsync(municipality, date);

            if (matchingRecords == null || matchingRecords.Count == 0)
            {
                return NotFound("No tax record found for that municipality and date.");
            }

            // Taking narrower date range as it overrides broader range
            var chosen = matchingRecords
                .OrderBy(r => (r.EndDate - r.StartDate).TotalDays)
                .First();

            return Ok(chosen.Rate);
        }

        [HttpPost]
        public async Task<ActionResult<TaxRecord>> PostTax(TaxRecord record)
        {
            if (record.StartDate > record.EndDate)
            {
                return BadRequest("StartDate cannot be after EndDate.");
            }

            await _repository.AddAsync(record);

            return CreatedAtAction(nameof(GetTax), new { municipality = record.Municipality, date = record.StartDate }, record);
        }
    }
}
