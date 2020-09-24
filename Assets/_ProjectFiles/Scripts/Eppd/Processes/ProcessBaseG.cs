using System.Collections.Generic;
using Gasanov.Eppd.Data;
using Gasanov.Eppd.Processors;

namespace Gasanov.Eppd.Processes
{
    public abstract class ProcessBase<TDataBlock> : IProcess<TDataBlock> where TDataBlock : DataBlock
    {
        
        public bool IsDisposed { get; set; }
        
        public void Process()
        {
            return;
        }

        public abstract void SearchData(List<DataBlock> data);

        public abstract void Process(TDataBlock dataBlock);

        public abstract void Dispose();
    }
}