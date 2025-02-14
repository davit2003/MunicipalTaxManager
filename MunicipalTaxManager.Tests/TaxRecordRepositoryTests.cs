using Microsoft.EntityFrameworkCore;
using MunicipalTaxManager.DataLayer;
using MunicipalTaxManager.Models;
using MunicipalTaxManager.Repositories;
using Xunit;

namespace MunicipalTaxManager.Tests
{
    public class TaxRecordRepositoryTests
    {
        [Fact]
        public async Task GetByMunicipalityAndDateAsync_ReturnsCorrectRecords()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TaxDbContext>()
                .UseInMemoryDatabase("TestDb_Repo")
                .Options;

            using var context = new TaxDbContext(options);
            var repo = new TaxRecordRepository(context);

            // Insert data
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

            Assert.Equal(2, results.Count); // Both daily & yearly match
        }

        public async Task AddAsync_InsertsRecordSuccessfully()
        {
            var options = new DbContextOptionsBuilder<TaxDbContext>()
                .UseInMemoryDatabase(databaseName: "AddAsyncTestDb")
                .Options;

            using var context = new TaxDbContext(options);
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
    }
}
