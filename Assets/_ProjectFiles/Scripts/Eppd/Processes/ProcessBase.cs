using System.Collections.Generic;
using Gasanov.Eppd.Data;
using Gasanov.Eppd.Processors;

namespace Gasanov.Eppd.Processes
{
    public abstract class ProcessBase : IProcess
    {
        public bool IsDisposed { get; set; }

        public abstract void Dispose();

        public abstract void Process();

        public abstract void SearchData(List<DataBlock> data);

        public bool DataNullOrDisposed(params DataBlock[] dataBlocks)
        {
            for (var i = 0; i < dataBlocks.Length; i++)
            {
                var datablock = dataBlocks[i];
                
                if (datablock == null)
                    return true;

                if (datablock.IsDisposed)
                    return true;
            }

            return false;
        }
    }
}