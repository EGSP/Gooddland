using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gasanov.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scenes
{
    public class GameSceneManager : Singleton<GameSceneManager>
    {
        private IEnumerator sceneLoadRoutine;
        private IEnumerator sceneUnloadRoutine;

        private List<Scene> loadedScenes = new List<Scene>();
        
        protected override void Awake()
        {
            base.Awake();
            AlwaysExist = true;
        
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        
        /// <summary>
        /// Вгружает новую сцену в текущую активную.
        /// </summary>
        public CallBack LoadSceneAdditively(string sceneName)
        {
            var cb = new CallBack();
            
            if (Application.CanStreamedLevelBeLoaded(sceneName) == false)
            {
                Debug.LogError($"Scene {sceneName} does not exist!");
                return cb;
            }
        
            // Если прошлая операция незакончена.
            if (sceneLoadRoutine != null && sceneUnloadRoutine != null)
                return cb;
        
            sceneLoadRoutine = LoadSceneAdditivelyRoutine(sceneName, cb);
            StartCoroutine(sceneLoadRoutine);

            return cb;
        }

        /// <summary>
        /// Выгружает сцену.
        /// </summary>
        public CallBack UnloadScene(string sceneName)
        {
            var cb = new CallBack();
            
            // Если прошлая операция незакончена.
            if (sceneLoadRoutine != null && sceneUnloadRoutine != null)
                return cb;

            if (loadedScenes.FirstOrDefault(x => x.name == sceneName).name == null)
                return cb;
        
            sceneUnloadRoutine = UnloadSceneRoutine(sceneName, cb);
            StartCoroutine(sceneUnloadRoutine);

            return cb;
        }
        
        private IEnumerator LoadSceneAdditivelyRoutine(string sceneName, CallBack callBack)
        {
            var sceneAo = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!sceneAo.isDone)
            {
                // Debug.Log(sceneAo.progress);
                yield return null;
            }
        
            SceneManager.SetActiveScene(loadedScenes.Find(x=>x.name == sceneName));

            yield return new WaitForEndOfFrame();
            sceneLoadRoutine = null;
            
            callBack.On();
        }

        private IEnumerator UnloadSceneRoutine(string sceneName, CallBack callBack)
        {
            var ao = SceneManager.UnloadSceneAsync(sceneName,
                UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

            yield return ao;
            yield return new WaitForEndOfFrame();
        
            sceneUnloadRoutine = null;
            
            callBack.On();
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"Loaded scene {scene.name}");
            loadedScenes.Add(scene);
        }
    
        private void OnSceneUnloaded(Scene scene)
        {
            loadedScenes.Remove(scene);
            Debug.Log($"Unloaded scene {scene.name}");
        }
        
    }
}