using CRM.API.Client.Service.Data.Extensions;
using CRM.API.Client.Service.Data.Models.Request;
using CRM.Common.Services.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Primitives;

namespace CRM.API.Client.Service.Data.Mappers
{
    public class PaginationParametersMapper(IValidator<PaginationParameters> validator) : IMapper<IDictionary<string, StringValues>, PaginationParameters>
    {
        public PaginationParameters Map(IDictionary<string, StringValues> keyValues)
        {
            var result = new PaginationParameters
            {
                PageAfter = keyValues.TryGetIntValueOrDefault("page_after", 0),
                PageSize = keyValues.TryGetIntValueOrDefault("page_size", 25)
            };

            validator.ValidateAndThrow(result);

            return result;
        }
      
    }
}