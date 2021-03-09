namespace CustomerService.Database
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICustomerDataStore
    {
        Task<IEnumerable<Interfaces.Customer>> GetCustomersAsync(bool includeInactive, CancellationToken cancellationToken);

        Task<Interfaces.Customer> GetCustomerAsync(object customerId, CancellationToken token);

        Task AddCustomerAsync(Interfaces.Customer customer, CancellationToken cancellationToken);

        Task UpdateCustomerAsync(Interfaces.Customer customer, CancellationToken cancellationToken);

        Task DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken);

        Task AddAddressAsync(Interfaces.Address address, Guid customerId, CancellationToken cancellationToken);

        Task UpdateAddressAsync(Interfaces.Address address, CancellationToken cancellationToken);

        Task DeleteAddressAsync(Guid addressId, CancellationToken cancellationToken);
    }
}
