using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameManagerSpace.Menu
{
    public class View : MonoBehaviour, IMenuView
    {
        /// <summary>
        /// 1.<!--Setup button pressed feedback.-->
        /// 2.<!--Attatch ButtonHighLight to button and assign onClick function to button.-->
        /// </summary>
        /// <param name="ButtonHighLight">
        /// The 'script' attatchs to button which is used to dectect select or deselect
        /// </param>
        public Button CurrentHighLightButton { get; set; }
        [SerializeField] List<Button> buttons = new List<Button>();

        public void SelectCallback(Button btn)
        {
            if (btn == null) return;
            CurrentHighLightButton = btn;
            btn.Select();
        }
        public void InitButton()
        {
            CurrentHighLightButton = buttons[0];
            buttons[0].Select();

            foreach (Button btn in buttons)
            {
                btn.GetComponent<ButtonHighlighted>().Init(SelectCallback);
            }
        }
        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (CurrentHighLightButton == null) return;
                CurrentHighLightButton.Select();
            }
        }
    }
}