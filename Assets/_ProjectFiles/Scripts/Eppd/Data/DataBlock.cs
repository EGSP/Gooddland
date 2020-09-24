using System;
using Gasanov.Extensions;

namespace Gasanov.Eppd.Data
{
    public abstract class DataBlock : IDisposable, ICloneable
    {
        public bool IsDisposed { get; protected set; }
        
        public abstract object Clone();

        public virtual void Dispose()
        {
            IsDisposed = true;
        }
    }
}