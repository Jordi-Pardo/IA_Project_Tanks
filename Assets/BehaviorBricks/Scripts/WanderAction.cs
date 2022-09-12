using UnityEngine;
using UnityEngine.AI;

using Pada1.BBCore;           // Code attributes
using Pada1.BBCore.Tasks;     // TaskStatus
using Pada1.BBCore.Framework; // BasePrimitiveAction

[Condition("Tanks/Wander")]
public class WanderAction : BasePrimitiveAction
{
    [InParam("This Transform")]
    public Transform transform;

    [InParam("Layer mask")]
    public LayerMask wanderLayerMask;

    [OutParam("World Target")]
    public Vector3 worldTarget;

    [InParam("Agent")]
    public NavMeshAgent agent;

    [InParam("ShootScript")]
    public Shoot shootScript;

    public bool m_CanMove = true;
    private float freq = 0f;
    private float stopDistance = 5f;
    public override TaskStatus OnUpdate()
    {
        CheckIfHasTarget();
        if (m_CanMove)
        {
            MovewithFrames();
        }
        return TaskStatus.RUNNING;
    }

    void Wander()
    {
        float radius = 4f;
        float offset = 10f;

        Vector3 localTarget = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));

        localTarget.Normalize();
        localTarget *= radius;
        localTarget += new Vector3(0, 0, offset);

        worldTarget = transform.TransformPoint(localTarget);
        worldTarget.y = 1f;
        Debug.Log("Checkinng..");
        if (!Physics.CheckSphere(worldTarget, 0.2f, wanderLayerMask))
        {
            worldTarget.y = 0f;
            Debug.Log("Check");

            agent.SetDestination(worldTarget);
        }
        else
        {
            worldTarget.y = 0f;
            worldTarget *= -1f;
            Debug.Log("Resversed");
            agent.SetDestination(worldTarget);

        }

        Debug.DrawLine(transform.position, worldTarget, Color.red, 1f);
    }

    private void CheckIfHasTarget()
    {
        if (shootScript.target == null)
        {
            m_CanMove = true;
            agent.isStopped = false;
        }
        else
        {
            m_CanMove = false;
            agent.isStopped = true;
        }
    }

    void MovewithFrames()
    {
        if (Vector3.Distance(agent.destination, transform.position) < stopDistance)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }


        freq += Time.deltaTime;
        if (freq > 1f)
        {
            freq -= 1f;
            Wander();
        }
    }
}
