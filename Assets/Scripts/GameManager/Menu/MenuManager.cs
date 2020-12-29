using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace GameManagerSpace.Menu
{
    public class MenuManager : MonoBehaviour, IMenuManager
    {
        [SerializeField] string hallScene = "HallScene";
        [SerializeField] string settingsScene = "SettingsScene";
        System.Action<string, bool> loadSceneAction;

        View view = null;
        bool ableToClickButton = false;

        public void Init(System.Action<string, bool> callback)
        {
            loadSceneAction = callback;
        }

        public void AnimationEventCallback()
        {
            view.InitButton();
            ableToClickButton = true;
        }

        public void Play()
        {
            if (ableToClickButton == false) return;
            loadSceneAction(hallScene, false);
        }

        public void Settings()
        {
            if (ableToClickButton == false) return;
            loadSceneAction(settingsScene, false);
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void Confirm()
        {
            if (ableToClickButton == false) return;
            if (ReInput.players.GetSystemPlayer().GetButtonDown("Confirm"))
            {
                view.CurrentHighLightButton.onClick.Invoke();
            }
        }

        /// <summary>
        /// Menu UI Handling
        /// </summary>
        private void Awake()
        {
            view = GetComponent<View>();
        }
        private void Start()
        {
            foreach (AnimationCore _a in FindObjectsOfType<AnimationCore>())
            {
                _a.AnimationCallback = AnimationEventCallback;
            }
        }
        private void Update()
        {
            Confirm();
        }
    }
}