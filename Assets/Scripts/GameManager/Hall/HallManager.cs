using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rewired;
using Role.PlayerSpace.Hall;

namespace GameManagerSpace.Hall
{
    public enum SelectModelState
    {
        onJoin, onChooseEscaper, onChooseHunter, onChooseMap, onWait
    }
    public class PlayerContainer
    {
        public int id = -1;
        public PlayerCharacter selfScript = null;
        public GameObject selfAvatar = null;
        public SelectModelState selfSelectState = SelectModelState.onJoin;
        public GameObject escaperModel = null;
        public GameObject hunterModel = null;
        public string choosenMap = "";

    }
    public class HallManager : MonoBehaviour, IHallManager
    {
        System.Action<string, bool> loadSceneAction;
        public void Init(System.Action<string, bool> _loadSceneCallback, System.Action<int> _joinCallback, System.Action<int> _removeAction)
        {
            loadSceneAction = _loadSceneCallback;
            joinAction = _joinCallback;
            removeAction = _removeAction;
        }


        /// <param name="Other">Class and variables</param>
        Model model = null;
        View view = null;
        bool ableToJoin = true;
        bool isStarting = false;
        string nextScene = "";

        /// <param name="Controller">Join and Remove feature.</param>
        System.Action<int> joinAction = null;
        System.Action<int> removeAction = null;
        List<Player> activePlayers = new List<Player>(); // Rewired "player"
        List<InputActionSourceData> activeController = new List<InputActionSourceData>(); // Rewired "controller"

        /// <param name="PlayerCharacter">Store player datas</param>
        List<PlayerContainer> containers = new List<PlayerContainer>();
        List<PlayerCharacter> activePlayerCharacter = new List<PlayerCharacter>();

        public IEnumerator StartCountDown()
        {
            isStarting = true;
            float startDuration = 3f; // Start in n sec.
            while (startDuration > 0)
            {
                if (activePlayerCharacter.Count > 0)
                {
                    Debug.Log("Start terminate");
                    isStarting = false;
                    startDuration = 999;
                    StopCoroutine(StartCountDown());
                    break;
                }
                view.UpdateTMP_Text(startDuration.ToString());
                startDuration--;
                yield return new WaitForSeconds(1);
            }

            view.UpdateTMP_Text("");

            ableToJoin = false;
            yield return new WaitForSeconds(0.1f);

            if (startDuration <= 0 && activePlayerCharacter.Count <= 0)
            {
                var a = UpdateCoreModel();
                yield return a;
                var b = MapPollResults();
                yield return b;
                GameStart();
            }
            yield return null;
        }
        public IEnumerator UpdateCoreModel()
        {
            List<GameObject> escaperPrefabs = new List<GameObject>();
            List<GameObject> hunterPrefabs = new List<GameObject>();

            string path = "TestAvatar/";

            foreach (PlayerContainer c in containers)
            {
                escaperPrefabs.Add(Resources.Load<GameObject>(path + c.escaperModel.name));
                hunterPrefabs.Add(Resources.Load<GameObject>(path + c.hunterModel.name));
            }

            CoreModel.EscaperPrefabs = escaperPrefabs;
            CoreModel.HunterPrefabs = hunterPrefabs;
            yield return null;
        }
        public IEnumerator MapPollResults()
        {
            var polls = new Dictionary<string, int>();

            foreach (PlayerContainer c in containers)
            {
                string key = c.choosenMap;
                if (polls.ContainsKey(key))
                {
                    polls[key]++;
                }
                else
                {
                    polls.Add(key, 1);
                }
            }
            nextScene = polls.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

            yield return null;
        }

        public void GameStart()
        {
            loadSceneAction(nextScene, true);
        }

        public void UpdateCameras()
        {
            List<Camera> cameras = new List<Camera>();
            foreach (PlayerContainer p in containers)
            {
                Camera cam = p.selfAvatar.GetComponentInChildren<Camera>();
                cameras.Add(cam);
            }
            cameras.Resize();
        }

        public void UndoModelActive(GameObject model)
        {
            if (model != null)
            {
                model.SetActive(true);
            }
        }

        public void StateBack(int id)
        {
            switch (containers.GetID(id).selfSelectState)
            {
                case SelectModelState.onChooseEscaper:
                    PlayerContainer _p = containers.GetID(id);

                    activePlayerCharacter.Remove(containers.GetID(id).selfScript);
                    Destroy(_p.selfAvatar);
                    containers.Remove(_p);

                    RemoveController(id);
                    break;
                case SelectModelState.onChooseHunter:
                    containers.GetID(id).selfSelectState--;
                    UndoModelActive(containers.GetID(id).escaperModel);
                    containers.GetID(id).escaperModel = null;
                    break;
                case SelectModelState.onChooseMap:
                    containers.GetID(id).selfSelectState--;
                    UndoModelActive(containers.GetID(id).hunterModel);
                    containers.GetID(id).hunterModel = null;
                    break;
                case SelectModelState.onWait:
                    containers.GetID(id).selfSelectState--;
                    PlayerCharacter _p1 = containers.GetID(id).selfScript;
                    containers.GetID(id).selfAvatar.SetActive(true);
                    activePlayerCharacter.Add(_p1);
                    break;
            }
        }

        public void SelectEscapser(int id, GameObject model)
        {
            if (containers.GetID(id).selfSelectState < SelectModelState.onChooseEscaper) return;

            if (containers.GetID(id).selfSelectState <= SelectModelState.onChooseEscaper)
                containers.GetID(id).selfSelectState++;

            UndoModelActive(containers.GetID(id).escaperModel);
            containers.GetID(id).escaperModel = model;
            model.SetActive(false);
        }

        public void SelectHunter(int id, GameObject model)
        {
            if (containers.GetID(id).selfSelectState < SelectModelState.onChooseHunter) return;

            if (containers.GetID(id).selfSelectState <= SelectModelState.onChooseHunter)
                containers.GetID(id).selfSelectState++;

            UndoModelActive(containers.GetID(id).hunterModel);
            containers.GetID(id).hunterModel = model;
            model.SetActive(false);
        }

        public void SelectMap(int id, GameObject map)
        {
            if (containers.GetID(id).selfSelectState < SelectModelState.onChooseMap) return;

            if (containers.GetID(id).selfSelectState <= SelectModelState.onWait)
                containers.GetID(id).selfSelectState++;

            containers.GetID(id).choosenMap = map.name;

            AllStepFinished(id);
        }

        public void AllStepFinished(int id)
        {
            PlayerCharacter _p = containers.GetID(id).selfScript;
            containers.GetID(id).selfAvatar.SetActive(false);
            activePlayerCharacter.Remove(_p);
            UpdateCameras();
        }

        public void RemoveController(int id)
        {
            Player player = ReInput.players.GetPlayer(id);

            activePlayers.Remove(player);
            if (activePlayers.Count < 4) ableToJoin = true;

            InputActionSourceData input = activeController[id];
            removeAction(id);
            activeController.Remove(input);
            ReInput.players.GetSystemPlayer().controllers.AddController(input.controller, true);

            player.controllers.RemoveController(input.controller);
            containers = containers.Where(container => container.id != id).ToList();

            UpdateCameras();
        }

        public void AssignController(Player player, InputActionSourceData source)
        {
            var controller = source.controller;

            activeController.Add(source);
            activePlayers.Add(player);

            player.controllers.AddController(controller, true);
            player.isPlaying = true;

            if (activePlayers.Count >= 4) ableToJoin = false;

            GetPlayerActive(player.id);
        }

        public void GetPlayerActive(int id)
        {
            PlayerContainer _c = new PlayerContainer();
            _c.id = id;
            _c.selfSelectState = SelectModelState.onChooseEscaper;
            containers.Add(_c);

            List<System.Action<int, GameObject>> actions = new List<System.Action<int, GameObject>>();
            actions.Add(SelectEscapser);
            actions.Add(SelectHunter);
            actions.Add(SelectMap);

            GameObject go = Instantiate(model.GetPlayerAvatar);
            go.transform.position = model.GetStartPos;

            PlayerCharacter _p = go.GetComponentInChildren<PlayerCharacter>();
            _p.AssignController(id, actions);
            activePlayerCharacter.Add(_p);

            containers.GetID(id).selfAvatar = go;
            containers.GetID(id).selfScript = _p;
            UpdateCameras();
        }

        private void Awake()
        {
            model = GetComponent<Model>();
            view = GetComponent<View>();
        }
        void Update()
        {
            if (ReInput.players.GetSystemPlayer().GetButtonDown("JoinGame"))
            {
                if (ableToJoin == false) return;
                AssignNextPlayer();
            }

            for (int i = 0; i < ReInput.players.playerCount; i++)
            {
                if (ReInput.players.GetPlayer(i).GetButtonDown("StateBack"))
                {
                    StateBack(i);
                }
            }

            if (activePlayers.Count > 0 && activePlayerCharacter.Count == 0)
            {
                if (isStarting) return;
                StartCoroutine(StartCountDown());
            }
        }

        void AssignNextPlayer()
        {
            // Get the Rewired Player
            Player rewiredPlayer = FindPlayerWithoutController();
            if (rewiredPlayer == null)
            {
                Debug.Log("Players Overload");
                return;
            }

            // Determine which Controller was used to generate the JoinGame Action
            Player systemPlayer = ReInput.players.GetSystemPlayer();
            var inputSources = systemPlayer.GetCurrentInputSources("JoinGame");
            if (inputSources == null) return;

            foreach (var source in inputSources)
            {
                if (activeController.Contains(source)) continue;

                if (source.controllerType == ControllerType.Keyboard || source.controllerType == ControllerType.Joystick)
                {
                    joinAction(rewiredPlayer.id);
                    AssignController(rewiredPlayer, source);
                    break;
                }
                else
                { // Custom Controller
                    throw new System.NotImplementedException();
                }
            }
        }

        private Player FindPlayerWithoutController()
        {
            foreach (Player p in ReInput.players.Players)
            {
                if (p.controllers.joystickCount > 0 || p.controllers.hasKeyboard)
                    continue;
                return p;
            }
            return null;
        }
    }

    public static class HallManagerExtension
    {
        public static PlayerContainer GetID(this List<PlayerContainer> source, int id)
        {
            return source.Find(element => element.id == id);
        }
    }
}