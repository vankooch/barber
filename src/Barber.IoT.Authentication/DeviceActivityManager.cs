namespace Barber.IoT.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;

    public class DeviceActivityManager<TActivity> : IDeviceActivityManager<TActivity>, IDisposable
        where TActivity : class
    {
        private bool _disposed;

        public DeviceActivityManager(IDeviceActivityStore<TActivity> store)
        {
            this.Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IDeviceActivityStore<TActivity> Store { get; set; }

        /// <summary>
        /// The cancellation token used to cancel operations.
        /// </summary>
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public Task<IdentityResult> CreateAsync(TActivity activity)
        {
            this.ThrowIfDisposed();
            return this.Store.CreateActivityAsync(activity, this.CancellationToken);
        }

        /// <summary>
        /// Releases all resources used by the user manager.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<TActivity>> FindByDeviceIdAsync(string deviceId)
        {
            this.ThrowIfDisposed();
            return this.Store.FindActivityByDeviceIdAsync(deviceId, this.CancellationToken);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the role manager and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this._disposed)
            {
                this._disposed = true;
            }
        }

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }
    }
}
