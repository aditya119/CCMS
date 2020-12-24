using CCMS.Server.Services.DbServices;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbServiceCollectionExtensions
    {
        public static IServiceCollection AddDbServices(
             this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPlatformsService, PlatformsService>();
            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<IAppUsersService, AppUsersService>();

            services.AddScoped<ILawyersService, LawyersService>();
            services.AddScoped<ICourtsService, CourtsService>();
            services.AddScoped<ICaseTypesService, CaseTypesService>();
            services.AddScoped<ILocationsService, LocationsService>();

            services.AddScoped<IProceedingDecisionsService, ProceedingDecisionsService>();
            services.AddScoped<IActorTypesService, ActorTypesService>();

            services.AddScoped<IAttachmentsService, AttachmentsService>();

            services.AddScoped<ICourtCasesService, CourtCasesService>();
            services.AddScoped<ICaseDatesService, CaseDatesService>();
            services.AddScoped<ICaseActorsService, CaseActorsService>();
            services.AddScoped<ICaseProceedingsService, CaseProceedingsService>();
            services.AddScoped<IInsightsService, InsightsService>();

            return services;
        }
    }
}
