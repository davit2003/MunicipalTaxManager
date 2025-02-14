using Microsoft.EntityFrameworkCore;
using MunicipalTaxManager.DataLayer;
using MunicipalTaxManager.Models;

namespace MunicipalTaxManager.Repositories
{
    public interface ITaxRecordRepository
    {
        /// <summary>
        /// Get a tax record by id.
        /// </summary>
        Task<TaxRecord?> GetByIdAsync(int id);

        /// <summary>
        /// Get all tax records.
        /// </summary>
        Task<List<TaxRecord>> GetAllAsync();

        /// <summary>
        /// Returns all tax records that match the municipality and overlap the given date.
        /// </summary>
        Task<List<TaxRecord>> GetByMunicipalityAndDateAsync(string municipality, DateTime date);

        /// <summary>
        /// Inserts a new tax record.
        /// </summary>
        Task AddAsync(TaxRecord record);

        /// <summary>
        /// Update an existing tax record.
        /// </summary>
        Task UpdateAsync(TaxRecord record);

        /// <summary>
        /// Delete an existing tax record.
        /// </summary>
        Task DeleteAsync(int id);
    }

    public class TaxRecordRepository : ITaxRecordRepository
    {
        private readonly TaxDbContext _context;

        public TaxRecordRepository(TaxDbContext context)
        {
            _context = context;
        }

        public Task<TaxRecord?> GetByIdAsync(int id)
        {
            return _context.TaxRecords.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<TaxRecord>> GetAllAsync()
        {
            return _context.TaxRecords.ToListAsync();
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
        
        public async Task UpdateAsync(TaxRecord record)
        {
            _context.TaxRecords.Update(record);
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteAsync(int id)
        {
            var entity = await _context.TaxRecords.FindAsync(id);
            if (entity != null)
            {
                _context.TaxRecords.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
