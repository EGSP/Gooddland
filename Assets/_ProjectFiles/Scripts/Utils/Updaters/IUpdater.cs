using System;

namespace Gasanov.Utils.Updaters
{
    public interface IUpdater
    {
        event Action<IUpdater> OnComplete;

        string Name { get; }
        bool Active { get; }
        
        void Update(float deltaTime);

        void Pause();
        void Resume();

        void Destroy();
    }
}