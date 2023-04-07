using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinLoseScreenController : MonoBehaviour
{
    [SerializeField] CanvasGroup opacityController;
    [SerializeField] TMP_Text winLoseText;
    [SerializeField] TMP_Text turnCount;

    public float fadeInTime = 0.5f;

    public void OnRestart()
    {

    }

    public void OnLevelSelect()
    {

    }

    GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        
    }

    public void Start()
    {
        enabled = false;
        opacityController.alpha = 0;
    }

    public void Show(bool win)
    {
        enabled = true;
        winLoseText.text = win switch
        {
            false => "You Lose!",
            true => "You Win!"
        };

        turnCount.text = $"Number of Turns: {gm.player.moveCnt}";

        IEnumerator fadeIn()
        {
            float time = 0;
            while (opacityController.alpha < 1)
            {
                opacityController.alpha = time / fadeInTime;
                time += Time.deltaTime;
                yield return null;
            }
        }

        StartCoroutine(fadeIn());
    }
}
