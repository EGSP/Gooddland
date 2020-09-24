using System;
using Gasanov.Eppd.Processors;

namespace Gasanov.Eppd.SubProcessors.Realizations
{
    public class StateMachineProcessor : Processor
    {
        public override void Init()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Создает новое состояние заданного типа.
        /// Заменяет текущее состояние. Возвращает новый экземпляр.
        /// </summary>
        public TProcessor ChangeState<TProcessor>() where TProcessor : StateProcessor
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Меняет состояние на заданное.
        /// </summary>
        public void ChangeStateTo(StateProcessor nextState)
        {
            throw new NotImplementedException();
        }
    }
}