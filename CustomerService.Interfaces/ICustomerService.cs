namespace CustomerServiceNS.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICustomerService
    {
        Task<bool> AddCustomerAsync(Customer customer, CancellationToken cancellationToken);

        Task<Customer> GetCustomerAsync(Guid customerId, CancellationToken token);

        Task<IEnumerable<Customer>> GetCustomersAsync(bool includeInactive, CancellationToken cancellationToken);

        Task<bool> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken);

        Task<bool> DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken);
    }
}
