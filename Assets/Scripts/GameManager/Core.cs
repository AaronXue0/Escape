using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;
using GameManagerSpace.Menu;
using GameManagerSpace.Hall;
using GameManagerSpace.Game;

namespace GameManagerSpace
{
    public enum SceneState
    {
        LoadScene,
        MenuScene,
        SettingsScene,
        HallScene,
        LabMapScene,
    };
    public enum GameState
    {
        Setting, // On game scene and is setting.
        Starting, // On game scene and is starting.
        Playing, // On game scene and is playing.
        Scoring, // On score scene.
    }
    public class Core : MonoBehaviour, ICore
    {
        private SceneState currentScene = new SceneState();
        private List<Player> inputs = new List<Player>();
        CoreView trasition = null;

        /// <summary>
        /* Input Controller */
        /// <summary>
        public void AssignAllJoysticksToSystemPlayer(bool removeFromOtherPlayers)
        {
            foreach (var j in ReInput.controllers.Joysticks)
            {
                ReInput.players.GetSystemPlayer().controllers.AddController(j, removeFromOtherPlayers);
            }
        }
        public void ChangeInputMaps(string name)
        {
            inputs.SelectAllTheMap(name);
        }
        /// <summary>
        /* Scene Control */
        /// <summary>
        public void ChangeScene(string name)
        {
            SceneManager.LoadScene(name);
        }
        public void MaskChangeScene(string name, bool withLoading)
        {
            if (withLoading) trasition.MaskInWithLoading(() => SceneManager.LoadScene(name), () => trasition.UpdateLoadingUI(false));
            else trasition.MaskIn(() => SceneManager.LoadScene(name));
        }
        /// <summary>
        /// *List each game scene name here and SceneState enum.
        /// </summary>
        /// <param name="GameSceneRegister">Important ! Must remember to register game scene</param>
        void SceneStateManagement()
        {
            switch (currentScene)
            {
                case SceneState.LoadScene:
                    ChangeInputMaps("Load");

                    var pressAnyButton = FindObjectOfType<PressAnyButton>();
                    pressAnyButton.SceneCallbackAction = ChangeScene;

                    break;
                case SceneState.MenuScene:
                    ChangeInputMaps("Menu");

                    var menu = FindObjectOfType<MenuManager>();
                    menu.Init(MaskChangeScene);

                    break;
                case SceneState.SettingsScene:
                    ChangeInputMaps("Settings");

                    // var settings = FindObjectOfType<SettingsManager>();
                    // settings.Init(MaskChangeScene);

                    trasition.MaskOut();
                    break;
                case SceneState.HallScene:
                    ChangeInputMaps("Hall");

                    var hall = FindObjectOfType<HallManager>();
                    hall.Init(
                        MaskChangeScene,
                        (int id) => assignedJoysticks.Add(id),
                        (int id) => assignedJoysticks.Remove(id)
                    );

                    trasition.MaskOut();
                    break;
                case SceneState.LabMapScene:
                    ChangeInputMaps("GamePlay");

                    var gm = FindObjectOfType<GameManager>();
                    gm.Init(MaskChangeScene, trasition.MaskOut);

                    break;
            }
        }
        /// <summary>
        /* Native APIs */
        /// <summary>
        List<int> assignedJoysticks = new List<int>();

        void OnControllerConnected(ControllerStatusChangedEventArgs args)
        {
            if (args.controllerType != ControllerType.Joystick) return;

            // Check if this Joystick has already been assigned. If so, just let Auto-Assign do its job.
            if (assignedJoysticks.Contains(args.controllerId)) return;

            // Joystick hasn't ever been assigned before. Make sure it's assigned to the System Player until it's been explicitly assigned
            ReInput.players.GetSystemPlayer().controllers.AddController(
                args.controllerType,
                args.controllerId,
                true // remove any auto-assignments that might have happened
            );
        }
        void Awake()
        {
            ReInput.ControllerConnectedEvent += OnControllerConnected;
            currentScene.ChangeState(SceneManager.GetActiveScene().name);
            inputs = inputs.FindAllPlayersWithJoystick();
            trasition = GetComponent<CoreView>();
        }
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            currentScene.ChangeState(scene.name);
            SceneStateManagement();
        }
        void Start()
        {
            AssignAllJoysticksToSystemPlayer(true);
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}