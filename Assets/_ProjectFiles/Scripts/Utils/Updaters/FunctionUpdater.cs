using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Gasanov.Utils.Updaters
{
    
    public class FunctionUpdater : IUpdater
    {
        public event Action<IUpdater> OnComplete;

        public FunctionUpdater(Func<float, bool> function, string name = "default")
        {
            this.function = function;
            Name = name;
        }
        
        /// <summary>
        /// Вызываемая функция.
        /// </summary>
        private Func<float, bool> function;

        public string Name { get; private set; }
        public bool Active { get; private set; }

        public void Update(float deltaTime)
        {
            if (!Active)
                return;

            if (function(deltaTime))
            {
                OnComplete(this);
                Destroy();
            }
        }

        public void Pause()
        {
            Active = false;
        }

        public void Resume()
        {
            Active = true;
        }

        public void Destroy()
        {
            UpdaterFactory.Destroy(this);
        }
    }

}
