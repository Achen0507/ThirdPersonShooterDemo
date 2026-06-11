public class Sequence : Node
{
    public Sequence(Blackboard blackboard) : base(blackboard) { }

    public override State Evaluate()
    {
        bool anyChildRunning = false;

        foreach (Node child in children)
        {
            switch (child.Evaluate())
            {
                case State.Failure:
                    state = State.Failure;
                    return state;
                case State.Running:
                    anyChildRunning = true;
                    continue;
                case State.Success:
                    continue;
            }
        }

        state = anyChildRunning ? State.Running : State.Success;
        return state;
    }
}
