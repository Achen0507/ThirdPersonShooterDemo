using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : Node
{
    private NavMeshAgent agent;
    private Transform player;
    private float chaseRange;

    public ChasePlayer(Blackboard blackboard, float chaseRange) : base(blackboard)
    {
        agent = blackboard.Get<NavMeshAgent>("NavMeshAgent");
        player = blackboard.Get<Transform>("Player");
        this.chaseRange = chaseRange;

        if (agent != null)
            agent.updateRotation = false;
    }

    override public State Evaluate()
    {
        if (player == null || agent == null)
        {
            state = State.Failure;
            return state;
        }

        float distance = Vector3.Distance(agent.transform.position, player.position);

        if (distance > chaseRange)
        {
            agent.SetDestination(player.position);
            Animator anim = blackboard.Get<Animator>("Animator");
            if (anim != null)
            {
                float speed = agent.velocity.magnitude;
                anim.SetFloat("Speed", speed);
            }
            state = State.Running;
            return state;
        }

        bool isVisible = CheckPlayerVisible();
        Debug.Log($"距离: {distance}, chaseRange: {chaseRange}, isVisible: {isVisible}");

        // 在范围内但被遮挡，且没贴脸 → 继续靠近
        if (!isVisible && distance > 2f)
        {
            Debug.Log("继续靠近");
            agent.SetDestination(player.position);
            Animator anim = blackboard.Get<Animator>("Animator");
            if (anim != null)
            {
                float speed = agent.velocity.magnitude;
                anim.SetFloat("Speed", speed);
            }
            state = State.Running;
            return state;
        }

        // 在范围内且可见，或者已经贴脸 → 停下准备射击
        agent.ResetPath();
        Animator animator = blackboard.Get<Animator>("Animator");
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
        state = State.Failure;
        return state;
    }

    private bool CheckPlayerVisible()
    {
        Transform self = blackboard.Get<Transform>("Transform");
        Transform player = blackboard.Get<Transform>("Player");

        Vector3 startPos = self.position + Vector3.up * 1f;
        Vector3 targetPos = player.position + Vector3.up * 0.5f;
        Vector3 direction = (targetPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, targetPos);

        int playerLayer = LayerMask.NameToLayer("Player");
        int groundLayer = LayerMask.NameToLayer("Ground");  
        int layerMask = (1 << playerLayer) | (1 << groundLayer);

        if (Physics.Raycast(startPos, direction, out RaycastHit hit, distance, layerMask))
        {
            return hit.transform.CompareTag("Player");
        }
        return false;
    }
}