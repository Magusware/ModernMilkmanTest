namespace CustomerServiceNS.Tests
{
    using System;
    using System.Linq;
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using AutoFixture.Xunit2;

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

            fixture.Customize<Database.Customer>(customization =>
            {
                return customization
                    .Without(customer => customer.Addresses)
                    .Do(customer => 
                    {
                        customer.Addresses = fixture.CreateMany<Database.Address>();

                        foreach (var address in customer.Addresses)
                        {
                            address.IsPrimary = false;
                        }

                        customer.Addresses.First().IsPrimary = true;
                    });
            });
            return fixture;
        }
    }
}
