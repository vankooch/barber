using System;
using Barber.IoT.Context;
using Barber.IoT.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Barber.IoT.Authentication.Test.Base
{
    public class TestBootstrap : IDisposable
    {
        private readonly IConfigurationRoot _configuration;
        private readonly Container _container;

        public TestBootstrap()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            this._configuration = builder.Build();
            this._container = this.BootstrapContainer(null, this._configuration);
            this._container.Verify();
        }

        public IConfigurationRoot Configuration => this._configuration;
        
        public Container Container => this._container;

        public void Dispose() => this._container.Dispose();

        private Container BootstrapContainer(Container container, IConfigurationRoot config)
        {
            if (container == null)
            {
                container = new Container();
            }

            // Options
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Configure
            container.RegisterInstance(config);

            // Ef
            var contextOptions = new DbContextOptionsBuilder<BarberIoTContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            container.RegisterInstance(contextOptions);

            // Main
            container.Register<IBarberIoTContextCreator, BarberIoTContextDiCreator>(Lifestyle.Transient);
            container.Register<IBarberIoTContext>(() => new BarberIoTContext(contextOptions.Options), Lifestyle.Transient);

            return container;
        }
    }
}
