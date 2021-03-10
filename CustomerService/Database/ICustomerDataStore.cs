namespace CustomerServiceNS.Database
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICustomerDataStore
    {
        Task<IEnumerable<Interfaces.Customer>> GetCustomersAsync(bool includeInactive, CancellationToken cancellationToken);

        Task<Interfaces.Customer> GetCustomerAsync(CustomerGetOptions options, CancellationToken cancellationToken);

        Task AddCustomerAsync(Interfaces.Customer customer, CancellationToken cancellationToken);

        Task UpdateCustomerAsync(Interfaces.Customer customer, CancellationToken cancellationToken);

        Task DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken);

        Task<bool> CheckCustomerExistsAsync(CustomerGetOptions options, CancellationToken cancellationToken);
    }
}
