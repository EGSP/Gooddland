using System;
using System.Collections.Generic;
using Gasanov.Eppd.Data;
using Gasanov.Eppd.Processors;

namespace Gasanov.Eppd.Processes
{
    /// <summary>
    /// Позволяет связывать два процесса.
    /// </summary>
    public class ContextProcess<TFirstProcess, TSecondProcess> : IProcess
        where TFirstProcess : IProcess
        where TSecondProcess : IProcess
    {

        private readonly TFirstProcess _firstProcess;

        private readonly TSecondProcess _secondProcess;

        private readonly Action<TFirstProcess, TSecondProcess> _context;


        /// <param name="context">Действие, которе будет выполняться с процессами</param>
        public ContextProcess(
            TFirstProcess firstProcess,
            TSecondProcess secondProcess,
            Action<TFirstProcess, TSecondProcess> context)
        {
            _firstProcess = firstProcess;
            _secondProcess = secondProcess;
        }
        
        public bool IsDisposed { get; set; }

        public void Process()
        {
            if (IsDisposed)
                return;
            
            if (AnyIsDisposed())
            {
                Dispose();
            }

            _context(_firstProcess, _secondProcess);
        }

        public void SearchData(List<DataBlock> data)
        {
            _firstProcess.SearchData(data);
            _secondProcess.SearchData(data);
        }

        /// <summary>
        /// Проверяет ссылки на процессы
        /// </summary>
        private bool AnyIsDisposed()
        {
            if (_firstProcess.IsDisposed)
                return true;

            if (_secondProcess.IsDisposed)
                return true;

            return false;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}