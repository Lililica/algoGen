using System.Linq;
using UnityEngine;

public class spawner : MonoBehaviour
{

    [SerializeField]
    private GameObject crabPrefab;

    [SerializeField]
    private int numberOfCrabs = 10;

    [SerializeField]
    public int maxNumberOfCrabs = 1000;

    [SerializeField]
    private float collisionDistance = 1f;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < numberOfCrabs; i++)
        {
            GameObject obj = Instantiate(crabPrefab, new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)), Quaternion.identity, transform);
            Crab crab = obj.GetComponent<Crab>();
            crab.ShuffleGenes();
            crab.FullBelly();
            crab.eve = this;
        }
        
    }

    // Update is called once per frame
    void DontUpdate()
    {
        /*
        for (int i = 0; i < numberOfCrabs; i++)
        {
            for (int j = i + 1; j < numberOfCrabs; j++)
            {
                if (crabs[i] == null || crabs[j] == null)
                {
                    continue;
                }
                float distance = Vector3.Distance(crabs[i].transform.position, crabs[j].transform.position);
                if (distance < collisionDistance)
                {
                    Crab crab1 = crabs[i].GetComponent<Crab>();
                    Crab crab2 = crabs[j].GetComponent<Crab>();

                    if (crab1.GetWeight() > crab2.GetWeight())
                    {
                        Destroy(crabs[j]);
                    }
                    else
                    {
                        Destroy(crabs[i]);
                    }
                }
            }
        }
        */
    }
}
