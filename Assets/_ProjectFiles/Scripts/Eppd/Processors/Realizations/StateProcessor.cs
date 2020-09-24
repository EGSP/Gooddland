using Gasanov.Eppd.Processors;

namespace Gasanov.Eppd.SubProcessors.Realizations
{
    public abstract class StateProcessor : Processor
    {
        public override void Tick()
        {
            TickState();
        }

        /// <summary>
        /// Метод используемый машиной состояний
        /// </summary>
        public abstract StateProcessor TickState();
       
    }
}