using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HumansRemainingDisplay : MonoBehaviour
{
    private GameManager gameManager;
    private TMP_Text status;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        status = GetComponent<TMP_Text>();
        gameManager.onHumansRemainingChanged.AddListener(count => status.text = $"{count} HUMANS REMAIN."); ;
    }
}
