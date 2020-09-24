using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Gasanov.Eppd.Data;
using Gasanov.Eppd.Processes.Realizations;

namespace Gasanov.Eppd.SubProcessors.Realizations
{
    public class MovementProcessor : SubProcessor
    {
        public override void Init()
        {
            AppendProcess(new MoveProcess());
            
            DataReadAvailable();
        }

        public override void SearchData(List<DataBlock> data)
        {
            return;
        }

        public override void Dispose()
        {
            return;
        }
    }
}