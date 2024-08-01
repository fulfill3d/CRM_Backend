using CRM.Common.Database.Data;
using CRM.API.Business.Management.Data.Database;
using CRM.API.Business.Management.Data.Models.Request;
using CRM.API.Business.Management.Data.Models.Response;
using CRM.API.Business.Management.Services.Interfaces;
using CRM.Integrations.GoogleMapsClient.Interfaces;
using CRM.Integrations.GoogleMapsClient.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CRM.API.Business.Management.Services
{
    public class ManagementService(ManagementContext dbContext, IGoogleMapsClient googleMapsClient)
        : IManagementService
    {
        // STORE
        
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
                    }).FirstOrDefault() ?? new StoreLocationViewModel(),
                    Employees = s.StoreEmployees.Select(se => new StoreEmployeeViewModel
                    {
                        Id = se.Id,
                        NickName = se.NickName,
                        FirstName = se.FirstName,
                        LastName = se.LastName,
                        Email = se.Email,
                        Phone = se.Phone,
                        CreatedAt = se.CreatedAt,
                        UpdatedAt = se.UpdatedAt,
                    }).ToList()
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
            
            var store = new Store
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
        
        // EMPLOYEE

        public async Task<List<StoreEmployeeViewModel>> GetEmployees(string businessRefId)
        {
            var employees = await dbContext.StoreEmployees
                .Include(se => se.Store)
                .Where(se => 
                    se.IsEnabled == true && 
                    se.Store.BusinessRefId == Guid.Parse(businessRefId))
                .Select(se => new StoreEmployeeViewModel
                {
                    Id = se.Id,
                    NickName = se.NickName,
                    FirstName = se.FirstName,
                    LastName = se.LastName,
                    Email = se.Email,
                    Phone = se.Phone,
                    CreatedAt = se.CreatedAt,
                    UpdatedAt = se.UpdatedAt,
                })
                .ToListAsync();

            return employees;
        }

        public async Task<bool> AddEmployee(string businessRefId, int storeId, EmployeeRequest request)
        {
            var employee = new StoreEmployee
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsEnabled = true,
                NickName = request.NickName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
            };

            var store = await dbContext.Stores
                .FirstOrDefaultAsync(s =>
                    s.IsEnabled == true && 
                    s.Id == storeId &&
                    s.BusinessRefId == Guid.Parse(businessRefId));

            if (store == null) return false;
            
            store.StoreEmployees.Add(employee);

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditEmployee(string businessRefId, EmployeeRequest request)
        {
            var employee = await dbContext.StoreEmployees
                .Include(se => se.Store)
                .FirstOrDefaultAsync(se => 
                    se.IsEnabled == true && 
                    se.Id == request.Id && 
                    se.Store.BusinessRefId == Guid.Parse(businessRefId));

            if (employee == null) return false;

            employee.UpdatedAt = DateTime.UtcNow;
            employee.NickName = request.NickName;
            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Phone = request.Phone;
            employee.Email = request.Email;

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteEmployee(string businessRefId, int employeeId)
        {
            var employee = await dbContext.StoreEmployees
                .Include(se => se.Store)
                .FirstOrDefaultAsync(se => 
                    se.IsEnabled == true && 
                    se.Id == employeeId && 
                    se.Store.BusinessRefId == Guid.Parse(businessRefId));

            if (employee == null) return false;
            
            employee.UpdatedAt = DateTime.UtcNow;
            employee.IsEnabled = false;

            await dbContext.SaveChangesAsync();

            return true;
        }
        
        // STORE SERVICES

        public async Task<CategoryViewModel> GetServiceCategories()
        {
            var categories = await dbContext.ServiceCategories
                .Where(sc => sc.IsEnabled == true)
                .Select(sc => new CategoriesViewModel
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    Description = sc.Description
                })
                .ToListAsync();

            var subcategories = await dbContext.ServiceSubCategories
                .Where(sc => sc.IsEnabled == true)
                .Select(sc => new SubCategoriesViewModel
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    Description = sc.Description
                })
                .ToListAsync();

            var categoriesViewModel = new CategoryViewModel
            {
                Categories = categories,
                SubCategories = subcategories,
            };

            return categoriesViewModel;
        }

        public async Task<List<ServiceViewModel>> GetStoreServices(string businessRefId, int storeId)
        {
            var services = await dbContext.StoreServices
                .Include(s => s.Store)
                .Include(s => s.CategorizeStoreServices)
                .ThenInclude(cs => cs.ServiceCategory)
                .Include(s => s.CategorizeStoreServices)
                .ThenInclude(cs => cs.ServiceSubCategory)
                .Where(ss => 
                    ss.IsEnabled == true && 
                    ss.BusinessRefId == Guid.Parse(businessRefId) && 
                    ss.Store.Id == storeId)
                .Select(s => new ServiceViewModel
                {
                    Id = s.Id,
                    Price = s.Price ?? decimal.Zero,
                    Duration = s.Duration ?? 0,
                    Name = s.Name,
                    Description = s.Description,
                    Categories = s.CategorizeStoreServices
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
                }).ToListAsync();
    
            return services;
        }

        public async Task<bool> AddStoreService(string businessRefId, int storeId, StoreServiceRequest request)
        {
            var store = await dbContext.Stores
                .FirstOrDefaultAsync(s =>
                    s.IsEnabled == true &&
                    s.BusinessRefId == Guid.Parse(businessRefId));

            if (store == null) return false;

            // Ensure all categories and subcategories exist
            var existingCategories = await dbContext.ServiceCategories
                .Where(sc => request.Categories.Contains(sc.Id) && sc.IsEnabled == true)
                .Select(sc => sc.Id)
                .ToListAsync();

            var existingSubCategories = await dbContext.ServiceSubCategories
                .Where(ssc => request.SubCategories.Contains(ssc.Id) && ssc.IsEnabled == true)
                .Select(ssc => ssc.Id)
                .ToListAsync();

            if (existingCategories.Count != request.Categories.Count() || existingSubCategories.Count != request.SubCategories.Count())
            {
                return false;
            }

            var categorizeStoreServices = new List<CategorizeStoreService>();
            
            foreach (var categoryId in request.Categories)
            {
                foreach (var subCategoryId in request.SubCategories)
                {
                    categorizeStoreServices.Add(new CategorizeStoreService
                    {
                        ServiceCategoryId = categoryId,
                        ServiceSubCategoryId = subCategoryId,
                        IsEnabled = true
                    });
                }
            }

            var storeService = new StoreService
            {
                Name = request.Name,
                Description = request.Description,
                Duration = request.Duration,
                Price = request.Price,
                BusinessRefId = Guid.Parse(businessRefId),
                StoreId = storeId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsEnabled = true,
                CategorizeStoreServices = categorizeStoreServices
            };

            store.StoreServices.Add(storeService);

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditStoreService(string businessRefId, StoreServiceRequest request)
        {
            var service = await dbContext.StoreServices
                .FirstOrDefaultAsync(ss =>
                    ss.IsEnabled == true &&
                    ss.BusinessRefId == Guid.Parse(businessRefId) &&
                    ss.Id == request.Id);

            if (service == null) return false;
            
            service.UpdatedAt = DateTime.UtcNow;
            service.Name = request.Name;
            service.Description = request.Description;
            service.Duration = request.Duration;
            service.Price = request.Price;
            
            // TODO Edit Store Service Category
            // // Ensure all categories and subcategories exist
            // var existingCategories = await _dbContext.ServiceCategories
            //     .Where(sc => request.Categories.Contains(sc.Id) && sc.IsEnabled == true)
            //     .Select(sc => sc.Id)
            //     .ToListAsync();
            //
            // var existingSubCategories = await _dbContext.ServiceSubCategories
            //     .Where(ssc => request.SubCategories.Contains(ssc.Id) && ssc.IsEnabled == true)
            //     .Select(ssc => ssc.Id)
            //     .ToListAsync();
            //
            // if (existingCategories.Count != request.Categories.Count() || existingSubCategories.Count != request.SubCategories.Count())
            // {
            //     return new BadRequestObjectResult("One or more categories or subcategories do not exist or are not enabled.");
            // }
            //
            // // REMOVE IF NOT EXISTS ANYMORE
            // var categoryIds = request.Categories.ToList();
            // var subCategoryIds = request.SubCategories.ToList();
            //
            // foreach (var categorize in service.CategorizeStoreServices)
            // {
            //     if (!categoryIds.Contains(categorize.ServiceCategoryId) &&
            //         !subCategoryIds.Contains(categorize.ServiceSubCategoryId))
            //     {
            //         categorize.IsEnabled = false;
            //     }
            //     else
            //     {
            //         categorize.IsEnabled = true;
            //     }
            // }
            //
            // // ADD IF NEW CATEGORY/SUBCATEGORY
            // var existingCategoryIds = service.CategorizeStoreServices
            //     .Where(c => c.IsEnabled == true)
            //     .Select(c => c.ServiceCategoryId)
            //     .ToList();
            //
            // var existingSubCategoryIds = service.CategorizeStoreServices
            //     .Where(c => c.IsEnabled == true)
            //     .Select(c => c.ServiceSubCategoryId)
            //     .ToList();
            //
            // var newCategories = categoryIds.Except(existingCategoryIds).ToList();
            //
            // var newSubCategories = subCategoryIds.Except(existingSubCategoryIds).ToList();
            //
            // if (newCategories.Count != 0 || newSubCategories.Count != 0)
            // {
            //     var newCategorizeStoreServices = new List<CategorizeStoreService>();
            //
            //     foreach (var categoryId in newCategories)
            //     {
            //         foreach (var subCategoryId in newSubCategories)
            //         {
            //             newCategorizeStoreServices.Add(new CategorizeStoreService
            //             {
            //                 ServiceCategoryId = categoryId,
            //                 ServiceSubCategoryId = subCategoryId,
            //                 IsEnabled = true
            //             });
            //         }
            //     }
            //
            //     foreach (var categoryId in newCategories)
            //     {
            //         foreach (var subCategoryId in existingSubCategoryIds)
            //         {
            //             newCategorizeStoreServices.Add(new CategorizeStoreService
            //             {
            //                 ServiceCategoryId = categoryId,
            //                 ServiceSubCategoryId = subCategoryId,
            //                 IsEnabled = true
            //             });
            //         }
            //     }
            //
            //     foreach (var categoryId in existingCategoryIds)
            //     {
            //         foreach (var subCategoryId in newSubCategories)
            //         {
            //             newCategorizeStoreServices.Add(new CategorizeStoreService
            //             {
            //                 ServiceCategoryId = categoryId,
            //                 ServiceSubCategoryId = subCategoryId,
            //                 IsEnabled = true
            //             });
            //         }
            //     }
            //
            //     foreach (var storeService in newCategorizeStoreServices)
            //     {
            //         service.CategorizeStoreServices.Add(storeService);
            //     }
            // }

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteStoreService(string businessRefId, int storeServiceId)
        {
            var service = await dbContext.StoreServices
                .Include(ss => ss.CategorizeStoreServices)
                .FirstOrDefaultAsync(ss =>
                    ss.IsEnabled == true &&
                    ss.BusinessRefId == Guid.Parse(businessRefId) &&
                    ss.Id == storeServiceId);

            if (service == null) return false;

            service.UpdatedAt = DateTime.UtcNow;
            service.IsEnabled = false;

            foreach (var categorize in service.CategorizeStoreServices)
            {
                categorize.IsEnabled = false;
            }

            await dbContext.SaveChangesAsync();

            return true;
        }
        
        // PRIVATE METHODS
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