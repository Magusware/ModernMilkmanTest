namespace CustomerService.Tests
{
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using AutoFixture.Xunit2;
    using System;

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

            return fixture;
        }
    }
}
