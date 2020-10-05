using System;
using System.Collections.Generic;
using Gasanov.Core.Mvp;
using Gasanov.Utils.SaveUtilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class MainMenuView : MonoBehaviour, IView
    {
        [BoxGroup("Main")]
        [SerializeField] private RectTransform mainMenu;
        
        [BoxGroup("Profile")]
        [SerializeField] private RectTransform profileMenu;
        [BoxGroup("Profile")]
        [SerializeField] private RectTransform profileListBox;
        [BoxGroup("Profile")]
        [SerializeField] private Button buttonPrefab;
        
        public event Action OnStart = delegate {  };
        public event Action OnHome = delegate {  };
        
        public event Action<DataProfile> OnProfile = delegate(DataProfile profile) {  };
        
        /// <summary>
        /// Multicast delegate
        /// </summary>
        private Action clearLast = delegate {  };
        
        public void Enable()
        {
            gameObject.SetActive(true);
            mainMenu.gameObject.SetActive(true);
            enabled = true;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            enabled = false;
        }

        public void OnStartClick()
        {
            mainMenu.gameObject.SetActive(false);
            profileMenu.gameObject.SetActive(true);
            clearLast += () => profileMenu.gameObject.SetActive(false);
            
            OnStart();
        }

        public void OpenHome()
        {
            ClearLast();

            mainMenu.gameObject.SetActive(true);
            OnHome();
        }

        public void ShowProfiles(List<DataProfile> profiles)
        {
            // Генерация кнопок и подписка на событие OnClick
            for (var i = 0; i < profiles.Count; i++)
            {
                var profileIndex = i;
                
                var button = Instantiate(buttonPrefab, profileListBox);
                
                button.GetComponentInChildren<TMP_Text>().text = profiles[profileIndex].Name;
                button.onClick.AddListener(()=> OnProfileClick(profiles[profileIndex]));

                clearLast += () => Destroy(button.gameObject);
            }
        }

        public void OnProfileClick(DataProfile profile)
        {
            OnProfile(profile);
        }

        private void ClearLast()
        {
            clearLast();
            clearLast = delegate {  };
        }
    }
}