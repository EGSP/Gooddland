using System;
using Game.Views;
using Gasanov.Core.Mvp;

namespace Game.Presenters
{
    public class GameSessionPresenter : IPresenter<GameSessionView, GameSession>
    {
        public GameSessionView View { get; private set; }
        public GameSession Model { get; private set; }
        
        public event Action OnDispose = delegate {  };

        private void Awake()
        {
            Share();
        }
        
        public void Share()
        {
            PresenterMediator.Register(this);
        }

        public bool Response(string key)
        {
            if (key == "gameSession")
                return true;

            return false;
        }
        
        public void Dispose()
        {
            OnDispose();
        }

    }
}