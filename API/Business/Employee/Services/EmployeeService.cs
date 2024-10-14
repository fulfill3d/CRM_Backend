using CRM.API.Business.Employee.Data.Database;
using CRM.API.Business.Employee.Data.Models;
using CRM.API.Business.Employee.Services.Interfaces;
using CRM.Common.Database.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Business.Employee.Services
{
    public class EmployeeService(EmployeeContext dbContext) : IEmployeeService
    {
        public async Task<List<StoreEmployeeViewModel>> GetEmployees(string businessRefId, int storeId)
        {
            var employees = await dbContext.StoreEmployees
                .Include(se => se.Store)
                .Where(se => 
                    se.IsEnabled == true && 
                    se.Store.Id == storeId &&
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
    }
}