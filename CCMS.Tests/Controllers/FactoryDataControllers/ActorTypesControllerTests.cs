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
    public class ActorTypesControllerTests
    {
        private readonly ActorTypesController _sut;
        private readonly IActorTypesService _mockActorTypesService = Substitute.For<IActorTypesService>();
        public ActorTypesControllerTests()
        {
            _sut = new ActorTypesController(_mockActorTypesService);
        }

        private static IEnumerable<ActorTypeModel> GetSampleData()
        {
            var result = new List<ActorTypeModel>
            {
                new ActorTypeModel { ActorTypeId = 1, ActorTypeName = "PETITIONER" },
                new ActorTypeModel { ActorTypeId = 2, ActorTypeName = "RESPONDENT" }
            };
            return result;
        }

        [Fact]
        public async Task GetAllActorTypes_Valid()
        {
            // Arrange
            IEnumerable<ActorTypeModel> expected = GetSampleData();
            _mockActorTypesService.RetrieveAllAsync().Returns(expected);

            // Act
            ActionResult<IEnumerable<ActorTypeModel>> response = await _sut.GetAllActorTypes();

            // Assert
            await _mockActorTypesService.Received(1).RetrieveAllAsync();
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<ActorTypeModel> actual = (IEnumerable<ActorTypeModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).ActorTypeId, actual.ElementAt(i).ActorTypeId);
                Assert.Equal(expected.ElementAt(i).ActorTypeName, actual.ElementAt(i).ActorTypeName);
            }
        }
    }
}
