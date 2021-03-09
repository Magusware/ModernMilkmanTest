namespace CustomerServiceNS.Database
{
    public interface ICustomerServiceDatabaseMapper
    {
        T Map<T>(object obj);
    }
}