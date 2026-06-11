using UnityEngine;
using UnityEngine.AI;

public class ReturnToStart : Node
{
    private NavMeshAgent agent;
    private Vector3 startPosition;
    private float arriveDistance = 1f;

    public ReturnToStart(Blackboard blackboard) : base(blackboard)
    {
        agent = blackboard.Get<NavMeshAgent>("NavMeshAgent");

        Transform self = blackboard.Get<Transform>("Transform");
        if (self != null)
            startPosition = self.position;
    }

    public override State Evaluate()
    {
        if (agent == null)
        {
            state = State.Failure;
            return state;
        }

        if (Vector3.Distance(agent.transform.position, startPosition) < arriveDistance)
        {
            agent.ResetPath();
            state = State.Success;
            return state;
        }

        agent.SetDestination(startPosition);
        state = State.Running;
        return state;
    }
}
