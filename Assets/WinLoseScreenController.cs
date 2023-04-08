using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLoseScreenController : MonoBehaviour
{
    [SerializeField] FadeInOut fadeSelf;
    [SerializeField] TMP_Text winLoseText;
    [SerializeField] TMP_Text turnCount;
    [SerializeField] TMP_Text helpMsg;

    GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void Show(bool win, string helpmsg = "")
    {
        helpMsg.text = helpmsg;
        winLoseText.text = win switch
        {
            false => "ZOMBIFIED",
            true => "ESCAPED"
        };

        turnCount.text = $"Number of Turns: {gm.player.moveCnt}";

        StartCoroutine(fadeSelf.FadeIn());
    }
}
