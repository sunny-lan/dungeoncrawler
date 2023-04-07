using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExitController : MonoBehaviour
{
    [SerializeField] Material openMat;
    [SerializeField] Material closedMat;
    [SerializeField] new Renderer renderer;
    [SerializeField] TextMeshPro text;
    [SerializeField] Color openColor;
    [SerializeField] Color closedColor;

    private GameManager gameManager;
    bool locked = true;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        renderer.material = closedMat;
        text.color = closedColor;
        gameManager.player.onChangePos.AddListener(_=>CheckPlayerWin());
        gameManager.player.onChangeZombieStatus.AddListener(_ => CheckPlayerWin());
    }

    public void UpdateKeyCount(int collected, int total)
    {
        text.text = $"EXIT LOCKED\n{collected}/{total} keys found";
    }

    public void Unlock()
    {
        locked = false;
        text.text = $"EXIT UNLOCKED";
        renderer.material = openMat;
        text.color = openColor;
    }

    private void CheckPlayerWin()
    {
        var playerPos = gameManager.player.pos;
        var myPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
        if (myPos == playerPos && !locked && !gameManager.player.isZombie)
        {
            Debug.Log("LEVEL COMPLETE");
            gameManager.HandlePlayerWinLose(true);
        }
    }
}
