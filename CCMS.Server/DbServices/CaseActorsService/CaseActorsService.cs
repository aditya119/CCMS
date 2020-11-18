using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CCMS.Server.DbDataAccess;
using CCMS.Shared.Models.CaseActorModels;
using Dapper.Oracle;

namespace CCMS.Server.DbServices
{
    public class CaseActorsService : ICaseActorsService
    {
        private readonly IOracleDataAccess _dataAccess;

        public CaseActorsService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task UpdateAsync(IEnumerable<UpdateCaseActorModel> caseActorModels, int currUser)
        {
            var executeSqlModels = new List<SqlParamsModel>();
            foreach (var model in caseActorModels)
            {
                var sqlModel = new SqlParamsModel
                {
                    Sql = "pkg_case_actors.p_update_case_actors",
                    Parameters = new OracleDynamicParameters()
                };
                sqlModel.Parameters.Add("pi_case_id", model.CaseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_actor_type_id", model.ActorTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_actor_name", model.ActorName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_actor_address", model.ActorAddress, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_actor_email", model.ActorEmail, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_actor_phone", model.ActorPhone, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_detail_file", model.DetailFile, dbType: OracleMappingType.Int32, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);

                executeSqlModels.Add(sqlModel);
            }

            await _dataAccess.ExecuteTransactionAsync(executeSqlModels);
        }

        public async Task<IEnumerable<CaseActorModel>> RetrieveAsync(int caseId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<CaseActorModel>("pkg_case_actors.p_get_all_case_actors", parameters);
        }
    }
}
