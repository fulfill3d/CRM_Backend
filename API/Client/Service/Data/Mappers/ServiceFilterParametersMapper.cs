using CRM.API.Client.Service.Data.Models.Request;
using CRM.Common.Services.Interfaces;
using Microsoft.Extensions.Primitives;

namespace CRM.API.Client.Service.Data.Mappers
{
    public class ServiceFilterParametersMapper : IMapper<IDictionary<string, StringValues>, ServiceFilterParameters>
    {
        public ServiceFilterParameters Map(IDictionary<string, StringValues> queryParameters)
        {
            var serviceFilterParameters = new ServiceFilterParameters();

            // Map Query parameter
            if (queryParameters.TryGetValue("query", out var query))
            {
                serviceFilterParameters.Query = query.FirstOrDefault();
            }

            // Map Radius parameter
            if (queryParameters.TryGetValue("radius", out var radiusString) && double.TryParse(radiusString.FirstOrDefault(), out double radius))
            {
                serviceFilterParameters.Radius = radius;
            }

            // Map Lat parameter
            if (queryParameters.TryGetValue("lat", out var latString) && decimal.TryParse(latString.FirstOrDefault(), out decimal lat))
            {
                serviceFilterParameters.Lat = lat;
            }

            // Map Lon parameter
            if (queryParameters.TryGetValue("lon", out var lonString) && decimal.TryParse(lonString.FirstOrDefault(), out decimal lon))
            {
                serviceFilterParameters.Lon = lon;
            }

            // Map CategoryIds parameter
            if (queryParameters.TryGetValue("categoryId", out var categoryIdsString))
            {
                var categoryIds = categoryIdsString.FirstOrDefault()?.Split(',')
                    .Where(id => int.TryParse(id, out _))
                    .Select(int.Parse)
                    .ToList();

                if (categoryIds != null && categoryIds.Count > 0)
                {
                    serviceFilterParameters.CategoryIds = categoryIds;
                }
            }

            // Map SubCategoryIds parameter
            if (queryParameters.TryGetValue("subcategoryId", out var subCategoryIdsString))
            {
                var subCategoryIds = subCategoryIdsString.FirstOrDefault()?.Split(',')
                    .Where(id => int.TryParse(id, out _))
                    .Select(int.Parse)
                    .ToList();

                if (subCategoryIds != null && subCategoryIds.Count > 0)
                {
                    serviceFilterParameters.SubCategoryIds = subCategoryIds;
                }
            }

            return serviceFilterParameters;
        }
    }

}