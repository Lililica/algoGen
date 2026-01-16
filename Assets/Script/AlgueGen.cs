using System.Numerics;
using System.Xml.Serialization;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class AlgueGen : MonoBehaviour
{
    

    [Header ("Algue Gen Stats")]

    [SerializeField]
    private GameObject alguePrefab1;
    [SerializeField]
    private GameObject alguePrefab2;
    
    private GameObject alguePrefab
    {
        get
        {
            if (Random.value > 0.5f)
            {
                return alguePrefab1;
            }
            else
            {
                return alguePrefab2;
            }
        }
    }

    [SerializeField]
    private int maxAlgueCount = 1000;
    [SerializeField]
    private int algueCount = 50;


    [SerializeField]
    private GameObject plane;

    [SerializeField]
    private float generationInterval = 40f;

    private Vector3[] alguePositions;

    private Vector3 min;
    private Vector3 max;

    private float timeSinceGeneration = 0f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        min = plane.GetComponent<Renderer>().bounds.min + Vector3.one * 0.5f;
        max = plane.GetComponent<Renderer>().bounds.max - Vector3.one * 0.5f;



        for(int i = 0; i < algueCount; i++)
        {
            GenerateAlgue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Generate algue every n seconds
        timeSinceGeneration -= Time.deltaTime;
        while (timeSinceGeneration <= 0f)
        {  
            if (gameObject.transform.childCount <= maxAlgueCount) {
                GenerateAlgue();
            }
            timeSinceGeneration += generationInterval;
        }
    }

    private void GenerateAlgue()
    {
        


        Vector3 randomPos = new Vector3(
            Random.Range(min.x, max.x),
            0,
            Random.Range(min.z, max.z)
        );
        NavMeshHit Hit;
        if (NavMesh.SamplePosition(randomPos, out Hit, Mathf.Infinity, -1))
        {
            GameObject enemy = Instantiate(alguePrefab, Hit.position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), transform);
        }
        else
        {
            Debug.LogError($"Unable to place NavMeshAgent on NavMesh");
        }

        // Vector3 randomPosition = new Vector3(
        //     Random.Range(min.x, max.x),
        //     plane.transform.position.y + 0.5f,
        //     Random.Range(min.z, max.z)
        // );

        // Instantiate(alguePrefab, randomPosition, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), transform);
    }
}
