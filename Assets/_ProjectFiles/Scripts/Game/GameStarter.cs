using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scenes;
using Gasanov.Core;
using Game.View.Cameras;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : Singleton<GameStarter>
{
    private IEnumerator worldLoadRoutine;
    private IEnumerator worldUnloadRoutine;

    private List<Scene> loadedScenes = new List<Scene>();

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
        CameraManager.Instance.TranslateToTarget("world");
    }
    
    private void OnWorldSceneUnloaded()
    {
        CameraManager.Instance.ActiveCamera.TranslateToStart();
    }
}
