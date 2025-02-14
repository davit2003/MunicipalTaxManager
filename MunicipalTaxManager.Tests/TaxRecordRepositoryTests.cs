using Microsoft.EntityFrameworkCore;
using MunicipalTaxManager.DataLayer;
using MunicipalTaxManager.Models;
using MunicipalTaxManager.Repositories;
using Xunit;

namespace MunicipalTaxManager.Tests
{
    public class TaxRecordRepositoryTests
    {
        private DbContextOptions<TaxDbContext> _options;

        public TaxRecordRepositoryTests()
        {
            // Each test gets a unique in-memory DB name to avoid collisions
            _options = new DbContextOptionsBuilder<TaxDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task GetByMunicipalityAndDateAsync_ReturnsCorrectRecords()
        {
            using var context = new TaxDbContext(_options);
            var repo = new TaxRecordRepository(context);

            var recordYearly = new TaxRecord
            {
                Municipality = "Copenhagen",
                Rate = 0.2m,
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2024, 12, 31)
            };
            context.TaxRecords.Add(recordYearly);

            var recordDaily = new TaxRecord
            {
                Municipality = "Copenhagen",
                Rate = 0.1m,
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2024, 1, 1)
            };
            context.TaxRecords.Add(recordDaily);

            await context.SaveChangesAsync();

            var results = await repo.GetByMunicipalityAndDateAsync("Copenhagen", new DateTime(2024, 1, 1));

            Assert.Equal(2, results.Count);
        }

        public async Task AddAsync_InsertsRecordSuccessfully()
        {
            using var context = new TaxDbContext(_options);
            var repository = new TaxRecordRepository(context);

            var record = new TaxRecord
            {
                Municipality = "Aarhus",
                Rate = 0.25m,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31)
            };

            await repository.AddAsync(record);

            var saved = context.TaxRecords.Single();
            Assert.Equal("Aarhus", saved.Municipality);
            Assert.Equal(0.25m, saved.Rate);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllRecords()
        {
            using var context = new TaxDbContext(_options);
            var repo = new TaxRecordRepository(context);

            context.TaxRecords.AddRange(new[]
            {
                new TaxRecord
                {
                    Municipality = "Municipality1", Rate = 0.1m,
                    StartDate = new DateTime(2024,1,1),
                    EndDate = new DateTime(2024,12,31)
                },
                new TaxRecord
                {
                    Municipality = "Municipality2", Rate = 0.2m,
                    StartDate = new DateTime(2024,6,1),
                    EndDate = new DateTime(2024,6,30)
                }
            });
            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Municipality == "Municipality1" && r.Rate == 0.1m);
            Assert.Contains(result, r => r.Municipality == "Municipality2" && r.Rate == 0.2m);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesRecord_WhenRecordExists()
        {
            using var context = new TaxDbContext(_options);
            var repo = new TaxRecordRepository(context);

            var record = new TaxRecord
            {
                Municipality = "Copenhagen",
                Rate = 0.2m,
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2024, 12, 31)
            };

            context.TaxRecords.Add(record);
            await context.SaveChangesAsync();

            record.Rate = 0.3m;
            await repo.UpdateAsync(record);

            var updated = await context.TaxRecords.FindAsync(record.Id);
            Assert.NotNull(updated);
            Assert.Equal(0.3m, updated!.Rate);
        }

        [Fact]
        public async Task DeleteAsync_DeletesRecord_WhenRecordExists()
        {
            using var context = new TaxDbContext(_options);
            var repo = new TaxRecordRepository(context);

            var record = new TaxRecord
            {
                Municipality = "Aarhus",
                Rate = 0.25m,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31)
            };
            context.TaxRecords.Add(record);
            await context.SaveChangesAsync();

            await repo.DeleteAsync(record.Id);

            var deleted = await context.TaxRecords.FindAsync(record.Id);
            Assert.Null(deleted);
        }
    }
}
