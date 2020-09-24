using System;
using System.Collections;
using System.Collections.Generic;
using Gasanov.Eppd.Processes;
using Gasanov.Extensions.Linq;
using Gasanov.Eppd.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gasanov.Eppd.Processors
{
    public abstract class Processor
    {
        protected Processor()
        {
            Processes = new List<IProcess>();
            DataBlocks = new List<DataBlock>();
        }

        [OdinSerialize]
        // Исполняемые процессы
        protected List<IProcess> Processes;

        [OdinSerialize]
        // Данные систем
        protected List<DataBlock> DataBlocks;
        
        /// <summary>
        /// Вызывается, когда данные могут быть прочитаны
        /// </summary>
        private event Action<List<DataBlock>> OnDataReadAvailable = delegate(List<DataBlock> data) {  };

        /// <summary>
        /// Инициализирует процессор.
        /// </summary>
        public abstract void Init();

        // Запуск нового прохода 
        public virtual void Tick()
        {
            // Выполнение всех процессов
            for (var i = 0; i < Processes.Count; i++)
            {
                Processes[i].Process();
                ProcessorDebug.Log("tick");
            }
        }

        /// <summary>
        /// Добавляет процесс в конец списка.
        /// </summary>
        /// <param name="process"></param>
        public void AppendProcess(IProcess process)
        {
            OnDataReadAvailable += process.SearchData;
            Processes.Add(process);
        }
        
        protected void DataReadAvailable()
        {
            OnDataReadAvailable(DataBlocks);
        }

        /// <summary>
        /// Получение первого найденного блока заданного типа.
        /// Может вернуть null.
        /// </summary>
        public T GetDataBlock<T>() where T : DataBlock
        {
            var coincidence = DataBlocks.FindType<T>();

            return coincidence;
        }

        /// <summary>
        /// Передача данных процессору
        /// </summary>
        public void TranferData(List<DataBlock> dataBlocks)
        {
            if (dataBlocks != null)
                DataBlocks = dataBlocks;
        }

        public IEnumerator GetEnumerator()
        {
            return Processes.GetEnumerator();
        }
    }

}
