using UnityEngine;

public class Cooldown : Node
{
    private Node child;
    private float cooldownTime;
    private float lastExecutionTime = -Mathf.Infinity;

    public Cooldown(Blackboard blackboard, Node child, float cooldownTime) : base(blackboard)
    {
        this.child = child;
        this.cooldownTime = cooldownTime;
        AddChild(child);
    }

    public override State Evaluate()
    {
        if (Time.time >= lastExecutionTime + cooldownTime)
        {
            State childState = child.Evaluate();

            if (childState == State.Success)
            {
                lastExecutionTime = Time.time;
            }

            state = childState;
            return state;
        }

        state = State.Failure;
        return state;
    }

}
