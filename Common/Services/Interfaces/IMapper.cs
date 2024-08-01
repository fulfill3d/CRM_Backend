namespace CRM.Common.Services.Interfaces
{
    public interface IMapper<in T1, out T2>
        where T1 : class
        where T2 : class
    {
        T2 Map(T1 obj);
    }
}