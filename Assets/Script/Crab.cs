using UnityEngine;
using UnityEngine.AI;
public class Crab : MonoBehaviour
{

    // Crab stats
    [Header ("Crab Stats")]

    [SerializeField] 
    private float m_speed = 1f;

    [SerializeField]
    private float m_weight = 10;

    [SerializeField]
    private LayerMask seaFoodLayer;
    [SerializeField]
    private LayerMask crabLayer;

    [SerializeField]
    private float creationTime = 0f;

    private Vector3 targetPos;
    [SerializeField]
    private float foodLevel;
    private float age;

    private NavMeshAgent agent;

    [SerializeField]
    private Genes genes;

    const float TARGET_OFFSET = 0.3f;
    const float METABOLISM = 0.1f;
    const float INTERACT_DIST = 0.5f;
    const float FOOD_PER_BITE = 1f;
    const float MAX_CHILD_RATIO = .95f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        m_speed = genes.speed;
        m_weight = genes.weight;

        agent.speed = m_speed;
        this.transform.localScale = Vector3.one * (1 + m_weight / 40f);
    }

    void Update()
    {
        foodLevel -= METABOLISM * Time.deltaTime;
        if (foodLevel < 0) {
            transform.localScale = new Vector3(1, -1, 1);
            Destroy(gameObject, 5f);
            return;
        }

        GameObject hotCrabInYourVicinity = SeeHorny();
        GameObject yummyYummySeaGrass = SmelledFood();
        if (IsHorny() && hotCrabInYourVicinity != null) {
            targetPos = hotCrabInYourVicinity.transform.position;
            if ((targetPos - transform.position).sqrMagnitude <= INTERACT_DIST*INTERACT_DIST)
                Debug.Log("Seks");
                Mate(hotCrabInYourVicinity.GetComponent<Crab>());
        }
        else if (yummyYummySeaGrass != null) {
            targetPos = yummyYummySeaGrass.transform.position;
            if ((targetPos - transform.position).sqrMagnitude <= INTERACT_DIST*INTERACT_DIST)
                NomNom(yummyYummySeaGrass.gameObject);
        }
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

    public float GetWeight()
    {
        return m_weight;
    }

    public bool IsHorny()
    {
        return foodLevel >= genes.libidoThreshold;
    }

    public GameObject SeeHorny()
    {
        Collider[] crabs = Physics.OverlapSphere(
            gameObject.transform.position,
            genes.smell,
            crabLayer.value
        );

        if (crabs == null || crabs.Length == 0) return null;

        GameObject closest = crabs[0].gameObject;
        Crab crab = closest.gameObject.GetComponent<Crab>();

        if (crab == this) {
            if (crabs.Length == 1) return null;
            else closest = crabs[1].gameObject;
        }

        foreach (Collider coll in crabs) {
            crab = coll.gameObject.GetComponent<Crab>();
            if (
                crab != null &&
                crab != this &&
                crab.IsHorny() &&
                (transform.position - coll.transform.position).sqrMagnitude < (transform.position - closest.transform.position).sqrMagnitude
            )
                closest = coll.gameObject;
            return closest;
        }

        return null;
    }

    public GameObject SmelledFood()
    {
        Collider[] seeWeeds = Physics.OverlapSphere(
            gameObject.transform.position,
            genes.smell,
            seaFoodLayer.value
        );

        if (seeWeeds == null || seeWeeds.Length == 0) return null;

        GameObject closest = seeWeeds[0].gameObject;
        foreach (Collider coll in seeWeeds) {
            if ((transform.position - coll.transform.position).sqrMagnitude < (transform.position - closest.transform.position).sqrMagnitude)
                closest = coll.gameObject;
        }
        return closest;
    }

    private void NomNom(GameObject seeGrass)
    {
        Destroy(seeGrass);
        if (foodLevel <= genes.maxFoodLevel) foodLevel += FOOD_PER_BITE;
    }

    public void Mate(Crab partner)
    {
        int babyNumber = Random.Range(this.genes.minChild, this.genes.maxChild + 1);
        for (int i = 0; i < babyNumber; i++)
        {
            MakeBaby(partner);
        }

        foodLevel *= genes.childRatio;
    }

    public void MakeBaby(Crab partner)
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

        childGenes.minChild = Mathf.Min(this.genes.minChild, partner.genes.minChild);
        childGenes.maxChild = Mathf.Max(this.genes.maxChild, partner.genes.maxChild);

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
            Mathf.Min(Mathf.Max(this.genes.childRatio, partner.genes.childRatio) + MUTATION_OFFSET, MAX_CHILD_RATIO)
        );

        // Give a random max food level between the two parents
        childGenes.maxFoodLevel = Random.Range(
            Mathf.Min(this.genes.maxFoodLevel, partner.genes.maxFoodLevel) - MUTATION_OFFSET,
            Mathf.Max(this.genes.maxFoodLevel, partner.genes.maxFoodLevel) + MUTATION_OFFSET
        );
        
        Crab child =
            Instantiate(this, this.transform.position + new Vector3(1, 0, 1), Quaternion.identity);
        child.genes = childGenes;
        child.foodLevel = foodLevel * genes.childRatio;
        child.creationTime = Time.time;
    }

    public void ShuffleGenes() {
        genes.speed = Random.Range(4f, 20f);
        genes.weight = Random.Range(4f, 40f);
        genes.smell = Random.Range(4f, 20f);
        genes.minChild = Random.Range(2, 4);
        genes.maxChild = Random.Range(4, 6);
        genes.libidoThreshold = Random.Range(4f, 20f);
        genes.vision = Random.Range(4f, 20f);
        genes.childRatio = Random.Range(0.2f, 0.8f);
        genes.maxFoodLevel = Random.Range(4f, 20f);
    }

    public void FullBelly() {
        foodLevel = genes.maxFoodLevel;
    }
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
