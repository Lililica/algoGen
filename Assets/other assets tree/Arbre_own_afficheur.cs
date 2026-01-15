using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arbre_own_afficheur : MonoBehaviour
{
    public GameObject camera_player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(camera_player.transform.eulerAngles);
    }
}
