using UnityEngine;

public class IsPlayerTooFar : Node
{
    private float tooFarDistance;

    public IsPlayerTooFar(Blackboard blackboard, float tooFarDistance) : base(blackboard)
    {
        this.tooFarDistance = tooFarDistance;
    }

    public override State Evaluate()
    {
        Transform player = blackboard.GetPlayer();
        Transform self = blackboard.Get<Transform>("Transform");

        if (player == null || self == null)
        {
            state = State.Failure;
            return state;
        }

        float distance = Vector3.Distance(self.position, player.position);
        state = distance > tooFarDistance ? State.Success : State.Failure;
        return state;
    }
}
