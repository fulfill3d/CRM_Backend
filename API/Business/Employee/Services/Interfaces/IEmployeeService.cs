using CRM.API.Business.Employee.Data.Models;

namespace CRM.API.Business.Employee.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<StoreEmployeeViewModel>> GetEmployees(string businessRefId, int storeId);
        Task<bool> AddEmployee(string businessRefId, int storeId, EmployeeRequest request);
        Task<bool> EditEmployee(string businessRefId, EmployeeRequest request);
        Task<bool> DeleteEmployee(string businessRefId, int employeeId);
    }
}