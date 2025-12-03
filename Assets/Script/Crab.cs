using UnityEngine;
using UnityEngine.AI;
public class Crab : MonoBehaviour
{

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

    [Header ("Crab Stats")]
    [SerializeField]
    private Genes genes;

    [HideInInspector]
    public spawner eve;

    // Various simulation constant parameters
    const float TARGET_OFFSET = 0.3f;
    const float METABOLISM = 0.1f;
    const float SEX_REACH = 1.2f;
    const float INTERACT_DIST = 0.5f;
    const float FOOD_PER_BITE = 1.3f;
    const float MAX_CHILD_RATIO = .95f; // How much of their food level parents are allowed to give (prevent birth explosion)
    const bool METABOLISM_INCREASES_OVER_TIME = true; // Forces population turnover

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        agent.speed = genes.speed;
        agent.acceleration = genes.acceleration;
        agent.angularSpeed = genes.steering;
        this.transform.localScale = Vector3.one * (1 + genes.weight / 40f);
    }

    void Update()
    {
        // Decrease foodLevel while taking into account the metabolism increase if acctivated
        foodLevel -= Time.deltaTime * (METABOLISM + (METABOLISM_INCREASES_OVER_TIME ? (Time.time - creationTime)/271f : 0f));
        if (foodLevel < 0) {
            transform.localScale = new Vector3(1, -1, 1);
            agent.ResetPath();
            Destroy(gameObject, 5f);
            return;
        }

        GameObject hotCrabInYourVicinity = SeeHorny();
        GameObject yummyYummySeaGrass = SmelledFood();
        if (IsHorny() && hotCrabInYourVicinity != null)
        { // Wants to reproduce and has found a parter in his field of vision
            targetPos = hotCrabInYourVicinity.transform.position;
            if ((targetPos - transform.position).sqrMagnitude <= SEX_REACH*SEX_REACH)
                Mate(hotCrabInYourVicinity.GetComponent<Crab>());
        }
        else if (yummyYummySeaGrass != null)
        { // Has found food
            targetPos = yummyYummySeaGrass.transform.position;
            if ((targetPos - transform.position).sqrMagnitude <= INTERACT_DIST*INTERACT_DIST)
                NomNom(yummyYummySeaGrass.gameObject);
        }
        else if (!agent.hasPath || agent.remainingDistance < 0.5f)
        { // Wanders randomely
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 10f, 1);
            targetPos = hit.position;
        }

        if ((targetPos - agent.destination).sqrMagnitude >= TARGET_OFFSET)
        { // The current crabs target is too far from the path findings target (avoid recomputing a path every frame)
            agent.SetDestination(targetPos);
        }

        // Orient the crab to side its moving direction
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public bool IsHorny()
    { // Food level is high enough and the crab is old enough (prevent population explosion)
        return foodLevel >= genes.libidoThreshold && (Time.time - creationTime >= 1.5f);
    }

    public GameObject SeeHorny()
    { // Return a parter if detected, null if not
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
    { // Return a seaGrass if detected, null if not
        Collider[] seaWeeds = Physics.OverlapSphere(
            gameObject.transform.position,
            genes.smell,
            seaFoodLayer.value
        );

        if (seaWeeds == null || seaWeeds.Length == 0) return null;

        GameObject closest = seaWeeds[0].gameObject;
        foreach (Collider coll in seaWeeds) {
            if ((transform.position - coll.transform.position).sqrMagnitude < (transform.position - closest.transform.position).sqrMagnitude)
                closest = coll.gameObject;
        }
        return closest;
    }

    private void NomNom(GameObject seaGrass)
    { // Eat sea food
        Destroy(seaGrass);
        if (foodLevel <= genes.maxFoodLevel) foodLevel += FOOD_PER_BITE;
    }

    public void Mate(Crab partner)
    {
        int babyNumber = Random.Range(this.genes.minChild, this.genes.maxChild + 1);
        for (int i = 0; i < babyNumber; i++)
        {
            if (eve.transform.childCount <= eve.maxNumberOfCrabs)
                MakeBaby(partner);
        }

        foodLevel *= 1 - genes.childRatio;
    }

    public void MakeBaby(Crab partner)
    {
        Genes childGenes = new Genes();

        childGenes.speed = GeneMix(genes.speed, partner.genes.speed);
        childGenes.acceleration = GeneMix(genes.acceleration, partner.genes.acceleration);
        childGenes.steering = GeneMix(genes.steering, partner.genes.steering);
        childGenes.smell = Mathf.Max(GeneMix(genes.smell, partner.genes.smell), 0f);
        childGenes.libidoThreshold = GeneMix(genes.libidoThreshold, partner.genes.libidoThreshold);
        childGenes.vision = Mathf.Max(GeneMix(genes.vision, partner.genes.vision), 0f);
        childGenes.childRatio = Mathf.Clamp(ChildRatioMix(genes.childRatio, partner.genes.childRatio), 0f, MAX_CHILD_RATIO);
        childGenes.maxFoodLevel = GeneMix(genes.maxFoodLevel, partner.genes.maxFoodLevel);

        childGenes.minChild = Mathf.Min(genes.minChild, partner.genes.minChild);
        childGenes.maxChild = Mathf.Max(genes.maxChild, partner.genes.maxChild);
        
        Crab child =
            Instantiate(this, transform.position + new Vector3(1, 0, 1), Quaternion.identity, eve.transform);
        child.genes = childGenes;
        child.foodLevel = foodLevel * genes.childRatio;
        child.creationTime = Time.time;
        child.eve = eve;
    }

    private float GeneMix(float valueA, float valueB)
    { // Mix genes with a mutation variation
        const float MUTATION_OFFSET = 1f;

        return Random.Range(
            Mathf.Min(valueA, valueB) - MUTATION_OFFSET,
            Mathf.Max(valueA, valueB) + MUTATION_OFFSET
        );
    }

    private float ChildRatioMix(float valueA, float valueB)
    { // Mix the child ratio (the value is not on a very different scale as other genes, hence the special mix
        const float MUTATION_OFFSET = .01f;

        return Random.Range(
            Mathf.Min(valueA, valueB) - MUTATION_OFFSET,
            Mathf.Max(valueA, valueB) + MUTATION_OFFSET
        );
    }

    public void ShuffleGenes()
    { // Random starting genes
        genes.speed = Random.Range(4f, 20f);
        genes.acceleration = Random.Range(5f, 10f);
        genes.steering = Random.Range(90f, 120f);
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

    void OnDrawGizmosSelected () {
		Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, genes.smell);

		Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, genes.vision);
	}
}

[System.Serializable]
struct Genes {
    public float speed;
    public float acceleration;
    public float steering;
    public float weight;
    public float smell;
    public int minChild;
    public int maxChild;
    public float libidoThreshold;
    public float vision;
    public float childRatio;
    public float maxFoodLevel;
}
