namespace WebAPI.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CustomerServiceNS.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService customerService;

        public string DefaultCountry { get; set; } = "UK";

        public CustomerController(ICustomerService customerService)
        {
            // In a real implementation, this would not be the service
            // but instead would be the bridge to the service. Either a 
            // proxy or something similar.
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        /// <summary>
        /// Gets a specific customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>
        /// BadRequest (400): customer id not supplied
        /// NotFound (404): customer not found
        /// Ok (200): customer found, includes customer object
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync(Guid customerId, CancellationToken cancellationToken)
        {
            if (customerId == Guid.Empty)
            {
                return this.BadRequest();
            }

            var customer = await this.customerService
                .GetCustomerAsync(customerId, cancellationToken)
                .ConfigureAwait(false);

            if (customer == null)
            {
                return this.NotFound();
            }

            return this.Ok(customer);
        }

        /// <summary>
        /// Gets all customers, including inactive ones
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var customers = await this.customerService.GetCustomersAsync(true, cancellationToken).ConfigureAwait(false);

            return this.Ok(customers);
        }

        /// <summary>
        /// Gets all the customers which are not set to inactive
        /// </summary>
        /// <returns></returns>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveAsync(CancellationToken cancellationToken)
        {
            var customers = await this.customerService.GetCustomersAsync(false, cancellationToken).ConfigureAwait(false);

            return this.Ok(customers);
        }
    
        /// <summary>
        /// Creates a new customer
        /// </summary>
        /// <param name="customer">Parameter from body of a request message.</param>
        /// <returns>
        /// BadRequest (400): Customer or address(es) are invalid
        /// Conflict (409): Customer already exists
        /// Ok (200): Customer created
        /// </returns>
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody]Customer customer, CancellationToken cancellationToken)
        {
            if (!customer.IsValid)
            {
                return this.BadRequest();
            }

            customer.CustomerId = Guid.NewGuid();
            customer.PrimaryAddress.AddressId = Guid.NewGuid();

            if (string.IsNullOrEmpty(customer.PrimaryAddress.Country))
            {
                customer.PrimaryAddress.Country = this.DefaultCountry;
            }

            if (customer.SecondaryAddresses != null)
            {
                foreach (var address in customer.SecondaryAddresses)
                {
                    address.AddressId = Guid.NewGuid();

                    if (string.IsNullOrEmpty(address.Country))
                    {
                        address.Country = this.DefaultCountry;
                    }
                }
            }

            if (!await this.customerService.AddCustomerAsync(customer, cancellationToken))
            {
                return this.Conflict();
            }

            return this.Ok(customer);
        }

        /// <summary>
        /// Deletes an existing customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>
        /// BadRequest (400): customer id is not valid
        /// NotFound (404): customer not found
        /// Ok (200): customer deleted
        /// </returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid customerId, CancellationToken cancellationToken)
        {
            if (customerId == Guid.Empty)
            {
                return this.BadRequest();
            }

            if (await this.customerService.DeleteCustomerAsync(customerId, cancellationToken).ConfigureAwait(false))
            {
                return this.NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// Adds an address into the system
        /// </summary>
        /// <param name="customerId">Required. Id of the customer to add the address to.</param>
        /// <param name="addressLine1">Required.</param>
        /// <param name="town">Required.</param>
        /// <param name="postcode">Required.</param>
        /// <param name="addressLine2"></param>
        /// <param name="county"></param>
        /// <param name="country">Will default to defined default country if not supplied</param>
        /// <returns>
        /// NotFound (404): no customer found with the id
        /// BadRequest (400): invalid customerId or address data
        /// Conflict (409): address already exists
        /// Ok (200): address successfully added
        /// </returns>
        [HttpPut("address")]
        public async Task<IActionResult> AddAddressAsync(
            Guid customerId,
            string addressLine1,
            string town,
            string postcode,
            string addressLine2, 
            string county,
            string country,
            CancellationToken cancellationToken)
        {
            if (customerId == Guid.Empty)
            {
                return this.BadRequest();
            }

            var customer = await this.customerService
                .GetCustomerAsync(customerId, cancellationToken)
                .ConfigureAwait(false);

            if (customer == null)
            {
                return this.NotFound();
            }

            var address = new Address()
            {
                AddressId = Guid.NewGuid(),
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                Country = country ?? this.DefaultCountry,
                County = county,
                Postcode = postcode,
                Town = town
            };

            if (!address.IsValid)
            {
                return this.BadRequest();
            }
    
            // check if address exists on customer
            if (customer.HasAddress(address))
            {
                return this.Conflict();
            }

            customer.SecondaryAddresses = customer.SecondaryAddresses
                .Append(address)
                .ToArray();

            await this.customerService
                .UpdateCustomerAsync(customer, cancellationToken)
                .ConfigureAwait(false);

            return this.Ok(customer);
        }

        /// <summary>
        /// Delete address, if primary is deleted the next address will be selected to be the new primary
        /// </summary>
        /// <param name="customerId">Id of the customer who's address is being removed</param>
        /// <param name="addressId">Id of the address to remove</param>
        /// <returns>
        /// BadRequest (400): invalid customerid or addressId
        /// NotFound (404): no customer found or address not linked to customer
        /// Conflict (409): customer has only 1 address and it cannot be deleted
        /// Ok (200): address deleted
        /// </returns>
        [HttpDelete("address")]
        public async Task<IActionResult> DeleteAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken)
        {
            if (customerId == Guid.Empty || addressId == Guid.Empty)
            {
                return this.BadRequest();
            }

            var customer = await this.customerService
                .GetCustomerAsync(customerId, cancellationToken)
                .ConfigureAwait(false);

            if (customer == null)
            {
                return this.NotFound();
            }

            if (customer.PrimaryAddress.AddressId == addressId)
            {
                if (customer.SecondaryAddresses?.Any() ?? false)
                {
                    customer.PrimaryAddress = customer.SecondaryAddresses.First();
                    customer.SecondaryAddresses = customer.SecondaryAddresses
                        .Skip(1)
                        .ToArray();
                }
                else
                {
                    return this.Conflict();
                }
            }
            else if (customer.SecondaryAddresses.Any(x => x.AddressId == addressId))
            {
                customer.SecondaryAddresses = customer.SecondaryAddresses
                    .Where(x => x.AddressId != addressId)
                    .ToArray();
            }
            else
            {
                return this.NotFound();
            }

            await this.customerService
                .UpdateCustomerAsync(customer, cancellationToken)
                .ConfigureAwait(false);

            return this.Ok(customer);
        }

        /// <summary>
        /// Sets a different address as the primary address
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="addressId"></param>
        /// <returns>
        /// BadRequest (400): Invalid customer or address id
        /// NotFound (404): Unable to find customer or address
        /// Ok (200): Successfully changed address
        /// </returns>
        [HttpPost("primary")]
        public async Task<IActionResult> SetPrimaryAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken)
        {
            if (customerId == Guid.Empty || addressId == Guid.Empty)
            {
                return this.BadRequest();
            }

            // check if customer and address exist
            var customer = await this.customerService
                .GetCustomerAsync(customerId, cancellationToken)
                .ConfigureAwait(false);

            if (customer == null || 
                !(customer.SecondaryAddresses?.Any(x => x.AddressId == addressId) ?? false))
            {
                return this.NotFound();
            }

            var newPrimary = customer.SecondaryAddresses.First(x => x.AddressId == addressId);
            customer.SecondaryAddresses = customer.SecondaryAddresses
                .Where(x => x.AddressId != addressId)
                .Append(customer.PrimaryAddress)
                .ToArray();

            customer.PrimaryAddress = newPrimary;

            await this.customerService
                .UpdateCustomerAsync(customer, cancellationToken)
                .ConfigureAwait(false);

            return this.Ok(customer);
        }

        /// <summary>
        /// Sets the active/inactive state of the customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="active"></param>
        /// <returns>
        /// BadRequest (400): customer id not valid
        /// NotFound (404): customer not found
        /// Ok (200): set customer active state
        /// </returns>
        [HttpPost("active")]
        public async Task<IActionResult> SetActiveAsync(Guid customerId, bool active, CancellationToken cancellationToken)
        {
            if (customerId == Guid.Empty)
            {
                return this.BadRequest();
            }

            var customer = await this.customerService
                .GetCustomerAsync(customerId, cancellationToken)
                .ConfigureAwait(false);

            if (customer == null)
            {
                return this.NotFound();
            }

            customer.IsActive = active;

            await this.customerService
                .UpdateCustomerAsync(customer, cancellationToken)
                .ConfigureAwait(false);

            return this.Ok(customer);
        }
    }
}
