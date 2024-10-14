using CRM.API.Business.Store.Data.Database;
using CRM.API.Business.Store.Data.Models;
using CRM.API.Business.Store.Services.Interfaces;
using CRM.Common.Database.Data;
using CRM.Integrations.GoogleMapsClient.Interfaces;
using CRM.Integrations.GoogleMapsClient.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CRM.API.Business.Store.Services
{
    public class StoreService(StoreContext dbContext, IGoogleMapsClient googleMapsClient) : IStoreService
    {
        public async Task<List<StoreViewModel>> GetStores(string businessRefId)
        {
            var stores = await dbContext.Stores.Where(s =>
                    s.IsEnabled == true &&
                    s.BusinessRefId == Guid.Parse(businessRefId))
                .Select(s => new StoreViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                }).ToListAsync();
            
            return stores;
        }

        public async Task<StoreViewModel?> GetStore(string businessRefId, int storeId)
        {
            var store = await dbContext.Stores
                .Include(s => s.StoreEmployees)
                .Include(s => s.StoreServices)
                .Include(s => s.StoreLocations)
                .ThenInclude(sl => sl.Address)
                .Where(s =>
                    s.IsEnabled == true &&
                    s.BusinessRefId == Guid.Parse(businessRefId) && 
                    s.Id == storeId)
                .Select(s => new StoreViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    Location = s.StoreLocations.Select(sl => new StoreLocationViewModel
                    {
                        LocationId = sl.Id,
                        AddressId = sl.Address.Id,
                        CreatedAt = sl.CreatedAt,
                        UpdatedAt = sl.UpdatedAt,
                        LocationName = sl.Name,
                        Longitude = (decimal)sl.Location.Coordinate.X,
                        Latitude = (decimal)sl.Location.Coordinate.Y,
                        AddressFirstName = sl.Address.FirstName,
                        AddressLastName = sl.Address.LastName,
                        Street1 = sl.Address.Street1,
                        Street2 = sl.Address.Street2,
                        City = sl.Address.City,
                        State = sl.Address.State,
                        Country = sl.Address.Country,
                        ZipCode = sl.Address.ZipCode,
                    }).FirstOrDefault() ?? new StoreLocationViewModel()
                }).FirstOrDefaultAsync();

            return store;
        }

        public async Task<bool> AddStore(string businessRefId, StoreRequest request)
        {
            var address = new Address
            {
                FirstName = request.Address.FirstName,
                LastName = request.Address.LastName,
                Street1 = request.Address.Street1,
                Street2 = request.Address.Street2,
                City = request.Address.City,
                State = request.Address.State,
                Country = request.Address.Country,
                ZipCode = request.Address.ZipCode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsEnabled = true
            };
            
            var geoLocation = await GetTopologyPointByAddress(request.Address);
            
            var storeLocation = new StoreLocation
            {
                Name = request.Address.LocationName,
                Location = geoLocation,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsEnabled = true,
                Address = address
            };
            
            var store = new Common.Database.Data.Store
            {
                Name = request.Name,
                Description = request.Description,
                BusinessRefId = Guid.Parse(businessRefId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsEnabled = true,
                StoreLocations = new List<StoreLocation> { storeLocation }
            };

            var business = await dbContext.Businesses
                .FirstOrDefaultAsync(b =>
                    b.IsEnabled == true && b.RefId ==
                    Guid.Parse(businessRefId));

            if (business == null) return false;
            
            business.Stores.Add(store);

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditStore(string businessRefId, StoreRequest request)
        {
            var store = await dbContext.Stores
                .Include(s => s.StoreLocations)
                .ThenInclude(sl => sl.Address)
                .FirstOrDefaultAsync(s =>
                    s.IsEnabled == true &&
                    s.BusinessRefId == Guid.Parse(businessRefId) &&
                    s.Id == request.Id);

            if (store == null) return false;

            store.Name = request.Name;
            store.Description = request.Description;
            store.UpdatedAt = DateTime.UtcNow;
            var location = store.StoreLocations.FirstOrDefault(); // We have only one location per store

            if (location == null) return false;
            
            location.Name = request.Address.LocationName;
            location.UpdatedAt = DateTime.UtcNow;
            
            location.Address.FirstName = request.Address.FirstName;
            location.Address.LastName = request.Address.LastName;
            location.Address.Street1 = request.Address.Street1;
            location.Address.Street2 = request.Address.Street2;
            location.Address.City = request.Address.City;
            location.Address.State = request.Address.State;
            location.Address.Country = request.Address.Country;
            location.Address.ZipCode = request.Address.ZipCode;
            location.Address.UpdatedAt = DateTime.UtcNow;
            
            var geoLocation = await GetTopologyPointByAddress(request.Address);
            location.Location = geoLocation;

            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStore(string businessRefId, int storeId)
        {
            var store = await dbContext.Stores
                .Include(s => s.StoreLocations)
                .ThenInclude(sl => sl.Address)
                .FirstOrDefaultAsync(s =>
                    s.IsEnabled == true &&
                    s.BusinessRefId == Guid.Parse(businessRefId) &&
                    s.Id == storeId);

            if (store == null) return false;

            store.IsEnabled = false;
            store.UpdatedAt = DateTime.UtcNow;
            
            var location = store.StoreLocations.FirstOrDefault(); // We have only one location per store

            if (location == null) return false;

            location.IsEnabled = false;
            location.UpdatedAt = DateTime.UtcNow;
            location.Address.IsEnabled = false;
            location.Address.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
            return true;
        }
        
        private async Task<Point> GetTopologyPointByAddress(AddressRequest address)
        {
            var geometryFactory = new GeometryFactory();
            
            var response = await googleMapsClient.GetCoordinatesAsync(new GeocodingRequest
            {
                Address = address.ToString()
            });
            
            var point = geometryFactory.CreatePoint(new Coordinate(response?.Longitude ?? 0, response?.Latitude ?? 0));
            point.SRID = 4326;

            return point;
        }
    }
}