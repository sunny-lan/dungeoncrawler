using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : GridEntity
{
    private Camera playerCam;
    [SerializeField] TMP_Text statusText;
    [SerializeField] ChoiceList actionsList;

    public float hpDrainPerTurn = 0.5f;

    protected override void Awake()
    {
        base.Awake();

        playerCam = GetComponentInChildren<Camera>();
        canvas = GetComponentInChildren<Canvas>().GetComponent<RectTransform>();

        onChangeZombieStatus.AddListener(_ => UpdatePossibleActions());

        var playerDamagedIndicator = GetComponentInChildren<PulseImageColor>();
        float oldhealth = health;
        onChangeHealth.AddListener(health =>
        {
            UpdateStatus();

            if (health < oldhealth)
                playerDamagedIndicator.Trigger((oldhealth - health) / 20);

            if (health <= -maxHealth)
            {
                gameManager.HandlePlayerWinLose(false);
            }

            oldhealth = health;
        });
    }

    public void DisableControls()
    {
        playerCam.GetComponent<CameraLook>().enabled = false;
        canvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        biteIndicator.transform.parent = null;
        UpdateStatus();
    }

    public int moveCnt { get; private set; } = 0;

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
            //float y = transform.position.y;
            //if (Physics.Raycast(raycastCenter.position, Vector3.down, out var hitInfo, 2, LayerMask.GetMask("Floor")))
            //{
            //    y = hitInfo.point.y;
            //}
            //else
            //{
            //    Debug.Assert(false, "Raycast to find floor failed");
            //}

            transform.position = new(pos.x, 0, pos.y);
            OnDidTurn();
            return true;
        }
        return false;
    }

    void OnDidTurn()
    {
        if (gameManager.playerWinState != null) return;

        health -= hpDrainPerTurn;
        moveCnt++;
        if (moveCnt % 2 == 0)
        {
            gameManager.DoEnemyTurn();
        }
        else
        {
            gameManager.DoEnemyTelegraph();
        }
        UpdatePossibleActions();
    }

    //public float biteRange = 1.5f;
    //public float gunRange = 10f;
    float punchRange = 1.5f;
    float zombiePunchStrength = 10, humanPunchStength = 1;


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
                    if (!gridEntity.isZombie && gridEntity.pos.Manhattan(pos) <= 1)
                    {
                        possibleActions.Add(new BiteAction(gridEntity, this)
                        {
                            keybind = new MouseTrigger()
                            {
                                mouseBtn = 0
                            }
                        });
                    }
                }
                else
                {
                    //if (curRaycastInfo.distance <= gunRange)
                    //    possibleActions.Add(new()
                    //    {
                    //        name = "Shoot",
                    //        action = () =>
                    //        {
                    //            float bulletDmg = 50; // TODO
                    //            gridEntity.health -= bulletDmg;
                    //        },
                    //    });
                }
            }

            var punchable = hitObj.GetComponentInChildren<IPunchable>();
            if (punchable != null && ((MonoBehaviour)punchable).enabled)
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
                        },
                        keybind = new KeyTrigger()
                        {
                            keyCode = KeyCode.P,
                        }
                    });
            }
        }

        possibleActions.Add(new PlayerAction()
        {
            name = "Skip Turn",
            action = () => { },
            keybind = new MouseTrigger()
            {
                mouseBtn = 1,
            }
        });

        UpdatePossibleActionsUI();
    }


    [SerializeField] GameObject biteIndicator;
    private RectTransform canvas;

    void UpdatePossibleActionsUI()
    {
        curRaycastedObj?.GetComponentInChildren<Outline>()?.SetEnabled(possibleActions.Count > 0);

        actionsList.SetItems(possibleActions.Select((action, idx) =>
        {
            return (possibleActions[idx].keybind.name, action.name.ToString());
        }));

        Vector2Int? bitePos = null;
        foreach (var action in possibleActions)
        {
            if (action is BiteAction biteAction)
            {
                bitePos = biteAction.victim.pos;
            }
        }

        if (bitePos is Vector2Int bite)
        {
            biteIndicator.SetActive(true);
            biteIndicator.transform.position = new(bite.x, 0, bite.y);
        }
        else
        {
            biteIndicator.SetActive(false);

        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(canvas);
    }

    bool CheckActions()
    {
        for (int i = 0; i < possibleActions.Count; i++)
        {
            if (possibleActions[i].keybind.isDown() == true)
            {
                possibleActions[i].action();
                OnDidTurn();
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
        {
            if (gameManager.UnfoundKeys.Count > 0)
                statusText.text = $"Find {gameManager.UnfoundKeys.Count} remaining keys to unlock exit";
            else
                statusText.text = $"All keys found. Find the exit and escape.";
        }
    }

    [SerializeField] TMP_Text txtKeysFound;

    internal void OnCollectedKey(KeyController key)
    {
        UpdateStatus();
        //txtKeysFound.text = $"Keys Found: {keysFound} / {gameManager.AllKeys.Count}";
    }
}

public interface IPunchable
{
    public void GetPunched(GridEntity by, float strength, Vector3 direction);
}

public class PlayerAction
{

    public string name;
    public Action action;

    public InputTrigger keybind;
}

public class BiteAction : PlayerAction
{
    public readonly GridEntity victim;

    public BiteAction(GridEntity victim, GridEntity player)
    {
        this.victim = victim;
        name = "Bite";
        action = () => player.Bite(victim);
    }
}

public abstract class InputTrigger
{
    public abstract string name { get; }

    public abstract bool isDown();
}

public class KeyTrigger : InputTrigger
{

    public KeyCode keyCode;

    public override string name => keyCode.ToString();

    public override bool isDown()
    {
        return Input.GetKeyDown(keyCode);
    }

}

public class MouseTrigger : InputTrigger
{
    public int mouseBtn;

    public override string name => mouseBtn switch
    {
        0 => "LMB",
        1 => "RMB",
        2 => "MMB",
        _ => throw new NotImplementedException(),
    };

    public override bool isDown()
    {
        return Input.GetMouseButtonDown(mouseBtn);
    }
}