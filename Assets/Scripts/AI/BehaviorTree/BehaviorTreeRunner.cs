using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BehaviorTreeRunner : MonoBehaviour
{
    [SerializeField] private BehaviorTreeAsset behaviorTree;

    private Blackboard blackboard;
    private Node rootNode;
    private NavMeshAgent agent;
    private Transform player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            agent = gameObject.AddComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
                playerObj = playerHealth.gameObject;
        }

        player = playerObj.transform;

        blackboard = GetComponent<Blackboard>();
        if (blackboard == null) blackboard = gameObject.AddComponent<Blackboard>();

        Animator animator = GetComponent<Animator>();
        blackboard.Set("Animator", animator);

        blackboard.Set("Transform", transform);
        blackboard.Set("Player", player);
        blackboard.Set("NavMeshAgent", agent);
        blackboard.Set("Health", GetComponent<PlayerHealth>());

        BuildTree();
    }

    private void Update()
    {
        if (rootNode != null)
        {
            Debug.Log("行为树执行中");
            rootNode.Evaluate();
        }
    }

    private void BuildTree()
    {
        // 示例：一个简单的追逐 + 攻击行为树
        // 这里先写死，后面用 ScriptableObject 配置

        float chaseRange = 8f;
        float tooFarRange = 12f;

        var isPlayerInRange = new IsPlayerInRange(blackboard, chaseRange);
        var isPlayerTooFar = new IsPlayerTooFar(blackboard, tooFarRange);
        var isPlayerVisible = new IsPlayerVisible(blackboard);
        var chasePlayer = new ChasePlayer(blackboard, chaseRange);
        var shootAtPlayer = new ShootAtPlayer(blackboard, chaseRange);
        var returnToStart = new ReturnToStart(blackboard);
        var cooldown = new Cooldown(blackboard, shootAtPlayer, 1f);

        // 战斗：范围内 + 可见 → 射击
        var combatSequence = new Sequence(blackboard);
        combatSequence.AddChild(isPlayerInRange);
        combatSequence.AddChild(isPlayerVisible);
        combatSequence.AddChild(cooldown);

        // 追逐：范围内但不可见 → 继续靠近
        var returnSequence = new Sequence(blackboard);
        returnSequence.AddChild(isPlayerTooFar);
        returnSequence.AddChild(returnToStart);

        var selector = new Selector(blackboard);
        selector.AddChild(combatSequence);   // 优先：可见就射击
        selector.AddChild(chasePlayer);      // 其次：不可见就靠近（或追逐）
        selector.AddChild(returnSequence);   // 最后：太远就返回

        rootNode = selector;
    }
}
