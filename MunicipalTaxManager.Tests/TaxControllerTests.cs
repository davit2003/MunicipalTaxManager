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

        [Fact]
        public async Task GetAllTaxes_ReturnsOk_WithAllRecords()
        {
            var mockRepo = new Mock<ITaxRecordRepository>();
            var fakeData = new List<TaxRecord>
            {
                new TaxRecord { Id = 1, Municipality = "Municipality1", Rate = 0.1m },
                new TaxRecord { Id = 2, Municipality = "Municipality2", Rate = 0.2m }
            };

            mockRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(fakeData);

            var controller = new TaxController(mockRepo.Object);

            var result = await controller.GetAllTaxes();

            var okResult = Assert.IsType<ActionResult<List<TaxRecord>>>(result);
            var okObject = Assert.IsType<OkObjectResult>(okResult.Result);
            var returnedRecords = Assert.IsType<List<TaxRecord>>(okObject.Value);
            Assert.Equal(2, returnedRecords.Count);
        }

        [Fact]
        public async Task PutTax_ReturnsBadRequest_IfRouteIdMismatch()
        {
            var mockRepo = new Mock<ITaxRecordRepository>();
            var controller = new TaxController(mockRepo.Object);

            var recordToUpdate = new TaxRecord { Id = 2, Municipality = "TestMunicipality" };

            var result = await controller.PutTax(1, recordToUpdate);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Route ID does not match the TaxRecord ID.", badRequest.Value);
        }

        [Fact]
        public async Task PutTax_ReturnsNoContent_OnSuccess()
        {
            var mockRepo = new Mock<ITaxRecordRepository>();
            var controller = new TaxController(mockRepo.Object);

            var recordToUpdate = new TaxRecord { Id = 5, Municipality = "UpdatedMunicipality", Rate = 0.9m };

            var result = await controller.PutTax(5, recordToUpdate);

            Assert.IsType<NoContentResult>(result);
            mockRepo.Verify(r => r.UpdateAsync(recordToUpdate), Times.Once);
        }

        [Fact]
        public async Task DeleteTax_ReturnsNotFound_IfNoRecordWithId()
        {
            var mockRepo = new Mock<ITaxRecordRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaxRecord)null);

            var controller = new TaxController(mockRepo.Object);

            var result = await controller.DeleteTax(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Tax record with ID=999 not found.", notFound.Value);
        }

        [Fact]
        public async Task DeleteTax_ReturnsNoContent_WhenRecordExists()
        {
            var mockRepo = new Mock<ITaxRecordRepository>();
            var existingRecord = new TaxRecord { Id = 10, Municipality = "ExistingMunicipality" };
            mockRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(existingRecord);

            var controller = new TaxController(mockRepo.Object);

            var result = await controller.DeleteTax(10);

            Assert.IsType<NoContentResult>(result);
            mockRepo.Verify(r => r.DeleteAsync(10), Times.Once);
        }
    }
}
