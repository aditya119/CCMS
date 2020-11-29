using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.LawyerModels;
using Dapper.Oracle;
using NSubstitute;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.DbServices
{
    public class LawyersServiceTests
    {
        private readonly LawyersService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public LawyersServiceTests()
        {
            _sut = new LawyersService(_mockDataAccess);
        }

        private static SqlParamsModel GetParams_CreateAsync(NewLawyerModel lawyerModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_lawyers.p_create_new_lawyer",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_lawyer_email", lawyerModel.LawyerEmail, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_lawyer_fullname", lawyerModel.LawyerFullname, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_lawyer_phone", lawyerModel.LawyerPhone, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_lawyer_address", lawyerModel.LawyerAddress, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_lawyer_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task CreateAsync_Valid()
        {
            // Arrange
            NewLawyerModel lawyerModel = new()
            {
                LawyerEmail = "abc@xyz.com",
                LawyerFullname = "ABC",
                LawyerAddress = "Earth",
                LawyerPhone = "1234"
            };
            SqlParamsModel queryParams = GetParams_CreateAsync(lawyerModel);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.CreateAsync(lawyerModel);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static IEnumerable<LawyerListItemModel> GetSampleData_RetrieveAllAsync()
        {
            var result = new List<LawyerListItemModel>
            {
                new LawyerListItemModel { LawyerId = 1, LawyerNameAndEmail = "ABC (abc@xyz.com)" },
                new LawyerListItemModel { LawyerId = 2, LawyerNameAndEmail = "DEF (def@xyz.com)" }
            };
            return result;
        }
        private static SqlParamsModel GetParams_RetrieveAllAsync()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_lawyers.p_get_all_lawyers",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrieveAllAsync_Valid()
        {
            // Arrange
            SqlParamsModel queryParams = GetParams_RetrieveAllAsync();
            IEnumerable<LawyerListItemModel> expected = GetSampleData_RetrieveAllAsync();
            _mockDataAccess.QueryAsync<LawyerListItemModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<LawyerListItemModel> actual = await _sut.RetrieveAllAsync();

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<LawyerListItemModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).LawyerId, actual.ElementAt(i).LawyerId);
                Assert.Equal(expected.ElementAt(i).LawyerNameAndEmail, actual.ElementAt(i).LawyerNameAndEmail);
            }
        }

        private static LawyerDetailsModel GetSampleData_RetrieveAsync()
        {
            var result = new LawyerDetailsModel
            {
                LawyerId = 1,
                LawyerFullname = "ABC",
                LawyerEmail = "abc@xyz.com",
                LawyerAddress = "Earth",
                LawyerPhone = "1234"
            };
            return result;
        }
        private static SqlParamsModel GetParams_RetrieveAsync(int lawyerId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_lawyers.p_get_lawyer_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_lawyer_id", lawyerId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrieveAsync_Valid()
        {
            // Arrange
            int lawyerId = 1;
            SqlParamsModel queryParams = GetParams_RetrieveAsync(lawyerId);
            LawyerDetailsModel expected = GetSampleData_RetrieveAsync();
            _mockDataAccess.QueryFirstOrDefaultAsync<LawyerDetailsModel>(default).ReturnsForAnyArgs(expected);

            // Act
            LawyerDetailsModel actual = await _sut.RetrieveAsync(lawyerId);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<LawyerDetailsModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.LawyerId, actual.LawyerId);
            Assert.Equal(expected.LawyerFullname, actual.LawyerFullname);
            Assert.Equal(expected.LawyerEmail, actual.LawyerEmail);
            Assert.Equal(expected.LawyerAddress, actual.LawyerAddress);
            Assert.Equal(expected.LawyerPhone, actual.LawyerPhone);
        }

        private static SqlParamsModel GetParams_UpdateAsync(LawyerDetailsModel lawyerModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_lawyers.p_update_lawyer",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_lawyer_id", lawyerModel.LawyerId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_lawyer_email", lawyerModel.LawyerEmail, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_lawyer_fullname", lawyerModel.LawyerFullname, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_lawyer_phone", lawyerModel.LawyerPhone, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_lawyer_address", lawyerModel.LawyerAddress, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task UpdateAsync_Valid()
        {
            // Arrange
            LawyerDetailsModel lawyerModel = GetSampleData_RetrieveAsync();
            SqlParamsModel queryParams = GetParams_UpdateAsync(lawyerModel);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.UpdateAsync(lawyerModel);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_DeleteAsync(int lawyerId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_lawyers.p_delete_lawyer",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_lawyer_id", lawyerId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task DeleteAsync_Valid()
        {
            // Arrange
            int lawyerId = 1;
            SqlParamsModel queryParams = GetParams_DeleteAsync(lawyerId);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.DeleteAsync(lawyerId);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }
    }
}
