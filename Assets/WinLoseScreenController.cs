using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLoseScreenController : MonoBehaviour
{
    [SerializeField] FadeInOut fadeSelf;
    [SerializeField] FadeInOut blackOut;
    [SerializeField] TMP_Text winLoseText;
    [SerializeField] TMP_Text turnCount;

    public void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnLevelSelect()
    {
        IEnumerator tmp()
        {
            yield return blackOut.FadeIn();

            SceneManager.LoadScene("Level Select");
        }
        StartCoroutine(tmp());
    }

    GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void Show(bool win)
    {
        winLoseText.text = win switch
        {
            false => "You Lose!",
            true => "You Win!"
        };

        turnCount.text = $"Number of Turns: {gm.player.moveCnt}";

        StartCoroutine(fadeSelf.FadeIn());
    }
}
