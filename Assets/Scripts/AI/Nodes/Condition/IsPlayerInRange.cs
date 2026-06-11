using UnityEngine;

public class IsPlayerInRange : Node
{
    private float range;

    public IsPlayerInRange(Blackboard blackboard, float range) : base(blackboard)
    {
        this.range = range;
    }

    override public State Evaluate()
    {
        Transform player = blackboard.GetPlayer();
        Transform self = blackboard.Get<Transform>("Transform");

        if (player == null || self == null) {
            state = State.Failure;
            return state;
        }

        float distance = Vector3.Distance(self.position, player.position);
        state = distance <= range ? State.Success : State.Failure;
        return state;
    }
}
