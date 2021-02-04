using CCMS.Server.Services.DbDataAccessService;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OracleServiceCollectionExtensions
    {
        public static IServiceCollection AddOracleDataAccessService(
             this IServiceCollection services, string connectionString)
        {
            services.AddTransient<IDbConnection>((sp) => new OracleConnection(connectionString));

            services.AddScoped<IOracleDataAccess, OracleDataAccess>();
            return services;
        }
    }
}
