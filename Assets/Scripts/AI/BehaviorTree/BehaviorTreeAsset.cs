using UnityEngine;

[CreateAssetMenu(fileName = "NewBehaviorTree", menuName = "AI/Behavior Tree")]
public class BehaviorTreeAsset : ScriptableObject
{
    public Node rootNode;

    public void Initialize()
    {
        // 这里可以序列化/反序列化节点
    }
}
