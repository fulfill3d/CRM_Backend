using CRM.Common.Database;
using CRM.Common.DI;
using Microsoft.Extensions.DependencyInjection;
using CRM.API.Client.Service.Data.Database;
using CRM.API.Client.Service.Data.Mappers;
using CRM.API.Client.Service.Data.Models.Request;
using CRM.API.Client.Service.Services;
using CRM.API.Client.Service.Services.Interfaces;
using CRM.API.Client.Service.Services.Options;
using CRM.Common.Services.Interfaces;
using Microsoft.Extensions.Primitives;

namespace CRM.API.Client.Service
{
    public static class DepInj
    {
        public static void RegisterServices(
            this IServiceCollection services,
            DatabaseOption dbOption)
        {
            #region Miscellaneous

            services.AddDatabaseContext<ServiceContext>(dbOption);
            
            services.AddTransient<
                IMapper<IDictionary<string, StringValues>, ServiceFilterParameters>, 
                ServiceFilterParametersMapper>();
            
            services.AddTransient<
                IMapper<IDictionary<string, StringValues>, PaginationParameters>, 
                PaginationParametersMapper>();
            
            services.AddFluentValidator<ServiceFilterParameters>();
            services.AddFluentValidator<PaginationParametersMapper>();

            #endregion

            #region Services

            services.AddTransient<IServiceService, ServiceService>();

            #endregion
        }
    }
}