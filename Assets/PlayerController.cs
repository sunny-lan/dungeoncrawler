using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : GridEntity
{
    private Camera playerCam;
    [SerializeField] TMP_Text statusText;

    protected override void Awake()
    {
        base.Awake();

        playerCam = GetComponentInChildren<Camera>();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        UpdateStatus();
    }

    int moveCnt = 0;

    Vector2Int[] dirs =
    {
        Vector2Int.up,
        Vector2Int.left,
        Vector2Int.down,
        Vector2Int.right
    };

    Vector2Int GetFacingDir()
    {
        Vector2Int best = default;
        Vector2 forward2d = new(playerCam.transform.forward.x, playerCam.transform.forward.z);
        float max = float.NegativeInfinity;
        foreach (var dir in dirs)
        {
            var dot = Vector2.Dot(dir, forward2d);
            if (dot > max)
            {
                max = dot;
                best = dir;
            }
        }

        return best;
    }

    void TryMove(Vector2Int newPosition)
    {
        if (gameManager.IsWalkable(newPosition))
        {
            pos = newPosition;

            transform.position = new(pos.x, 0, pos.y);
            OnDidMove();
        }
    }

    void OnDidMove()
    {
        moveCnt++;
        if (moveCnt % 2 == 0)
        {
            gameManager.DoEnemyTurn();
        }

        gameManager.DoEnemyTelegraph();
    }

    public float biteRange = 1.5f;
    public float gunRange = 10f;

    Outline lastOutline;

    void CheckPlayerAction()
    {
        float range = isZombie ? biteRange : gunRange;

        var center = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Physics.Raycast(center, out var hitInfo, range, ~LayerMask.GetMask("Player"));

        GameObject hitObj = hitInfo.collider?.gameObject;
        Outline hitOutline = hitObj?.GetComponentInChildren<Outline>();

        if (hitOutline != lastOutline)
        {
            if (hitOutline != null)
            {
                hitOutline.OutlineWidth = 5;
                hitOutline.enabled = true;
            }

            // Clear old outline
            if (lastOutline != null)
            {
                lastOutline.enabled = false;
            }

            lastOutline = hitOutline;
        }


        if (!Input.GetMouseButtonDown(0))
            return;

        if (hitObj && hitObj.layer == LayerMask.NameToLayer("Enemy"))
        {
            var gridEntity = hitObj.GetComponent<GridEntity>();
            if (isZombie)
            {
                if (gridEntity.isZombie)
                    return;

                Bite(gridEntity);
            }
            else
            {
                float bulletDmg = 50; // TODO
                gridEntity.health -= bulletDmg;
            }

            OnDidMove();
        }
    }

    void CheckPlayerWalk()
    {
        Vector2Int facing = GetFacingDir();

        // Check inputs
        if (Input.GetKeyDown(KeyCode.W)) TryMove(pos + facing);
        else if (Input.GetKeyDown(KeyCode.A)) TryMove(pos + facing.Left());
        else if (Input.GetKeyDown(KeyCode.S)) TryMove(pos + facing.Back());
        else if (Input.GetKeyDown(KeyCode.D)) TryMove(pos + facing.Right());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        CheckPlayerAction();
        CheckPlayerWalk();

    }

    void UpdateStatus()
    {
        if (isZombie)
            statusText.text = $"You are a zombie. Bite {needToBite} humans to revive";
        else
            statusText.text = $"You are a human.";
    }

    public override void Bite(GridEntity victim)
    {
        base.Bite(victim);
        UpdateStatus();
    }

    public override void GetBitten(GridEntity by)
    {
        base.GetBitten(by);
        UpdateStatus();
    }
}
