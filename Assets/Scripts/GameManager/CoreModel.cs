using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagerSpace
{
    public class CoreModel : MonoBehaviour
    {
        public static List<GameObject> EscaperPrefabs { get; set; }
        public static List<GameObject> HunterPrefabs { get; set; }
        public static List<ScoreContainer> Scores { get; set; }
    }

    public class ScoreContainer
    {
        public int selfScore = 0;
        public int escapeScore = 0;
        public int getPlayerScore = 0;
        
        //...
        //...
        //...
    }
}