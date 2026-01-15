using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arbre_own_afficeur_manager : MonoBehaviour
{
    public GameObject player; 
    private Transform child2D;
    private Transform child3D;

    // Start is called before the first frame update
    void Start()
    {
        child2D = transform.Find("Arbrepng");
        child3D = transform.Find("Arbre3D");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 arbre_pos = transform.position;
        arbre_pos.y = player.transform.position.y;
        Vector3 V3_distance = arbre_pos - player.transform.position;
        if( V3_distance.magnitude < 10f)
        {
            child2D.gameObject.SetActive(false);
            child3D.gameObject.SetActive(true);
        }
        else 
        {
            child2D.gameObject.SetActive(true);
            child3D.gameObject.SetActive(false);
        }
    }
}
