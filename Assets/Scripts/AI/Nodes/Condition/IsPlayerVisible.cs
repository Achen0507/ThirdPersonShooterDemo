using UnityEngine;

public class IsPlayerVisible : Node
{
    private Transform self;
    private Transform player;
    private LayerMask layerMask;

    public IsPlayerVisible(Blackboard blackboard) : base(blackboard)
    {
        self = blackboard.Get<Transform>("Transform");
        player = blackboard.Get<Transform>("Player");

        int playerLayer = LayerMask.NameToLayer("Player");
        int coverLayer = LayerMask.NameToLayer("Ground");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        layerMask = (1 << playerLayer) | (1 << coverLayer);
        layerMask &= ~(1 << enemyLayer);
    }


    public override State Evaluate()
    {
        if (player == null || self == null)
        {
            state = State.Failure;
            return state;
        }

        Vector3 startPos = self.position + Vector3.up * 1.0f;
        Vector3 targetPos = player.position + Vector3.up * 0.5f;
        Vector3 direction = (targetPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, targetPos);

        if (Physics.Raycast(startPos, direction, out RaycastHit hit, distance, layerMask))
        {
            Debug.Log($"ÉäÏß»÷ÖÐ: {hit.transform.name}, Tag: {hit.transform.tag}");

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                state = State.Success;
                return state;
            }
                state = State.Failure;
                return state;
        }

        state = State.Failure;
        return state;
    }
}
