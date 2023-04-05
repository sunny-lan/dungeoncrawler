using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    private GameManager gameManager;
    public bool collectibleInZombieMode = false;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.RegisterKey(this);
    }

    private void Start()
    {
        gameManager.player.onChangePos.AddListener(OnPlayerChangePos);
    }

    private void OnPlayerChangePos(Vector2Int keyPos)
    {
        Vector2Int myPos = new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
        if (myPos == keyPos && (collectibleInZombieMode || !gameManager.player.isZombie))
        {
            gameManager.OnPlayerCollectedKey(this);
            gameManager.player.OnCollectedKey(this);
            Destroy(gameObject);
        }
    }
}
