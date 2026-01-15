using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_capsule : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 non = new Vector3(0,1,0);
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            non.y = 1;   
            transform.Rotate(non);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            non.y = -1;
            transform.Rotate(non);
        }   
        float vitesse = 0.1f;
        Vector3 deplacement = new Vector3(0,0,0);
        if (Input.GetKey(KeyCode.T))
        {
            deplacement.z -= vitesse;   
            transform.position += deplacement;
        }

        if (Input.GetKey(KeyCode.F))
        {
            deplacement.x += vitesse;
            transform.position += deplacement;
        }   

        if (Input.GetKey(KeyCode.G))
        {
            deplacement.z += vitesse;   
            transform.position += deplacement;
        }

        if (Input.GetKey(KeyCode.H))
        {
            deplacement.x -= vitesse;
            transform.position += deplacement;
        }   
    }
}
