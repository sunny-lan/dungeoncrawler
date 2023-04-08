using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class EnemyController : GridEntity
{
    enum StunStatus { WAIT_NEXT_TELEGRAPH, WAIT_NEXT_TURN, NORMAL };
    Vector2Int lastMoveDir = Vector2Int.up;
    [SerializeField][Range(0, 1)] float randomMoveChance = 0.5f;
    [SerializeField] EnemyTelegraphController telegraphController;


    [Header("Models and animations")]

    [SerializeField] GameObject zombieModel;
    [SerializeField] GameObject guardModel;

    Animator zombieAnimator;
    Animator guardAnimator;

    [SerializeField] Renderer zombieHead;
    [SerializeField] Renderer guardHead;
    [SerializeField] AttachToHead attachToHead;

    [System.Serializable]
    struct Animations
    {
        public RuntimeAnimatorController hostile, flee, stunned, idle;
    }

    [SerializeField] Animations zombieAnimations;
    [SerializeField] Animations guardAnimations;

    StunStatus stunStatus = StunStatus.NORMAL;

    private EmotionType _emotion;
    public EmotionType emotion
    {
        get => _emotion; private set
        {
            var oldVal = _emotion;
            _emotion = value;
            telegraphController.SetEmotion(emotion);
            if (oldVal != value)
                UpdatePose();
        }
    }

    Animator curAnimator => isZombie ? zombieAnimator : guardAnimator;

    private void UpdatePose()
    {
        Animations curAnimations = isZombie ? zombieAnimations : guardAnimations;

        // TODO this is a really scuffed way to control animations...
        curAnimator.runtimeAnimatorController = emotion switch
        {
            EmotionType.HOSTILE => curAnimations.hostile,
            EmotionType.FLEE => curAnimations.flee,
            EmotionType.STUNNED => curAnimations.stunned,
            EmotionType.IDLE => curAnimations.idle,
            _ => throw new System.NotImplementedException(),
        };

        if (emotion is EmotionType.IDLE or EmotionType.STUNNED)
        {
            // Idle animations can be played continuously
            curAnimator.speed = 1;
        }
        else
        {
            // Walking and running animations for guard are paused slowed down
            // because they look weird
            // Zombie walking animation looks okay, keep full animation speed
            curAnimator.speed = isZombie ? 1 : 0.02f;
        }
    }

    public bool freezeUntilPlayerClose = false;
    public float freezeUntilPlayerWithin = 5;

    protected override void Awake()
    {
        base.Awake();

        zombieAnimator = zombieModel.GetComponent<Animator>();
        guardAnimator = guardModel.GetComponent<Animator>();

        onChangeZombieStatus.AddListener(isZombie =>
        {
            zombieModel.SetActive(isZombie);
            guardModel.SetActive(!isZombie);

            attachToHead.head = isZombie ? zombieHead : guardHead;
        });

        var dirs = new List<Vector2Int>()
            {
                Vector2Int.up,
                Vector2Int.left,
                Vector2Int.down,
                Vector2Int.right
            };

        lastMoveDir = dirs[Random.Range(0, 4)];
    }

    protected override void Start()
    {
        base.Start();

        UpdatePose();
    }

    public void TelegraphTurn()
    {
        if (stunStatus == StunStatus.WAIT_NEXT_TELEGRAPH)
            stunStatus = StunStatus.WAIT_NEXT_TURN;

        if (stunStatus != StunStatus.NORMAL)
            return;

        if (isZombie)
        {
            if (!TelegraphBite())
            {
                // move toward humans
                TelegraphMove(targetIsZombie: false, moveTowardTarget: true);
            }
        }
        else
        {
            TelegraphMove(targetIsZombie: true, moveTowardTarget: false);
        }
    }

    public void DoTurn()
    {
        if(freezeUntilPlayerClose && (gameManager.player.pos - pos).magnitude >= freezeUntilPlayerWithin)
        {
            return;
        }

        var entity = gameManager.GetEntityAt(pos + telegraphController.GetDirection());

        if (stunStatus == StunStatus.WAIT_NEXT_TURN)
        {
            stunStatus = StunStatus.NORMAL;
            return;
        }

        if (stunStatus == StunStatus.WAIT_NEXT_TELEGRAPH)
            return;

        else if (telegraphController.GetTelegraphType() == EnemyTelegraphController.TelgraphType.MOVE)
        {
            if (entity == null)
            {
                DoMove(telegraphController.GetDirection());
            }
            else if (isZombie && !entity.isZombie)
            {
                Bite(entity);
            }
        }
        else
        {
            if (entity && isZombie && !entity.isZombie)
            {
                Bite(entity);
            }
        }

        telegraphController.ClearTelegraph();
    }

    private void DoMove(Vector2Int delta)
    {
        pos = pos + delta;
        transform.position = new Vector3(pos.x, 0, pos.y);
        lastMoveDir = delta;

        // Guard walk animations speed up a bit between steps
        if (!isZombie)
            curAnimator.GetComponent<AnimationSpeedController>().Play();
    }

    private bool TelegraphBite()
    {
        // attack enemy in random direction, returns true if I can attack anyone, false otherwise
        Vector2Int delta = lastMoveDir;

        var dirs = new List<Vector2Int>()
            {
                Vector2Int.up,
                Vector2Int.left,
                Vector2Int.down,
                Vector2Int.right
            };

        // pick random direction that we haven't tried yet
        for (int i = 0; i < 4; i++)
        {
            delta = dirs[Random.Range(0, dirs.Count)];
            var entity = gameManager.GetEntityAt(pos + delta);

            if (entity && !entity.isZombie)
            {
                telegraphController.UpdateTelegraph(EnemyTelegraphController.TelgraphType.ATTACK, delta, isZombie);
                emotion = EmotionType.HOSTILE;
                return true;
            }
            else
            {
                dirs.Remove(delta);
            }
        }
        emotion = EmotionType.IDLE;
        return false;
    }

    private bool TelegraphMove(bool targetIsZombie, bool moveTowardTarget)
    {
        var visible = gameManager.GetAllEntitiesVisibleBy(this);
        bool seesTarget = false;
        GridEntity nearestTarget = null;
        float distToNearestTarget = Mathf.Infinity;

        foreach (var entity in visible)
        {
            if (entity.isZombie == targetIsZombie) // I see a target
            {
                seesTarget = true;
                var distToTarget = Vector2Int.Distance(pos, entity.pos);
                if (distToTarget < distToNearestTarget)
                {
                    nearestTarget = entity;
                    distToNearestTarget = distToTarget;
                }
            }
        }

        if (seesTarget && TelegraphMoveInRelationToEntity(nearestTarget, moveTowardTarget))
        {
            emotion = moveTowardTarget ? EmotionType.HOSTILE : EmotionType.FLEE;
            return true;
        }

        emotion = EmotionType.IDLE;
        return TelegraphRandomMove();
    }

    private bool TelegraphMoveInRelationToEntity(GridEntity target, bool moveTowardTarget)
    {
        Debug.Assert(target);
        Debug.DrawLine(raycastCenter.position, target.raycastCenter.position, moveTowardTarget ? Color.green : Color.red, 0.5f);
        var dirs = new List<Vector2Int>()
            {
                Vector2Int.up,
                Vector2Int.left,
                Vector2Int.down,
                Vector2Int.right
            };


        if (moveTowardTarget)
        {
            dirs = dirs.OrderBy(x => Vector2Int.Distance(x + pos, target.pos)).ToList();
        }
        else
        {
            dirs = dirs.OrderByDescending(x => Vector2Int.Distance(x + pos, target.pos)).ToList();
        }

        foreach (var dir in dirs)
        {
            if (gameManager.IsWalkable(pos + dir))
            {
                telegraphController.UpdateTelegraph(EnemyTelegraphController.TelgraphType.MOVE, dir, isZombie);
                return true;
            }
        }
        return false;
    }

    private bool TelegraphRandomMove()
    {
        // more likely to continue moving in the same direction it was going
        if (gameManager.IsWalkable(pos + lastMoveDir) && Random.value >= randomMoveChance)
        {
            telegraphController.UpdateTelegraph(EnemyTelegraphController.TelgraphType.MOVE, lastMoveDir, isZombie);
        }
        else
        {
            var dirs = new List<Vector2Int>()
            {
                Vector2Int.up,
                Vector2Int.left,
                Vector2Int.down,
                Vector2Int.right
            };

            // pick random direction that we haven't tried yet
            for (int i = 0; i < 4; i++)
            {
                var delta = dirs[Random.Range(0, dirs.Count)];
                if (gameManager.IsWalkable(pos + delta))
                {
                    telegraphController.UpdateTelegraph(EnemyTelegraphController.TelgraphType.MOVE, delta, isZombie);
                    return true;
                }
                else
                {
                    dirs.Remove(delta);
                }
            }
        }

        return false;
    }

    public override void GetBitten(GridEntity by)
    {
        base.GetBitten(by);

        telegraphController.ClearTelegraph();
        emotion = EmotionType.STUNNED;
        stunStatus = StunStatus.WAIT_NEXT_TELEGRAPH;
    }
}
