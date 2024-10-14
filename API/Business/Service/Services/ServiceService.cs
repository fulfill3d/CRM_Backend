using CRM.API.Business.Service.Data.Database;
using CRM.API.Business.Service.Data.Models;
using CRM.API.Business.Service.Services.Interfaces;
using CRM.Common.Database.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Business.Service.Services
{
    public class ServiceService(ServiceContext dbContext) : IServiceService
    {

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
                        .Where(css => css.IsEnabled == true)
                        .Select(css => new ServiceCategoryViewModel
                        {
                            Id = css.ServiceCategory.Id,
                            Name = css.ServiceCategory.Name,
                            Description = css.ServiceCategory.Description
                        }).ToList(),
                    SubCategories = s.CategorizeStoreServices
                        .Where(css => css.IsEnabled == true)
                        .Select(cs => new ServiceSubCategoryViewModel { 
                            Id = cs.ServiceSubCategory.Id,
                            Name = cs.ServiceSubCategory.Name, 
                            Description = cs.ServiceSubCategory.Description 
                        }).ToList() 
                }).ToListAsync();
    
            return services;
        }

        public async Task<bool> AddStoreService(string businessRefId, int storeId, StoreServiceRequest request)
        {
            var store = await dbContext.Stores
                .FirstOrDefaultAsync(s =>
                    s.IsEnabled == true &&
                    s.BusinessRefId == Guid.Parse(businessRefId) && 
                    s.Id == storeId);

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
            // Parse the business reference ID
            if (!Guid.TryParse(businessRefId, out var businessGuid))
            {
                return false; // Invalid GUID format
            }

            // Fetch the store service and include current categorization data
            var service = await dbContext.StoreServices
                .Include(ss => ss.CategorizeStoreServices) // Include existing categorizations
                .FirstOrDefaultAsync(ss =>
                    ss.IsEnabled &&
                    ss.BusinessRefId == businessGuid &&
                    ss.Id == request.Id);

            if (service == null) return false;

            // Update service fields
            service.UpdatedAt = DateTime.UtcNow;
            service.Name = request.Name;
            service.Description = request.Description;
            service.Duration = request.Duration;
            service.Price = request.Price;

            // Step 1: Ensure all provided categories and subcategories exist and are enabled
            var existingCategories = await dbContext.ServiceCategories
                .Where(sc => request.Categories.Contains(sc.Id) && sc.IsEnabled)
                .Select(sc => sc.Id)
                .ToListAsync();

            var existingSubCategories = await dbContext.ServiceSubCategories
                .Where(ssc => request.SubCategories.Contains(ssc.Id) && ssc.IsEnabled)
                .Select(ssc => ssc.Id)
                .ToListAsync();

            // Validate existence of categories and subcategories
            if (existingCategories.Count != request.Categories.Count() || existingSubCategories.Count != request.SubCategories.Count())
            {
                return false; // Bad request if any of the categories/subcategories are invalid
            }

            // Step 2: Disable all existing CategorizeStoreService records using ExecuteUpdateAsync (EF Core 7+)
            await dbContext.CategorizeStoreServices
                .Where(c => c.StoreServiceId == service.Id && c.IsEnabled)
                .ExecuteUpdateAsync(setter => setter.SetProperty(c => c.IsEnabled, false));

            // Step 3: Create new categorization entries for the incoming request
            var newCategorizeStoreServices = request.Categories
                .SelectMany(catId => request.SubCategories, (catId, subId) => new CategorizeStoreService
                {
                    StoreServiceId = service.Id,
                    ServiceCategoryId = catId,
                    ServiceSubCategoryId = subId,
                    IsEnabled = true
                })
                .ToList();

            // Bulk insert the new category/subcategory combinations
            await dbContext.CategorizeStoreServices.AddRangeAsync(newCategorizeStoreServices);

            // Save changes
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
    }
}