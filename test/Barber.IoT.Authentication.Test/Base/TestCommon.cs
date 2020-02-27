using System;
using Microsoft.Extensions.Configuration;
using SimpleInjector;

namespace Barber.IoT.Authentication.Test.Base
{
    public abstract class TestCommon : IDisposable
    {
        private readonly TestBootstrap _shared;

        public TestCommon() => this._shared = new TestBootstrap();

        public IConfigurationRoot Configuration => this._shared.Configuration;

        public Container Container => this._shared.Container;

        public void Dispose() => this._shared.Dispose();
    }
}
