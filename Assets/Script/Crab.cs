using UnityEngine;
using UnityEngine.AI;
public class Crab : MonoBehaviour
{

    // Crab stats
    [Header ("Crab Stats")]

    [SerializeField] 
    private float m_speed = 1f;

    [SerializeField]
    private int m_weight = 10;

    public NavMeshAgent agent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_speed = Random.Range(0.5f, 10f);
        m_weight = Random.Range(1, 40);

        agent.speed = m_speed;
        this.transform.localScale = Vector3.one * (1 + m_weight / 40f);
    }

    // Update is called once per frame
    void Update()
    {

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 10f, 1);
            Vector3 finalPosition = hit.position;
            agent.SetDestination(finalPosition);
            
        }


        // Orient the crab to face its moving direction
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public int get_weight()
    {
        return m_weight;
    }

}
