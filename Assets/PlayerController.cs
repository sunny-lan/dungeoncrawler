using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerController : GridEntity
{
    private Camera playerCam;
    [SerializeField] TMP_Text statusText;
    [SerializeField] ChoiceList actionsList;

    protected override void Awake()
    {
        base.Awake();

        playerCam = GetComponentInChildren<Camera>();

        onChangeZombieStatus.AddListener(_ => UpdatePossibleActions());
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

    bool TryMove(Vector2Int newPosition)
    {
        if (gameManager.IsWalkable(newPosition))
        {
            pos = newPosition;

            // Determine height by raycasting down
            float y = transform.position.y;
            if(Physics.Raycast(raycastCenter.position, Vector3.down, out var hitInfo, 2, LayerMask.GetMask("Floor")))
            {
                y = hitInfo.point.y;
            }
            else
            {
                Debug.Assert(false, "Raycast to find floor failed");
            }

            transform.position = new(pos.x, y, pos.y);
            OnDidMove();
            return true;
        }
        return false;
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
    public float punchRange = 1.5f;

    public float zombiePunchStrength = 10, humanPunchStength = 1;


    GameObject curRaycastedObj;
    RaycastHit curRaycastInfo;

    // Detect if player is hovering over something with their crosshair,

    void DoPlayerCrosshairRaycast()
    {
        var center = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Physics.Raycast(center, out var hitInfo, 50, ~LayerMask.GetMask("Player"));

        GameObject hitObj = hitInfo.collider?.gameObject;

        if (hitObj != curRaycastedObj)
        {
            curRaycastedObj?.GetComponentInChildren<Outline>()?.SetEnabled(false);

            curRaycastInfo = hitInfo;
            curRaycastedObj = hitObj;
            UpdatePossibleActions();
        }
    }

    List<PlayerAction> possibleActions = new();
    void UpdatePossibleActions()
    {
        possibleActions.Clear();

        GameObject hitObj = curRaycastedObj;
        if (hitObj != null)
        {
            var gridEntity = hitObj.GetComponent<GridEntity>();
            if (gridEntity != null)
            {
                if (isZombie)
                {
                    if (!gridEntity.isZombie && curRaycastInfo.distance <= biteRange)
                    {
                        possibleActions.Add(new()
                        {
                            name = "Bite",
                            action = () => Bite(gridEntity),
                        });
                    }
                }
                else
                {
                    if(curRaycastInfo.distance <= gunRange)
                        possibleActions.Add(new()
                        {
                            name = "Shoot",
                            action = () =>
                            {
                                float bulletDmg = 50; // TODO
                                gridEntity.health -= bulletDmg;
                            },
                        });
                }
            }

            var punchable = hitObj.GetComponentInChildren<IPunchable>();
            if(punchable != null && ((MonoBehaviour)punchable).enabled)
            {
                if (curRaycastInfo.distance <= punchRange)
                    possibleActions.Add(new()
                    {
                        name = "Punch",
                        action = () =>
                        {
                            punchable.GetPunched(this, 
                                isZombie ? zombiePunchStrength : humanPunchStength,
                                curRaycastInfo.point - playerCam.transform.position);
                        }
                    });
            }
        }

        UpdatePossibleActionsUI();
    }

    public List<InputTrigger> inputTriggers = new()
    {
        new()
        {
            name = "1",
            keyCode = KeyCode.Alpha1,
        },
        new()
        {
            name = "2",
            keyCode = KeyCode.Alpha2,
        },
        new()
        {
            name = "3",
            keyCode = KeyCode.Alpha3,
        },
        new()
        {
            name = "4",
            keyCode = KeyCode.Alpha4,
        },
        new()
        {
            name = "5",
            keyCode = KeyCode.Alpha5,
        }
    };

    void UpdatePossibleActionsUI()
    {
        curRaycastedObj?.GetComponentInChildren<Outline>()?.SetEnabled(possibleActions.Count > 0);
       
        actionsList.SetItems(possibleActions.Select((action, idx) =>
        {
            return (inputTriggers[idx].name,action.name);
        }));
    }

    bool CheckActions()
    {
        for(int i = 0; i < possibleActions.Count; i++)
        {
            if (inputTriggers[i].isDown())
            {
                possibleActions[i].action();
                return true;
            }
        }

        return false;
    }

    bool CheckPlayerWalk()
    {
        Vector2Int facing = GetFacingDir();

        // Check inputs
        if (Input.GetKeyDown(KeyCode.W)) return TryMove(pos + facing);
        else if (Input.GetKeyDown(KeyCode.A)) return TryMove(pos + facing.Left());
        else if (Input.GetKeyDown(KeyCode.S)) return TryMove(pos + facing.Back());
        else if (Input.GetKeyDown(KeyCode.D)) return TryMove(pos + facing.Right());

        return false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        DoPlayerCrosshairRaycast();

        if (CheckActions()) return;
        if (CheckPlayerWalk()) return;

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

public interface IPunchable
{
    public void GetPunched(GridEntity by, float strength, Vector3 direction);
}

public struct PlayerAction
{
    public string name;
    public Action action;
}

public struct InputTrigger
{
    public string name;
    public KeyCode keyCode;

    public bool isDown()
    {
        return Input.GetKeyDown(keyCode);
    }
}