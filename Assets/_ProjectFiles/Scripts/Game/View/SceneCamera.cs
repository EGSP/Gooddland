using System;
using System.Collections;
using System.Collections.Generic;
using Gasanov.Utils.MathUtilities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.View.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class SceneCamera : SerializedMonoBehaviour, IDisposable
    {
        [BoxGroup("Entity")]
        [OdinSerialize] public string Name { get; private set; }
        
        /// <summary>
        /// Должна ли камера стать активной при появлении.
        /// </summary>
        [BoxGroup("Behaviour")]
        [SerializeField] public bool activeOnAwake;
        
        [BoxGroup("Behaviour")]
        [SerializeField] private float smoothness;

        private Vector3 startPosition;
        
        public Camera Camera
        {
            get
            {
                if (camera == null)
                {
                    camera = GetComponent<Camera>();

                    if (camera == null)
                        throw new NullReferenceException();
                }

                return camera;
            }
            private set => camera = value;
        }
        private Camera camera;
        
        /// <summary>
        /// Зарегестрирована ли камера.
        /// </summary>
        public bool Registered { get; set; }
        
        private IEnumerator translateRoutine;

        public event Action<SceneCamera> OnDispose = delegate(SceneCamera sceneCamera) {  };
        

        private void Awake()
        {
            startPosition = transform.position;
            
            RegistToManager();
            
            if (activeOnAwake)
                CameraManager.Instance.ActiveCamera = this;
        }

        private void RegistToManager()
        {
            if (Registered)
                return;
            
            var cm = CameraManager.Instance;
            cm.RegistCamera(this);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            OnDispose(this);
        }
        
        
        /// <summary>
        /// Перемещает текущую камеру на место цели.
        /// </summary>
        /// <param name="target"></param>
        public void TranslateToTarget(SceneCamera target)
        {    
            if(translateRoutine != null)
                StopCoroutine(translateRoutine);

            translateRoutine = TranslateCameraToTarget(target.transform.position, smoothness);
            StartCoroutine(translateRoutine);
        }

        private IEnumerator TranslateCameraToTarget(Vector3 targetPosition, float smoothness)
        {
            var cameraTransform = transform;
            
            var sqrMagnitude = (cameraTransform.position - targetPosition).magnitude;
            
            // Пока расстояние больше очень малого значения близкого к нулю
            while (sqrMagnitude > float.Epsilon)
            {
                var newPosition = MathUtils.DampPow(cameraTransform.position,targetPosition,
                    smoothness, Time.deltaTime);

                sqrMagnitude = (transform.position - newPosition).magnitude;

                cameraTransform.position = newPosition;
                
                // Ждем обновления кадра
                yield return null;
            }
            
            // Привязка к конечной позиции
            cameraTransform.position = targetPosition;
            
            translateRoutine = null;
        }

        public void TranslateToStart(bool immediately = false)
        {
            if(translateRoutine != null)
                StopCoroutine(translateRoutine);

            var localSmoothness = smoothness;
            if (immediately)
                localSmoothness = 0;

            translateRoutine = TranslateCameraToTarget(startPosition, localSmoothness);
            StartCoroutine(translateRoutine);
        }
    }
}