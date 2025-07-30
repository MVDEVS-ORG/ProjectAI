using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using Assets.ProjectAI.Scripts.PathFinding;
using Assets.ProjectAI.Scripts.EnemyScripts;

public enum EnemyStateTypes {
    Idle,
    Patrol,
    Chase,
    Attack,
    Dead,
    Search
}
public class EnemyAI : MonoBehaviour, IHealthSystem
{
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float detectionRange = 6f;
    public Transform attackSpawnPos;
    public float attackOffset = 0;
    public float attackCooldown = 1.5f;

    public HealthModels healthModel;
    public Animator animator;

    [SerializeField] private Collider2D _enemyCollider;
    private ObjectPoolManager _objectPoolmanager;
    private Transform _player;
    private int _health = 10; 
    private int _maxHealth = 10;

    [Header("XP on Death")]
    public int xp;

    private Tilemap floorTilemap;
    private Coroutine moveRoutine;

    protected Dictionary<IEnemyState, EnemyStateTypes> stateMap = new();
    protected IEnemyState currentState;

    [HideInInspector] public List<Vector3Int> currentPath;
    [HideInInspector] public int currentPathIndex;

    public int Health => _health;
    public Transform Target => _player;
    public int MaxHealth => _maxHealth;
    public List<IAttackBehavior> attackBehaviors = new List<IAttackBehavior>();
    public virtual void InitializeEnemy(Transform playerTransform, ObjectPoolManager poolManager)
    {
        _player = playerTransform;
        _objectPoolmanager = poolManager;
    }

    public virtual void InitializeStates()
    {
        Debug.LogError("States not Initialized");
    }
    void Start()
    {
        _enemyCollider.enabled = true;
        Initialize(healthModel);
        StartCoroutine(WaitForBakeAndStart());
    }

    void OnEnable()
    {
        _enemyCollider.enabled = true;
        Initialize(healthModel);
        StartCoroutine(WaitForBakeAndStart());
    }

    IEnumerator WaitForBakeAndStart()
    {
        floorTilemap = PathFindingManager.Instance.floorTilemap;
        while (!PathFindingManager.Instance.IsMapBaked)
            yield return null;
        TransitionToState(GetNextStateFromMap(EnemyStateTypes.Idle));
    }

    void Update()
    {
        currentState?.Update();
    }

    public void TransitionToState(IEnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this, _player, _objectPoolmanager);
    }

    public void StartPathMovement(List<Vector3Int> path)
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        currentPath = path;
        currentPathIndex = 0;
        if (path != null && path.Count > 0)
            moveRoutine = StartCoroutine(FollowPath());
    }

    private IEnumerator<WaitForEndOfFrame> FollowPath()
    {
        while (currentPathIndex < currentPath.Count)
        {
            Vector3 targetPos = floorTilemap.GetCellCenterWorld(currentPath[currentPathIndex]);
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            currentPathIndex++;
        }
    }

    public bool IsPlayerVisible()
    {
        if (_player == null)
            return false;
        return Vector3.Distance(transform.position, _player.position) < detectionRange;
    }

    public bool IsPlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, _player.position) <= attackRange;
    }


    public void StopMovement()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
    }

    public void TakeDamage(int damage)
    {
        _health = Mathf.Clamp(_health - damage, 0, _maxHealth);
        if(_health <= 0)
        {
            OnEnemyDeath();
        }
    }

    public void Heal(int healing)
    {
        _health = Mathf.Clamp(_health + healing, 0, _maxHealth);
    }

    public virtual void Initialize(HealthModels model)
    {
        _health = model.Health;
        _maxHealth = model.MaxHealth;
        InitializeStates();
    }

    public void ResetHealth()
    {
        _health = _maxHealth;
    }

    private void OnEnemyDeath()
    {
        //Add death animation
        _enemyCollider.enabled = false;
        StopMovement();
        TransitionToState(GetNextStateFromMap(EnemyStateTypes.Dead));
    }


    private List<IEnemyState> GetStateFromMap(EnemyStateTypes stateType)
    {
        List<IEnemyState> enemyState = new();
        foreach (var state in stateMap)
        {
            if (stateType == state.Value)
            {
                enemyState.Add(state.Key);
            }
        }
        if(enemyState.Count == 0)
        {
            Debug.LogError("State Can not be found!");
            enemyState.Add(new IdleState());
        }

        return enemyState;
    }

    public IEnemyState GetNextStateFromMap(EnemyStateTypes stateType)
    {
        List<IEnemyState> enemyState = GetStateFromMap(stateType);
        int index = Random.Range(0, enemyState.Count);
        return enemyState[index];
    }

    public void ResetEnemyAI()
    {
        StopAllCoroutines();

        // Reset movement
        StopMovement();
        currentPath = null;
        currentPathIndex = 0;

        // Reset health
        _health = _maxHealth;

        // Reset animation state
        animator?.Rebind();
        animator?.Update(0f);

        // Reset collider
        if (_enemyCollider != null)
            _enemyCollider.enabled = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (currentPath == null || currentPath.Count == 0 || PathFindingManager.Instance == null)
            return;

        Tilemap floor = PathFindingManager.Instance.floorTilemap;

        //  Green Dot: Current Position
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);

        //  Red Dot: Final Destination
        Vector3 finalPos = floor.GetCellCenterWorld(currentPath[^1]);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(finalPos, 0.15f);

        //  Blue Line: Path to be followed
        Gizmos.color = Color.blue;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Vector3 from = floor.GetCellCenterWorld(currentPath[i]);
            Vector3 to = floor.GetCellCenterWorld(currentPath[i + 1]);
            Gizmos.DrawLine(from, to);
        }

        //  Optional: Detection Range (Scene Only)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        //  Optional: Attack Range
        Gizmos.color = new Color(1f, 0.3f, 0f); // Orange
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif
}
