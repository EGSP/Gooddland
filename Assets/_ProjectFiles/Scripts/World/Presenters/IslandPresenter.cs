using System;
using Gasanov.Core;
using Gasanov.Core.Mvp;
using Gasanov.Extensions.Mono;
using Sirenix.Serialization;
using World.Entities;
using World.Views;

namespace World.Presenters
{
    public class IslandPresenter : SerializedSingleton<IslandPresenter>, IPresenter<IslandView, IslandEntity>
    {
        [OdinSerialize]
        public IslandView View { get; private set; }
        [OdinSerialize]
        public IslandEntity Model { get; private set; }
        
        public event Action OnDispose;
        
        private void Start()
        {
            if(View == null)
                throw new NullReferenceException();
        }

        /// <summary>
        /// Установка новой модели.
        /// Сеттер не используется из-за нагрузки на инспектор Unity.
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(IslandEntity model)
        {
            if (Model != null)
            {
                // Отписываемся от событий.
            }

            // Подписываемся на события.
            Model = model;
            
            View.Enable();
            View.ShowIsland(Model.IslandData);
        }

        public void Dispose()
        {
            View.Disable();
            OnDispose();
        }
        
        public void Share()
        {
            return;
        }

        public bool Response(string key)
        {
            return false;
        }
    }
}