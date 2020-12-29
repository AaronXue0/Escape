using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Role.PlayerSpace.Game
{
    [System.Serializable]
    public class HealthBarImageGroup
    {
        public GameObject GetPlayerCanvas { get { return playerCanvas; } }
        public Image HealthBar { get { return healthBar; } }
        [SerializeField] GameObject playerCanvas = null;
        [SerializeField] Image healthBar = null;
    }
    public class View : MonoBehaviour
    {
        [SerializeField] Image balloon = null;
        Vector3 balloonPosition;
        public HealthBarImageGroup healthBarImageGroup;
        public void UpdateHealthBar(float amount)
        {
            if (transform.localScale.x < 0)
                healthBarImageGroup.GetPlayerCanvas.transform.localScale = new Vector3(-1, 1, 1);
            else if (transform.localScale.x > 0)
                healthBarImageGroup.GetPlayerCanvas.transform.localScale = new Vector3(1, 1, 1);
            healthBarImageGroup.HealthBar.fillAmount = amount;
        }
        public void UpdaetShaderRenderer(string effect)
        {
            Material mat = GetComponent<Renderer>().material;
            mat.EnableKeyword(effect);
        }
        private void Start()
        {
            balloon.enabled = false;
            balloonPosition = new Vector3(-0.5f, 1, 0);
        }
        public void UpdateBalloon(bool flag)
        {
            balloonPosition = new Vector3(-0.5f * transform.localScale.x, 1, 0);
            balloon.enabled = flag;
            balloon.transform.localPosition = balloonPosition;
            balloon.transform.localScale = transform.localScale;
        }
    }
}