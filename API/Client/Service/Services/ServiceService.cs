using System.Linq.Expressions;
using CRM.API.Client.Service.Data.Database;
using CRM.API.Client.Service.Data.Helpers;
using CRM.API.Client.Service.Data.Models.Request;
using CRM.API.Client.Service.Data.Models.Response;
using CRM.API.Client.Service.Services.Interfaces;
using CRM.Common.Database.Data;
using CRM.Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using NetTopologySuite.Geometries;

namespace CRM.API.Client.Service.Services
{
    public class ServiceService(
        IMapper<IDictionary<string, StringValues>, ServiceFilterParameters> serviceFilterParametersMapper,
        IMapper<IDictionary<string, StringValues>, PaginationParameters> paginationFilterParametersMapper,
        ServiceContext dbContext) : IServiceService
    {
        private const double MetersPerMile = 1609.34;
        public async Task<List<ServiceViewModel>> GetServices(Dictionary<string, StringValues> parameters)
        {
            var serviceFilters = serviceFilterParametersMapper.Map(parameters);
            var paginationFilters = paginationFilterParametersMapper.Map(parameters);

            #region Check Cache
            // TODO: RedisCache
            #endregion

            var predicate = BuildServicePredicate(serviceFilters);
            
            var storeServices = await dbContext.StoreServices
                .Include(ss => ss.CategorizeStoreServices)
                .Include(ss => ss.Store)
                .Where(predicate)
                .OrderBy(ss => ss.Id) // Order by a unique column (e.g., Id) for consistent pagination
                .Skip(paginationFilters.PageAfter)
                .Take(paginationFilters.PageSize)
                .Select(ss => new ServiceViewModel
                {
                    Id = ss.Id,
                    Price = ss.Price ?? decimal.Zero,
                    Duration = ss.Duration ?? 0,
                    Name = ss.Name,
                    Description = ss.Description,
                    Categories = ss.CategorizeStoreServices
                        .GroupBy(cs => new
                        {
                            cs.ServiceCategory.Id, 
                            cs.ServiceCategory.Name, 
                            cs.ServiceCategory.Description
                        })
                        .Select(g => new ServiceCategoryViewModel
                        {
                            Id = g.Key.Id,
                            Name = g.Key.Name,
                            Description = g.Key.Description,
                            SubCategories = g.Select(cs => new ServiceSubCategoryViewModel
                            {
                                Id = cs.ServiceSubCategory.Id,
                                Name = cs.ServiceSubCategory.Name,
                                Description = cs.ServiceSubCategory.Description
                            }).ToList()
                        }).ToList()
                })
                .ToListAsync();
            
            #region Write Cache
            // TODO: RedisCache
            #endregion

            return storeServices;
        }

        public async Task<ServiceDetailViewModel?> GetService(int serviceId)
        {
            var service = await dbContext.StoreServices
                .Include(ss => ss.Store)
                .ThenInclude(s => s.StoreLocations)
                .ThenInclude(sl => sl.Address)
                .Include(ss => ss.Store)
                .ThenInclude(s => s.StoreEmployees)
                .Where(ss => ss.IsEnabled && ss.Id == serviceId)
                .Select(ss => new ServiceDetailViewModel
                {
                    ServiceId = ss.Id,
                    ServiceDuration = ss.Duration ?? 0,
                    ServicePrice = ss.Price ?? decimal.Zero,
                    ServiceName = ss.Name,
                    ServiceDescription = ss.Description,
                    StoreId = ss.Store.Id,
                    StoreName = ss.Store.Name,
                    StoreDescription = ss.Store.Description,
                    Lon = ss.Store.StoreLocations.Select(sl => sl.Location.Coordinate.X).FirstOrDefault(),
                    Lat = ss.Store.StoreLocations.Select(sl => sl.Location.Coordinate.Y).FirstOrDefault(),
                    Street1 = ss.Store.StoreLocations.Select(sl => sl.Address.Street1).FirstOrDefault() ?? string.Empty,
                    Street2 = ss.Store.StoreLocations.Select(sl => sl.Address.Street1).FirstOrDefault(),
                    City = ss.Store.StoreLocations.Select(sl => sl.Address.City).FirstOrDefault() ?? string.Empty,
                    State = ss.Store.StoreLocations.Select(sl => sl.Address.State).FirstOrDefault() ?? string.Empty,
                    Country = ss.Store.StoreLocations.Select(sl => sl.Address.Country).FirstOrDefault() ?? string.Empty,
                    ZipCode = ss.Store.StoreLocations.Select(sl => sl.Address.ZipCode).FirstOrDefault() ?? string.Empty,
                    StoreEmployees = ss.Store.StoreEmployees.Select(se => new StoreEmployeeViewModel
                    { 
                        Id = se.Id,
                        NickName = se.NickName,
                    }).ToList(),
                }).FirstOrDefaultAsync();

            return service;
        }

        private Expression<Func<StoreService, bool>> BuildServicePredicate(ServiceFilterParameters serviceFilters)
        {
            var predicate = PredicateBuilder.True<StoreService>();
            
            // Filter by location
            if (serviceFilters.Lat.HasValue && serviceFilters.Lon.HasValue)
            {
                var userLocation = new Point((double)serviceFilters.Lat.Value, (double)serviceFilters.Lon.Value) { SRID = 4326 };
                
                var radiusInMiles = serviceFilters.Radius.GetValueOrDefault(5); // Radius in miles
                var radiusInMeters = radiusInMiles * MetersPerMile;
                
                predicate = predicate.And(ss => ss.Store.StoreLocations
                    .Any(sl => sl.Location.Distance(userLocation) <= radiusInMeters));
            }
            // Filter by category
            if (serviceFilters.CategoryIds.Count > 0)
            {
                predicate = predicate.And(ss =>
                    ss.CategorizeStoreServices
                        .Any(css => css.IsEnabled == true && 
                                    serviceFilters.CategoryIds.Contains(css.ServiceCategoryId)));
            }
            
            // Filter by subcategory
            if (serviceFilters.SubCategoryIds.Count > 0)
            {
                predicate = predicate.And(ss => 
                    ss.CategorizeStoreServices.
                        Any(css => css.IsEnabled == true && 
                                   serviceFilters.SubCategoryIds.Contains(css.ServiceSubCategoryId)));
            }

            return predicate;
        }
    }
}