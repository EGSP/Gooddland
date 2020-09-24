using System.Collections.Generic;
using System.Threading.Tasks;
using Gasanov.Eppd.Data;
using Gasanov.Eppd.Processors;

namespace Gasanov.Eppd.Processes
{
    /// <summary>
    /// Позволяет связать данные и процесс.
    /// </summary>
    public class DataProcess<TDataBlock,TProcess> : IProcess
        where TDataBlock: DataBlock
        where TProcess: IProcess<TDataBlock>
    {
        /// <summary>
        /// Исполняемый процесс.
        /// </summary>
        private readonly TProcess _process;
        
        public DataProcess(Processor processor, TProcess process)
        {
            var dBlock = processor.GetDataBlock<TDataBlock>();

            _process = process;

            FindData = true;
        }
        
        public DataProcess(Processor processor, TProcess process, TDataBlock data): this(processor, process)
        {
            if (data != null)
            {
                _dataBlock = data;
            }
        }

        
        /// <summary>
        /// Данные, которые передаются процессу.
        /// </summary>
        private TDataBlock _dataBlock;

        /// <summary>
        /// Искать данные, если они null.
        /// </summary>
        public bool FindData { get; set; }

        public bool IsDisposed { get; private set; }

        public void Process()
        {
            if (_process.IsDisposed)
            {
                Dispose();
            }
            
            if (IsDisposed)
                return;

            if (_dataBlock == null)
                return;

            _process.Process(_dataBlock);
        }

        public void SearchData(List<DataBlock> data)
        {
            if(_dataBlock == null && FindData)
                _process.SearchData(data);
        }

        /// <summary>
        /// Установка данных. Старые данные будут заменены.
        /// </summary>
        private void SetData(TDataBlock data)
        {
            if (data != null)
            {
                _dataBlock = data;
            }
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}