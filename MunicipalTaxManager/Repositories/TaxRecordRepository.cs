using Microsoft.EntityFrameworkCore;
using MunicipalTaxManager.DataLayer;
using MunicipalTaxManager.Models;

namespace MunicipalTaxManager.Repositories
{
    public interface ITaxRecordRepository
    {
        /// <summary>
        /// Returns all tax records that match the municipality and overlap the given date.
        /// </summary>
        Task<List<TaxRecord>> GetByMunicipalityAndDateAsync(string municipality, DateTime date);

        /// <summary>
        /// Inserts a new tax record into the store.
        /// </summary>
        Task AddAsync(TaxRecord record);
    }

    public class TaxRecordRepository : ITaxRecordRepository
    {
        private readonly TaxDbContext _context;

        public TaxRecordRepository(TaxDbContext context)
        {
            _context = context;
        }

        public Task<List<TaxRecord>> GetByMunicipalityAndDateAsync(
            string municipality,
            DateTime date)
        {
            return _context.TaxRecords
                .Where(r => r.Municipality == municipality
                         && r.StartDate <= date
                         && r.EndDate >= date)
                .ToListAsync();
        }

        public async Task AddAsync(TaxRecord record)
        {
            _context.TaxRecords.Add(record);
            await _context.SaveChangesAsync();
        }
    }
}
