using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gasanov.Core;
using Gasanov.Utils.MathUtilities;
using UnityEngine;

namespace Game.Cameras
{ 
    public class CameraManager : Singleton<CameraManager>
    {
        /// <summary>
        /// Текущая активная камера.
        /// </summary>
        public SceneCamera ActiveCamera
        {
            get => activeCamera;
            set
            {
                if (value == null)
                {
                    activeCamera = null;
                    return;
                }

                if (activeCamera == value)
                    return;
                
                if (activeCamera != null)
                    activeCamera.Camera.enabled = false;
                
                value.Camera.enabled = true;
                
                activeCamera = value;
            }
        }
        private SceneCamera activeCamera;

        /// <summary>
        /// Будет ли камера переходить в следующую зарегистрированную автоматически.
        /// </summary>
        public bool AutoTranslateToNew { get; set; } = true;

        /// <summary>
        /// При отключении активной камеры, будет выбрана самая первая в списке.
        /// </summary>
        public bool AutoBackToFirst { get; set; } = true;

        /// <summary>
        /// Существующие камеры на сцене.
        /// </summary>
        private List<SceneCamera> sceneCameras = new List<SceneCamera>();
        
        /// <summary>
        /// Перемещает текущую камеру на место цели, по имени.
        /// </summary>
        public CallBack TranslateToTarget(string name)
        {
            var cb = new CallBack();
            var target = sceneCameras.FirstOrDefault(x => x.Name == name);

            if (target == null)
            {
                return cb;
            }

            ActiveCamera.TranslateToTarget(target,cb);
            return cb;
        }

        /// <summary>
        /// Заносит камеру в список. Отключает и подписывается на OnDispose.
        /// </summary>
        public void RegistCamera(SceneCamera newCamera)
        {
            newCamera.OnDispose += RemoveCamera;
            newCamera.Registered = true;
            newCamera.Camera.enabled = false;
            
            sceneCameras.Add(newCamera);

            if (ActiveCamera == null)
            {
                ActiveCamera = newCamera;
            }
            else
            {
                if(AutoTranslateToNew)
                    ActiveCamera.TranslateToTarget(newCamera, new CallBack());                
            }

        }

        /// <summary>
        /// Убирает камеру из списка.
        /// </summary>
        public void RemoveCamera(SceneCamera sceneCamera)
        {
            sceneCameras.Remove(sceneCamera);

            sceneCamera.Registered = false;
            
            if (ActiveCamera == sceneCamera)
                ActiveCamera = null;

            if (sceneCameras.Count != 0)
            {
                if (AutoBackToFirst)
                    ActiveCamera = sceneCameras[0];
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (ActiveCamera == null)
            {
                var findedCamera = FindObjectOfType<SceneCamera>();
                RegistCamera(findedCamera);
            }
        }
        
    }
}