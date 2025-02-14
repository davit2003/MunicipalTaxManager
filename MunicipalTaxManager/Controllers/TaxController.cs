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

        [HttpGet("all")]
        public async Task<ActionResult<List<TaxRecord>>> GetAllTaxes()
        {
            var allTaxes = await _repository.GetAllAsync();
            return Ok(allTaxes);
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


        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutTax(int id, [FromBody] TaxRecord updatedRecord)
        {
            if (id != updatedRecord.Id)
            {
                return BadRequest("Route ID does not match the TaxRecord ID.");
            }

            await _repository.UpdateAsync(updatedRecord);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTax(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound($"Tax record with ID={id} not found.");
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
