using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagerSpace.Game
{
    public class GameManager : MonoBehaviour, IGameManager
    {
        [SerializeField] string scoreScene = "ScoreScene";

        System.Action<string, bool> loadSceneAction;
        System.Action loadedAction;
        public void Init(System.Action<string, bool> loadSceneActionCallback, System.Action loadedCallback)
        {
            loadSceneAction = loadSceneActionCallback;
            loadedAction = loadedCallback;
        }

        private GameState gameState = new GameState();
        private Control control = null;

        public void GameFlow()
        {
            switch (gameState)
            {
                case GameState.Setting:
                    StartCoroutine(Setting());
                    break;
                case GameState.Starting:
                    StartCoroutine(Starting());
                    break;
                case GameState.Playing:
                    Playing();
                    break;
                case GameState.Scoring:
                    Scoring();
                    break;
            }
        }

        public void SettingCallback()
        {
            gameState.ChangeState("Starting");
            GameFlow();
        }

        public IEnumerator Setting()
        {
            var a = StartCoroutine(control.RandomRoom());
            yield return a;
            var b = StartCoroutine(control.RandomStartItems());
            yield return b;
            var c = StartCoroutine(control.SpawnPlayers());
            yield return c;
            loadedAction();
        }

        public void StartingCallback()
        {
            gameState.ChangeState("Playing");
            GameFlow();
        }

        public IEnumerator Starting()
        {
            var a = StartCoroutine(control.EscaperStart());
            yield return a;
            var b = StartCoroutine(control.HunterStart());
            yield return b;
            StartingCallback();
        }

        public void PlayingCallback()
        {
            gameState.ChangeState("Scoring");
            GameFlow();
        }

        public IEnumerator Playing()
        {
            yield return null;
        }

        public void ScoringCallback()
        {

        }
        public IEnumerator Scoring()
        {
            loadSceneAction(scoreScene, false);
            yield return null;
        }

        private void Awake()
        {
            gameState.ChangeState("Setting");
            control = GetComponent<Control>();
            control.Init();
        }

        private void Start()
        {
            GameFlow();
        }
    }
}