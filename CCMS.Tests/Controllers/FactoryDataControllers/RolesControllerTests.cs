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
    public class RolesControllerTests
    {
        private readonly RolesController _sut;
        private readonly IRolesService _mockRolesService = Substitute.For<IRolesService>();
        public RolesControllerTests()
        {
            _sut = new RolesController(_mockRolesService);
        }

        private static IEnumerable<RoleModel> GetSampleData()
        {
            var result = new List<RoleModel>
            {
                new RoleModel { RoleId = 1, RoleName = "Operator", RoleDescription = "TBD" },
                new RoleModel { RoleId = 2, RoleName = "Manager", RoleDescription = "TBD" }
            };
            return result;
        }

        [Fact]
        public async Task GetAllRoles_Valid()
        {
            // Arrange
            IEnumerable<RoleModel> expected = GetSampleData();
            _mockRolesService.RetrieveAllAsync().Returns(expected);

            // Act
            ActionResult<IEnumerable<RoleModel>> response = await _sut.GetAllRoles();

            // Assert
            await _mockRolesService.Received(1).RetrieveAllAsync();
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<RoleModel> actual = (IEnumerable<RoleModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).RoleId, actual.ElementAt(i).RoleId);
                Assert.Equal(expected.ElementAt(i).RoleName, actual.ElementAt(i).RoleName);
                Assert.Equal(expected.ElementAt(i).RoleDescription, actual.ElementAt(i).RoleDescription);
            }
        }

        [Fact]
        public async Task GetRoleId_NotFound()
        {
            // Arrange
            string rolesCsv = "A,B,C";
            int expected = -1;
            _mockRolesService.GetRoleIdAsync(rolesCsv).Returns(expected);

            // Act
            ActionResult<int> response = await _sut.GetRoleId(rolesCsv);

            // Assert
            await _mockRolesService.Received(1).GetRoleIdAsync(rolesCsv);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task GetRoleId_Valid()
        {
            // Arrange
            string rolesCsv = "Operator,Manager";
            int expected = 3;
            _mockRolesService.GetRoleIdAsync(rolesCsv).Returns(expected);

            // Act
            ActionResult<int> response = await _sut.GetRoleId(rolesCsv);

            // Assert
            await _mockRolesService.Received(1).GetRoleIdAsync(rolesCsv);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            int actual = (int)createdAtActionResult.Value;
            Assert.Equal(expected, actual);
        }
    }
}
