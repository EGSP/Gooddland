using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scenes;
using Game.Cameras;
using Gasanov.Core;
using Gasanov.Utils.SaveUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using World.Generators;

namespace Game
{
    public class GameStarter : Singleton<GameStarter>
    {
        /// <summary>
        /// Процедура запуска начала игровой сессии.
        /// </summary>
        public void StartGame(DataProfile worldProfile)
        {
            Storage.SetLocal(worldProfile);
            
            InitializeData();
            
            LoadWorld("world");
        }

        /// <summary>
        /// Инициализация первичных данных.
        /// </summary>
        private void InitializeData()
        {
            var profileInfo = Storage.GetLocalProfileInfo();

            var firstLaunch = true;
            profileInfo.GetBool("firstLaunch", ref firstLaunch);
            
            if (firstLaunch)
            {
                // Генерация информации о мире.
                WorldGenerator.GenerateWorldInfo();
                
                // Снимаем метку первого запуска.
                profileInfo.SetBool("firstLaunch", false).Apply();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            AlwaysExist = true;

            CameraManager.Instance.AutoTranslateToNew = false;
        }

        public void LoadWorld(string sceneName)
        {
            GameSceneManager.Instance.LoadSceneAdditively(sceneName)
                .on += OnWorldSceneLoaded;
        }

        public void UnloadWorld(string sceneName)
        {
            GameSceneManager.Instance.UnloadScene(sceneName)
                .on += OnWorldSceneUnloaded;
        }

        private void OnWorldSceneLoaded()
        {
            LoadWorldData();
            GameSession.Instance.InitializeSession();
            CameraManager.Instance.TranslateToTarget("world");
        }
        
        private void OnWorldSceneUnloaded()
        {
            CameraManager.Instance.ActiveCamera.TranslateToStart();
        }
        
        private void LoadWorldData()
        {
            WorldGenerator.Instance.Init();
            WorldGenerator.Instance.LoadWorldInfo();
            
            // Загрузка острова игрока, если он был.
            GameMaster.LoadPlayerIsland();
            
            // Генерация нового острова. Проверка нужна, т.к. старые данные в приоритете.
            if (!GameMaster.PlayerHaveIsland)
            {
                // Остров в гейммастере на данный момент просто пустышка.
                var newPlayerIsland = WorldGenerator.Instance.CreateIsland(
                    0, 0);
                    
                // Установка и сохранение нового острова.
                GameMaster.SetPlayerIsland(newPlayerIsland);
            }
        }
        
        /// <summary>
        /// Получение всех профилей игры. При отсутствии будут сгенерированы новые.
        /// </summary>
        public List<DataProfile> GetProfiles()
        {
            var profiles = Storage.GetProfiles();

            if (profiles.Count == 0)
            {
                profiles.Add(new DataProfile("Game1"));
                profiles.Add(new DataProfile("Game2"));
                profiles.Add(new DataProfile("Game3"));
                profiles.Add(new DataProfile("Game4"));
                profiles.Add(new DataProfile("Game5"));

                Storage.g.SaveEntities("Profiles/", profiles);
            }

            return profiles;
        }
    }
}