public class Selector : Node
{
   public Selector(Blackboard blackboard) : base(blackboard) { }

    public override State Evaluate()
    {
        foreach (Node child in children)
        {
            switch (child.Evaluate())
            {
                case State.Running:
                    state = State.Running;
                    return state;
                case State.Success:
                    state = State.Success;
                    return state;
                case State.Failure:
                    continue;
            }
        }

        state = State.Failure;
        return state;
    }
}
