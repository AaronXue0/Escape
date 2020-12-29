using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Role.PlayerSpace.Game;
using Rewired;

namespace GameManagerSpace.Game
{
    /// <summary>
    /// Each child object count under their parent without knowing who they are.
    /// </summary>
    /// <param name="TransformPoolChildCount">Record child count of object</param>
    [System.Serializable]
    public class TransformPoolChildCount
    {
        public int hunterSP = 0;
        public int escaperSP = 1;
        public int firstRoomSP = 2;
        public int roomEndPos = 1;
        public int roomSP = 2;
    }
    /// <summary>
    /// Store object used by start item to a "List".
    /// </summary>
    /// <param name="StartItemContainer">Start Item Usage</param>
    [System.Serializable]
    public class StartItemContainer
    {
        public int id = 0;
        public Sprite image = null;
        public string itemTagName = "";
    }
    public class Model : MonoBehaviour
    {
        /// <summary>
        /// Trasnfroms and their child count.
        /// </summary>
        /// <param name="Object">Store every pools</param>
        public Transform GetStartItemPool { get { return startItemPool; } }
        [SerializeField] Transform startItemPool = null;
        public List<StartItemContainer> GetAllStartItems { get { return allStartItems; } }
        [SerializeField] List<StartItemContainer> allStartItems = new List<StartItemContainer>();
        public Transform GetRoomPool { get { return roomPool; } }
        [SerializeField] Transform roomPool = null;
        public Transform GetTransformPool(int id) { return transformPool.GetChild(id); }
        [SerializeField] Transform transformPool = null;
        public Transform GetDestinationRoom { get { return destinationRoom; } }
        [SerializeField] Transform destinationRoom = null;
        public TransformPoolChildCount GetChildCount { get { return childCount; } }
        [SerializeField] TransformPoolChildCount childCount = new TransformPoolChildCount();


        /// <summary>
        /// Store players of input, class and prefab.
        /// </summary>
        /// <param name="List">Input and players</param>
        public List<Player> ActiveInputs { get; set; }
        public List<PlayerCharacter> ActivePlayers { get; set; }
        public List<PlayerCharacter> SelectingItemPlayers { get; set; }
        public List<PlayerCharacter> SelectAsHunter { get; set; }
        public List<PlayerCharacter> SelectAsEscaper { get; set; }
        public List<PlayerCharacter> CaughtPlayers { get; set; }
        public List<PlayerCharacter> GoalPlayers { get; set; }

        /// <summary>
        /// Starting Values
        /// </summary>
        /// <param name="Int">Values used when game starting</param>
        public float GetEscaperDuration { get { return escaperDuration; } }
        [SerializeField] float escaperDuration = 3f;
        public float GetHunterDuration { get { return hunterDuration; } }
        [SerializeField] float hunterDuration = 3f;
    }
}