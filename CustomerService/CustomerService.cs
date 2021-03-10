namespace CustomerServiceNS
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces;

    public class CustomerService : ICustomerService
    {
        private readonly Database.ICustomerDataStore dataStore;

        public CustomerService(Database.ICustomerDataStore dataStore)
        {
            this.dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        }

        public async Task<bool> AddCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            try
            {
                var customerGetOptions = new Database.CustomerGetOptions()
                {
                    EmailAddress = customer.EmailAddress
                };

                if (!await this.dataStore.CheckCustomerExistsAsync(customerGetOptions, cancellationToken).ConfigureAwait(false))
                {
                    // Check here in case different entry points
                    // are used at a later date.
                    if (!customer.IsValid)
                    {
                        // Maybe return something better than
                        // bool in future to indicate the fault.
                        return false;
                    }

                    await this.dataStore.AddCustomerAsync(customer, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                // To do: Log exception
                throw;
            }

            return true;
        }

        public async Task<Customer> GetCustomerAsync(Guid customerId, CancellationToken cancellationToken)
        {
            try
            {
                var customerGetOptions = new Database.CustomerGetOptions() 
                { 
                    CustomerId = customerId 
                }; 

                return await dataStore.GetCustomerAsync(customerGetOptions, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // To do: Log exception
                throw;
            }
        }

        public async Task<bool> DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken)
        {
            try
            {
                var customerGetOptions = new Database.CustomerGetOptions()
                {
                    CustomerId = customerId
                };

                if (await this.dataStore.CheckCustomerExistsAsync(customerGetOptions, cancellationToken).ConfigureAwait(false))
                {
                    await this.dataStore.DeleteCustomerAsync(customerId, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                // To do: Log exception
                throw;
            }

            return true;
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync(bool includeInactive, CancellationToken cancellationToken)
        {
            try
            {
                return await dataStore.GetCustomersAsync(includeInactive, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // To do: Log exception
                throw;
            }
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            try
            {
                var customerGetOptions = new Database.CustomerGetOptions()
                {
                    CustomerId = customer.CustomerId
                };

                if (await this.dataStore.CheckCustomerExistsAsync(customerGetOptions, cancellationToken).ConfigureAwait(false))
                {
                    // Check here if the customer is valid as
                    // there may be other entry points later.
                    if (!customer.IsValid)
                    {
                        return false;
                    }

                    await this.dataStore.UpdateCustomerAsync(customer, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                // To do: Log exception
                throw;
            }

            return true;
        }
    }
}
