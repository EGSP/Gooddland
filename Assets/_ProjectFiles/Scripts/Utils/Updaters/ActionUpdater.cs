using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Gasanov.Utils.Updaters
{
    
    public class ActionUpdater : IUpdater
    {
        public event Action<IUpdater> OnComplete;

        public string Name { get; private set; }
        public bool Active { get; private set; }

        public void Update(float deltaTime)
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }

    
}
