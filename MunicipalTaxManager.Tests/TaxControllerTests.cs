using Microsoft.AspNetCore.Mvc;
using MunicipalTaxManager.Controllers;
using MunicipalTaxManager.Models;
using MunicipalTaxManager.Repositories;
using Moq;

namespace MunicipalTaxManager.Tests
{
    public class TaxControllerTests
    {
        [Fact]
        public async Task GetTax_NoMatchingRecord_ReturnsNotFound()
        {
            var mockRepo = new Mock<ITaxRecordRepository>();
            mockRepo.Setup(r => r.GetByMunicipalityAndDateAsync("Oslo", It.IsAny<DateTime>()))
                    .ReturnsAsync(new List<TaxRecord>());

            var controller = new TaxController(mockRepo.Object);

            var result = await controller.GetTax("Oslo", DateTime.Now);

            var notFoundResult = Assert.IsType<ActionResult<decimal>>(result);
            Assert.IsType<NotFoundObjectResult>(notFoundResult.Result);
        }

        [Fact]
        public async Task PostTax_StartDateAfterEndDate_ReturnsBadRequest()
        {
            var mockRepo = new Mock<ITaxRecordRepository>();
            var controller = new TaxController(mockRepo.Object);

            var invalidRecord = new TaxRecord
            {
                Municipality = "Copenhagen",
                Rate = 0.3m,
                StartDate = new DateTime(2025, 12, 31),
                EndDate = new DateTime(2025, 1, 1)
            };

            var result = await controller.PostTax(invalidRecord);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
