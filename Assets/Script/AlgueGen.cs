using System.Numerics;
using System.Xml.Serialization;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class AlgueGen : MonoBehaviour
{
    

    [Header ("Algue Gen Stats")]

    [SerializeField]
    private GameObject alguePrefab;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        min = plane.GetComponent<Renderer>().bounds.min;
        max = plane.GetComponent<Renderer>().bounds.max;

        for(int i = 0; i < algueCount; i++)
        {
            GenerateAlgue();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Generate algue every n seconds
        if (Time.frameCount % (generationInterval * 50) == 0)
        {  
            if (gameObject.transform.childCount <= maxAlgueCount) {
                GenerateAlgue();
            }
        }
    }

    private void GenerateAlgue()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(min.x, max.x),
            0f,
            Random.Range(min.z, max.z)
        );

        Instantiate(alguePrefab, randomPosition, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), gameObject.transform);
    }
}
