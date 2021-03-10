namespace WebAPI.Tests
{
    using System;
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using AutoFixture.Xunit2;
    using WebAPI.Controllers;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AutoDomainDataAttribute : AutoDataAttribute
    {
        public AutoDomainDataAttribute()
            : base(() => CreateFixture())
        {
        }

        private static IFixture CreateFixture()
        {
            IFixture fixture = new Fixture().Customize(new AutoMoqCustomization() { ConfigureMembers = true });

            fixture.Customize<CustomerController>(customisation =>
            {
                return customisation.OmitAutoProperties();
            });

            fixture.Customize<CustomerServiceNS.Interfaces.Customer>(customization =>
            {
                return customization
                    .Without(x => x.Title)
                    .Without(x => x.Forename)
                    .Without(x => x.Surname)
                    .Without(x => x.EmailAddress)
                    .Without(x => x.MobileNumber)
                    .Do(customer =>
                    {
                        customer.Title = Guid.NewGuid().ToString().Substring(0, 8);
                        customer.Forename = Guid.NewGuid().ToString().Substring(0, 8);
                        customer.Surname = Guid.NewGuid().ToString().Substring(0, 8);
                        customer.EmailAddress = Guid.NewGuid().ToString().Substring(0, 8);
                        customer.MobileNumber = Guid.NewGuid().ToString().Substring(0, 8);
                    });
            });

            fixture.Customize<CustomerServiceNS.Interfaces.Address>(customization =>
            {
                return customization
                    .Without(x => x.AddressLine1)
                    .Without(x => x.AddressLine2)
                    .Without(x => x.Town)
                    .Without(x => x.County)
                    .Without(x => x.Postcode)
                    .Without(x => x.Country)
                    .Do(x =>
                    {
                        x.AddressLine1 = Guid.NewGuid().ToString().Substring(0, 8);
                        x.AddressLine2 = Guid.NewGuid().ToString().Substring(0, 8);
                        x.Town = Guid.NewGuid().ToString().Substring(0, 8);
                        x.County = Guid.NewGuid().ToString().Substring(0, 8);
                        x.Postcode = Guid.NewGuid().ToString().Substring(0, 8);
                        x.Country = Guid.NewGuid().ToString().Substring(0, 8);
                        x.Postcode = Guid.NewGuid().ToString().Substring(0, 8);
                    });
            });

            return fixture;
        }
    }
}
