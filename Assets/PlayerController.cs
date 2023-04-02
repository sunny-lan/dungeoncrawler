using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : GridEntity
{
    private GameManager gameManager;
    private Camera camera;
    private PostProcessingController postproc;

    protected override void Awake()
    {
        base.Awake();

        gameManager = FindObjectOfType<GameManager>();
        camera = GetComponentInChildren<Camera>();
        postproc = FindObjectOfType<PostProcessingController>();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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
        Vector2 forward2d = new(camera.transform.forward.x, camera.transform.forward.z);
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
    }

    public float biteRange = 1.5f;
    public float gunRange = 10f;

    Outline lastOutline;

    void CheckPlayerAction()
    {
        float range = GetIsZombie() ? biteRange : gunRange;

        var center =  camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        bool didHitEnemy = Physics.Raycast(center, out var hitInfo, range, ~LayerMask.GetMask("Player"));


        GameObject hitObj = hitInfo.collider?.gameObject;
        didHitEnemy = didHitEnemy && (hitObj.layer == LayerMask.NameToLayer("Enemy"));

        if (!didHitEnemy)
        {
            // Clear outline since raycast didn't hit
            if (lastOutline != null)
            {
                lastOutline.enabled = false;
                lastOutline = null;
            }

            return;
        }

        Outline hitOutline = hitObj.GetOrAddComponent<Outline>();
        if (hitOutline != lastOutline)
        {
            hitOutline.OutlineWidth = 5;
            hitOutline.enabled = true;

            // Clear old outline
            if (lastOutline != null)
            {
                lastOutline.enabled = false;
            }

            lastOutline = hitOutline;
        }

        if (!Input.GetMouseButtonDown(0))
            return;

        var gridEntity = hitObj.GetComponent<GridEntity>();
        if (GetIsZombie())
        {
            if (gridEntity.GetIsZombie())
                return;

            gridEntity.GetBitten(this);
            bitesToHuman--;
            if(bitesToHuman == 0)
            {
                SetIsZombie(false);
            }
        }
        else
        {
            float bulletDmg = 1; // TODO
            gridEntity.GetDamaged(this, bulletDmg);
        }

        OnDidMove();
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

    public int bitesToHuman = 3;

    int needToBite;

    public override void GetBitten(GridEntity by)
    {
        if (GetIsZombie())
        {
            Debug.Log("Zombie shouldn't be bitten");
            return;
        }

        needToBite = bitesToHuman;
        base.GetBitten(by);
    }

    public override void SetIsZombie(bool isZomb)
    {
        postproc.ChangeVolumeProfile(isZomb);

        base.SetIsZombie(isZomb);
    }

}
