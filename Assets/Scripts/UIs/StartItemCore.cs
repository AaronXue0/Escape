using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartItemCore : MonoBehaviour
{
    [SerializeField] Image hintImage = null;
    [SerializeField] Sprite hintSprite = null;
    [SerializeField] string hintAnimationName = "HintButtonAnimation";
    [SerializeField] string selfTag = "Balloon";

    public void InstantSetup(Sprite imageSprite, string tagName)
    {
        GetComponent<SpriteRenderer>().sprite = imageSprite;
        hintImage.sprite = hintSprite;
        selfTag = tagName;
        transform.tag = selfTag;
    }

    private void Awake()
    {
        hintImage.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            hintImage.enabled = true;
            hintImage.GetComponent<Animator>().Play(hintAnimationName);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            hintImage.enabled = false;
            hintImage.GetComponent<Animator>().Play("Idle");
        }
    }
}
