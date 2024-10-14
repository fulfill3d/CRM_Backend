using Microsoft.Extensions.DependencyInjection;
using CRM.API.Business.Employee.Data.Database;
using CRM.API.Business.Employee.Data.Models;
using CRM.API.Business.Employee.Services;
using CRM.API.Business.Employee.Services.Interfaces;
using CRM.Common.Services;
using CRM.Common.Services.Options;

namespace CRM.API.Business.Employee
{
    public static class DepInj
    {
        public static void RegisterServices(
            this IServiceCollection services,
            Common.Database.DatabaseOption dbOption,
            Action<TokenValidationOptions> tokenValidation,
            Action<AuthorizationScope> authorizationScope)
        {
            #region Miscellaneous
            
            services.ConfigureServiceOptions<TokenValidationOptions>((_, opt) => tokenValidation(opt));
            services.ConfigureServiceOptions<AuthorizationScope>((_, opt) => authorizationScope(opt));
            services.AddDatabaseContext<EmployeeContext>(dbOption);
            services.AddB2CJwtTokenValidator((_, opt) => tokenValidation(opt));
            
            #endregion
            
            #region Services
            
            services.AddTransient<IEmployeeService, EmployeeService>();
            
            #endregion
            
            #region MapperValidator
            
            services.AddFluentValidator<EmployeeRequest>();
            services.AddHttpRequestBodyMapper();
            
            #endregion
        }
    }
}