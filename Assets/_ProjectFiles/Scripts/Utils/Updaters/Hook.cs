using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Gasanov.Core;
using UnityEngine;

namespace Gasanov.Utils.Updaters
{
    public class Hook : Singleton<Hook>
    {
        /// <summary>
        /// Вызывается при обновлении кадра.
        /// </summary>
        public event Action<float> OnUpdate = delegate(float f) {  };
        /// <summary>
        /// Вызывается при обновлении кадра.
        /// </summary>
        public event Action<float> OnFixedUpdate = delegate(float f) {  };
        /// <summary>
        /// Вызывается при обновлении кадра.
        /// </summary>
        public event Action<float> OnLateUpdate = delegate(float f) {  };

        private void Update()
        {
            OnUpdate(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            OnFixedUpdate(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            OnLateUpdate(Time.deltaTime);
        }
    }
}