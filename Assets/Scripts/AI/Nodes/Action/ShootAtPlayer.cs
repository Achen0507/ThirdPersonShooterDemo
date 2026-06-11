using UnityEngine;

public class ShootAtPlayer : Node
{
    private Transform player;
    private Transform self;
    private float chaseRange;
    private float lastShootTime = 0f;

    public ShootAtPlayer(Blackboard blackboard, float chaseRange) : base(blackboard)
    {
        self = blackboard.Get<Transform>("Transform");
        player = blackboard.Get<Transform>("Player");
        this.chaseRange = chaseRange;
    }

    public override State Evaluate()
    {
        if (player == null || self == null)
        {
            state = State.Failure;
            return state;
        }

        float distance = Vector3.Distance(self.position, player.position);

        if (distance > chaseRange)
        {
            state = State.Failure;
            return state;
        }

        Vector3 direction = (player.position - self.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            self.rotation = Quaternion.Slerp(self.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
        }

        Animator animator = blackboard.Get<Animator>("Animator");
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Shoot");
        }

        if (Time.time > lastShootTime + 1f)
        {
            lastShootTime = Time.time;
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth?.TakeDamage(8f);
        }

        state = State.Running;
        return state;
    }
}