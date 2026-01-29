using UnityEngine;

public class ArbreMove : MonoBehaviour
{

    [SerializeField]
    GameObject arbre;

    private bool hasMoved = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasMoved)
        {
            GameObject arbre1 = Instantiate(arbre);
            arbre1.SetActive(true);
            arbre1.transform.position = transform.position;
            hasMoved = true;
        }

    }
}
