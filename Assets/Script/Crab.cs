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

    private Vector3 targetPos;
    private float foodLevel;
    private float age;

    private Genes genes;

    const float TARGET_OFFSET = 0.3f;

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

        GameObject hotCrabInYourVicinity = seeHorny();
        GameObject yummyYummySeaGrass = smelledFood();
        if (isHorny() && hotCrabInYourVicinity != null)
            targetPos = hotCrabInYourVicinity.transform.position;
        else if (yummyYummySeaGrass != null)
            targetPos = yummyYummySeaGrass.transform.position;
        else if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 10f, 1);
            targetPos = hit.position;
        }

        if ((targetPos - agent.destination).sqrMagnitude >= TARGET_OFFSET)
        {
            agent.SetDestination(targetPos);
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

    public bool isHorny()
    {
        return foodLevel >= genes.libidoThreshold;
    }

    public GameObject seeHorny()
    {
        // TODO return closest horny crab (not self)
        return null;
    }

    public GameObject smelledFood()
    {
        return null;
    }


    public void mate(Crab partner)
    {
        for(int i = 0; i < Random.Range(this.genes.minChild, this.genes.maxChild + 1); i++)
        {
            make_baby(partner);
        }
    
    }

    public void make_baby(Crab partner)
    {
        Genes childGenes = new Genes();

        const float MUTATION_OFFSET = 1f;

        // Give a random speed between the two parents
        childGenes.speed = Random.Range(
            Mathf.Min(this.genes.speed, partner.genes.speed) - MUTATION_OFFSET,
            Mathf.Max(this.genes.speed, partner.genes.speed) + MUTATION_OFFSET
        );
        // Give a random weight between the two parents
        childGenes.weight = Random.Range(
            Mathf.Min(this.genes.weight, partner.genes.weight) - MUTATION_OFFSET,
            Mathf.Max(this.genes.weight, partner.genes.weight) + MUTATION_OFFSET
        );

        // Give a random smell between the two parents
        childGenes.smell = Random.Range(
            Mathf.Min(this.genes.smell, partner.genes.smell) - MUTATION_OFFSET,
            Mathf.Max(this.genes.smell, partner.genes.smell) + MUTATION_OFFSET
        );

        minChild = Mathf.Min(this.genes.minChild, partner.genes.minChild);
        maxChild = Mathf.Max(this.genes.maxChild, partner.genes.maxChild);

        // Give a random libido threshold between the two parents

        childGenes.libidoThreshold = Random.Range(
            Mathf.Min(this.genes.libidoThreshold, partner.genes.libidoThreshold) - MUTATION_OFFSET,
            Mathf.Max(this.genes.libidoThreshold, partner.genes.libidoThreshold) + MUTATION_OFFSET
        );
        
        // Give a random vision between the two parents
        childGenes.vision = Random.Range(
            Mathf.Min(this.genes.vision, partner.genes.vision) - MUTATION_OFFSET,
            Mathf.Max(this.genes.vision, partner.genes.vision) + MUTATION_OFFSET
        );

        // Give a random child ratio between the two parents
        childGenes.childRatio = Random.Range(
            Mathf.Min(this.genes.childRatio, partner.genes.childRatio) - MUTATION_OFFSET,
            Mathf.Max(this.genes.childRatio, partner.genes.childRatio) + MUTATION_OFFSET
        );

        // Give a random max food level between the two parents
        childGenes.maxFoodLevel = Random.Range(
            Mathf.Min(this.genes.maxFoodLevel, partner.genes.maxFoodLevel) - MUTATION_OFFSET,
            Mathf.Max(this.genes.maxFoodLevel, partner.genes.maxFoodLevel) + MUTATION_OFFSET
        );
        
        Crab child =
            Instantiate(this, this.transform.position + new Vector3(1, 0, 1), Quaternion.identity);
        child.genes = childGenes;
        child.foodLevel = 0.5f * child.genes.maxFoodLevel;    }
}

enum States {
    Wandering,
    Eating,
    Mating,
}

struct Genes {
    public float speed;
    public float weight;
    public float smell;
    public int minChild;
    public int maxChild;
    public float libidoThreshold;
    public float vision;
    public float childRatio;
    public float maxFoodLevel;
}
