using System.Collections.Generic;

public abstract class Node 
{
    protected State state;
    public Node parent;
    protected List<Node> children = new List<Node>();
    protected Blackboard blackboard;

    public Node(Blackboard blackboard) {
        this.blackboard = blackboard;
    }

    public void AddChild(Node child) {
        child.parent = this;
        children.Add(child);
    }

    public abstract State Evaluate();
}
