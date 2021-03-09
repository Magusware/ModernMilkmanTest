﻿namespace WebAPI.Controllers
{
    using System;
    using System.Threading.Tasks;
    using CustomerServiceNS.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        public string DefaultCountry { get; set; } = "UK";

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
        public async Task<IActionResult> GetAsync(Guid customerId)
        {
            if (customerId == Guid.Empty)
            {
                return this.BadRequest();
            }

            var customer = new Customer()
            {
                CustomerId = customerId
            };

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
        public async Task<IActionResult> GetAllAsync()
        {
            var customers = new[]
            {
                new Customer()
                {
                    CustomerId = Guid.NewGuid(),
                    IsActive = true
                },
                new Customer()
                {
                    CustomerId = Guid.NewGuid(),
                    IsActive = false
                },
                new Customer()
                {
                    CustomerId = Guid.NewGuid(),
                    IsActive = false
                }
            };

            return this.Ok(customers);
        }

        /// <summary>
        /// Gets all the customers which are not set to inactive
        /// </summary>
        /// <returns></returns>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveAsync()
        {
            var customers = new[]
            {
                new Customer()
                {
                    CustomerId = Guid.NewGuid(),
                    IsActive = true
                },
                new Customer()
                {
                    CustomerId = Guid.NewGuid(),
                    IsActive = true
                },
                new Customer()
                {
                    CustomerId = Guid.NewGuid(),
                    IsActive = true
                }
            };

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
        public async Task<IActionResult> PutAsync([FromBody]Customer customer)
        {
            if (!customer.IsValid)
            {
                return this.BadRequest();
            }

            // check if customer already exists
            // not sure how to determine this yet since addresses 
            // can be changed and names are not unique, might
            // be able to tie this to email address.
            if (false)
            {
                return this.Conflict();
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

            // Write customer to customer service

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
        public async Task<IActionResult> DeleteAsync(Guid customerId)
        {
            if (customerId == Guid.Empty)
            {
                return this.BadRequest();
            }

            // check if customer exists
            if (false)
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
            string country)
        {
            if (customerId == Guid.Empty)
            {
                return this.BadRequest();
            }

            // check customer exists
            if (false)
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
            if (false)
            {
                return this.Conflict();
            }

            // add address to customer

            return this.Ok(address);
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
        public async Task<IActionResult> DeleteAddressAsync(Guid customerId, Guid addressId)
        {
            if (customerId == Guid.Empty || addressId == Guid.Empty)
            {
                return this.BadRequest();
            }

            // Check if customer and address exist
            if (false)
            {
                return this.NotFound();
            }

            // Check if can delete
            if (false)
            {
                return this.Conflict();
            }

            // Do delete

            return this.Ok();
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
        public async Task<IActionResult> SetPrimaryAddressAsync(Guid customerId, Guid addressId)
        {
            if (customerId == Guid.Empty || addressId == Guid.Empty)
            {
                return this.BadRequest();
            }

            // check if customer and address exist
            if (false)
            {
                return this.NotFound();
            }

            return this.Ok();
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
        public async Task<IActionResult> SetActiveAsync(Guid customerId, bool active)
        {
            if (customerId == Guid.Empty)
            {
                return this.BadRequest();
            }

            // check if customer exists
            if (false)
            {
                return this.NotFound();
            }

            // Set customer active state

            return this.Ok();
        }
    }
}
