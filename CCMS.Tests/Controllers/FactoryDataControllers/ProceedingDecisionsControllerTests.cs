using CCMS.Server.Controllers.FactoryDataControllers;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.Controllers.FactoryDataControllers
{
    public class ProceedingDecisionsControllerTests
    {
        private readonly ProceedingDecisionsController _sut;
        private readonly IProceedingDecisionsService _mockProceedingDecisionsService = Substitute.For<IProceedingDecisionsService>();
        public ProceedingDecisionsControllerTests()
        {
            _sut = new ProceedingDecisionsController(_mockProceedingDecisionsService);
        }

        private static IEnumerable<ProceedingDecisionModel> GetSampleData()
        {
            var result = new List<ProceedingDecisionModel>
            {
                new ProceedingDecisionModel { ProceedingDecisionId = 1, ProceedingDecisionName = "PENDING", HasNextHearingDate = false, HasOrderAttachment = false },
                new ProceedingDecisionModel { ProceedingDecisionId = 2, ProceedingDecisionName = "ADJOURNMENT", HasNextHearingDate = true, HasOrderAttachment = false }
            };
            return result;
        }

        [Fact]
        public async Task GetAllProceedingDecisions_Valid()
        {
            // Arrange
            IEnumerable<ProceedingDecisionModel> expected = GetSampleData();
            _mockProceedingDecisionsService.RetrieveAllAsync().Returns(expected);

            // Act
            ActionResult<IEnumerable<ProceedingDecisionModel>> response = await _sut.GetAllProceedingDecisions();

            // Assert
            await _mockProceedingDecisionsService.Received(1).RetrieveAllAsync();
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<ProceedingDecisionModel> actual = (IEnumerable<ProceedingDecisionModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).ProceedingDecisionId, actual.ElementAt(i).ProceedingDecisionId);
                Assert.Equal(expected.ElementAt(i).ProceedingDecisionName, actual.ElementAt(i).ProceedingDecisionName);
                Assert.Equal(expected.ElementAt(i).HasNextHearingDate, actual.ElementAt(i).HasNextHearingDate);
                Assert.Equal(expected.ElementAt(i).HasOrderAttachment, actual.ElementAt(i).HasOrderAttachment);
            }
        }
    }
}
