using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Role.PlayerSpace.Game;
namespace GameManagerSpace.Game
{
    public class Control : MonoBehaviour, IGameControl
    {
        Model model = null;
        View view = null;

        public System.Action settingAction = null;
        public System.Action playingAction = null;

        private int getStartItemCounter = 0;
        private int getStartItemTarget = 0;
        private bool isGoaled = false;

        public void Init()
        {
            model = GetComponent<Model>();
            view = GetComponent<View>();

            model.ActiveInputs = new List<Player>();
            model.ActivePlayers = new List<PlayerCharacter>();
            model.CaughtPlayers = new List<PlayerCharacter>();
            model.GoalPlayers = new List<PlayerCharacter>();
            model.SelectAsHunter = new List<PlayerCharacter>();
            model.SelectAsEscaper = new List<PlayerCharacter>();
        }

        /// <param name="Callback">Callback assign to players.</param>
        public void GetStartItemCallback(PlayerCharacter role)
        {
            model.SelectingItemPlayers.Remove(role);
            if (getStartItemCounter >= getStartItemTarget)
                return;

            getStartItemCounter++;

            if (getStartItemCounter == getStartItemTarget)
                settingAction();
        }
        public void GetCaught(PlayerCharacter role)
        {
            model.CaughtPlayers.Add(role);
            if (model.CaughtPlayers.Count == model.SelectAsEscaper.Count)
            {
                StartCoroutine(PlayingCoroutine(0));
            }
        }
        public void GetGoal(PlayerCharacter role)
        {
            model.GoalPlayers.Add(role);

            if (isGoaled) return;
            isGoaled = true;
            StartCoroutine(PlayingCoroutine(1));
        }

        /// <param name="Setting">Game is setting.</param>
        public IEnumerator RandomRoom()
        {
            List<Transform> roomPrefabs = new List<Transform>();
            List<int> seed = new List<int>();
            Transform startPos = model.GetTransformPool(model.GetChildCount.firstRoomSP);

            foreach (Transform t in model.GetRoomPool)
            {
                roomPrefabs.Add(t);
            }

            int index = 0;
            seed = roomPrefabs.RandomSeed(roomPrefabs.Count);
            foreach (int s in seed)
            {
                yield return null;
                Vector2 pos = (index == 0) ?
                    startPos.position :
                    roomPrefabs[seed[index - 1]].GetChild(model.GetChildCount.roomEndPos).position
                ;
                roomPrefabs[seed[index]].transform.position = pos;
                roomPrefabs[seed[index]].gameObject.SetActive(true);
                index++;
            }

            for (int i = 0; i < roomPrefabs.Count; i++)
            {
                if (seed.Contains(i)) continue;
                roomPrefabs[i].gameObject.SetActive(false);
            }

            model.GetDestinationRoom.position = roomPrefabs[seed[index - 1]].GetChild(model.GetChildCount.roomEndPos).position;
        }
        public IEnumerator RandomStartItems()
        {
            List<Transform> startItems = new List<Transform>();
            List<int> seed = new List<int>();
            foreach (Transform t in model.GetStartItemPool)
            {
                if (t.gameObject.activeSelf == false) t.gameObject.SetActive(true);
                startItems.Add(t);
            }
            seed = model.GetAllStartItems.RandomSeed(3);
            int index = 0;
            foreach (Transform t in startItems)
            {
                yield return null;
                int n = seed[index];
                string name = model.GetAllStartItems[n].itemTagName;
                Sprite image = model.GetAllStartItems[n].image;
                t.gameObject.GetComponent<StartItemCore>().InstantSetup(image, name);
                index++;
            }
        }
        public IEnumerator SpawnPlayers()
        {
            List<Player> activeInputs = new List<Player>();
            List<PlayerCharacter> activePlayers = new List<PlayerCharacter>();

            List<PlayerCharacter> selectAsHunter = new List<PlayerCharacter>();
            List<PlayerCharacter> selectAsEscaper = new List<PlayerCharacter>();

            List<Camera> cameras = new List<Camera>();

            Vector2 hunterSP = model.GetTransformPool(model.GetChildCount.hunterSP).position;
            Vector2 escaperSP = model.GetTransformPool(model.GetChildCount.escaperSP).position;

            int rnd = Random.Range(0, model.ActivePlayers.Count);
            int index = 0;

            foreach (Player p in ReInput.players.AllPlayers.GetActivePlayers())
            {
                List<System.Action<PlayerCharacter>> actions = new List<System.Action<PlayerCharacter>>();

                GameObject playerPrefab = Instantiate(
                    (index == rnd) ?
                    CoreModel.HunterPrefabs[index] :
                    CoreModel.EscaperPrefabs[index]
                );
                cameras.Add(playerPrefab.GetComponentInChildren<Camera>());

                activeInputs.Add(p);

                PlayerCharacter _p = playerPrefab.GetComponentInChildren<PlayerCharacter>();
                activePlayers.Add(_p);

                _p.AssignController(p.id);

                if (index == rnd)
                {
                    actions.Add(null);
                    actions.Add(GetCaught);
                    actions.Add(GetGoal);
                    _p.AssignTeam(1, actions);
                    _p.transform.position = hunterSP;
                    selectAsHunter.Add(_p);
                }
                else
                {
                    actions.Add(GetStartItemCallback);
                    actions.Add(GetCaught);
                    actions.Add(GetGoal);
                    getStartItemTarget++;
                    _p.AssignTeam(0, actions);
                    _p.transform.position = escaperSP;
                    selectAsEscaper.Add(_p);
                }

                index++;
                yield return null;
            }

            model.ActiveInputs = activeInputs;
            model.ActivePlayers = activePlayers;

            model.SelectAsHunter = selectAsHunter;
            model.SelectAsEscaper = selectAsEscaper;

            cameras.Resize();
        }
        /// <param name="Starting">Game is starting.</param>
        public IEnumerator EscaperStart()
        {
            int counter = (int)model.GetEscaperDuration;
            List<PlayerCharacter> escapers = model.SelectAsEscaper;
            while (counter > 0)
            {
                yield return new WaitForSeconds(1);
                view.UpdateCountingDownView(escapers, counter.ToString());
                counter--;
            }
            yield return new WaitForSeconds(1);
            view.UpdateCountingDownView(escapers, "0");
            yield return new WaitForSeconds(1);
            view.UpdateCountingDownView(escapers, "");
            foreach (PlayerCharacter role in escapers)
            {
                // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
                // ███  New Feature  ███████████████████████████████████████████████████████████████████████████████████████████████████
                // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
            }
        }
        public IEnumerator HunterStart()
        {
            float counter = (int)model.GetHunterDuration;
            List<PlayerCharacter> hunters = model.SelectAsHunter;
            while (counter > 0)
            {
                yield return new WaitForSeconds(1);
                view.UpdateCountingDownView(hunters, counter.ToString());
                counter--;
            }
            yield return new WaitForSeconds(1);
            view.UpdateCountingDownView(hunters, "0");
            yield return new WaitForSeconds(1);
            view.UpdateCountingDownView(hunters, "");
            foreach (PlayerCharacter role in hunters)
            {
                // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
                // ███  New Feature  ███████████████████████████████████████████████████████████████████████████████████████████████████
                // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
            }
        }
        /// <param name="Playing">Game is playing.</param>
        public IEnumerator Playing()
        {
            yield return null;
        }
        public IEnumerator PlayingCoroutine(int mode)
        {
            var delay = StartCoroutine(PlayFinished());
            switch (mode)
            {
                case 0:
                    yield return delay;
                    break;
                case 1:
                    var countDown = StartCoroutine(DelayAndBroadcast(10, true));
                    yield return countDown;
                    yield return delay;
                    break;
                default:
                    Debug.Log("Game playing finish mode not registered yet");
                    break;
            }
            playingAction();
            yield return null;
        }
        public IEnumerator PlayFinished()
        {
            var delay = StartCoroutine(DelayAndBroadcast(3, false));
            view.UpdateCountingDownView(model.ActivePlayers, "Game Over");
            yield return delay;
        }
        public IEnumerator DelayAndBroadcast(int counter, bool doCast)
        {
            while (counter >= 0)
            {
                yield return new WaitForSeconds(1);
                if (doCast) view.UpdateCountingDownView(model.ActivePlayers, counter.ToString());
                counter--;
            }
        }
        /// <param name="Game over">This round is over.</param>
        public IEnumerator Scoring()
        {
            // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
            // ███  New Feature  ███████████████████████████████████████████████████████████████████████████████████████████████████
            // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
            yield return null;
        }
    }
}