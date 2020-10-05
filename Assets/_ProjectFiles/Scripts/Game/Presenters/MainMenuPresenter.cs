using System;
using System.Collections.Generic;
using Game;
using Game.Views;
using Gasanov.Core.Mvp;
using Gasanov.Core;
using Gasanov.Extensions.Mono;
using Gasanov.Utils.SaveUtilities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Presenters
{
    public class MainMenuPresenter : SerializedMonoBehaviour, IPresenter<MainMenuView, GameStarter>
    {
        [OdinSerialize]
        public MainMenuView View { get; private set; }
        [OdinSerialize]
        public GameStarter Model { get; private set; }
        
        public event Action OnDispose = delegate() {  };

        public void Share()
        {
            PresenterMediator.Register(this);
        }

        public bool Response(string key)
        {
            return false;
        }

        private void Awake()
        {
            Share();
        }

        /// <summary>
        /// Start вместо Awake нужен для корректной работы при ручном создании презентера.
        /// </summary>
        private void Start()
        {
            Model = this.ValidateComponent<GameStarter>(Model);
            
            if(Model == null)
                throw new NullReferenceException();
            
            if(View == null)
                throw new NullReferenceException();

            View.OnStart += OnStart;
            View.OnProfile += OnProfile;
            View.OnHome += OnHome;
            
            View.Enable();
        }

        private void OnStart()
        {
            var profiles = Model.GetProfiles();
            View.ShowProfiles(profiles);
        }

        private void OnProfile(DataProfile profile)
        {
            Debug.Log(profile.Name);
            Model.StartGame(profile);
            
            View.Disable();
        }

        private void OnHome()
        {
            
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            OnDispose();
        }
    }
}