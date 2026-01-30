using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_capsule : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public float vitesse = 1f;
    public float vitesse_rotation = 2f;
    public Transform spawn_point;

    // Update is called once per frame

    void check_too_far()
    {
        Vector3 vect_distance = spawn_point.position - transform.position;
        if(vect_distance.magnitude > 80)
        {
            transform.position = spawn_point.position;
        }
    }

    void Update()
    {
        float rotation_reduction = 0f;
        // transform.rotation = Quaternion.Euler(transform.rotation.x*rotation_reduction, transform.rotation.y, transform.rotation.z*rotation_reduction);
        Vector3 rot = transform.eulerAngles;
        rot.x *= rotation_reduction;
        rot.z *= rotation_reduction;
        transform.eulerAngles = rot;
        Vector3 non = new Vector3(0,vitesse_rotation,0);
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            non.y = -Mathf.Abs(non.y);   
            transform.Rotate(non);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            non.y = Mathf.Abs(non.y);
            transform.Rotate(non);
        }
        //Vector3 deplacement = new Vector3(0,0,0);
        if (Input.GetKey(KeyCode.W))
        {
            //deplacement.z -= vitesse;   
            //transform.position += deplacement;
            transform.position += transform.forward*vitesse;
        }

        if (Input.GetKey(KeyCode.A))
        {
            //deplacement.x += vitesse;
            //transform.position += deplacement;
            transform.position -= transform.right*vitesse;
        }   

        if (Input.GetKey(KeyCode.S))
        {
            // deplacement.z += vitesse;   
            // transform.position += deplacement;
            transform.position -= transform.forward*vitesse;
        }

        if (Input.GetKey(KeyCode.D))
        {
            //deplacement.x -= vitesse;
            //transform.position += deplacement;
            transform.position += transform.right*vitesse;
        }

    check_too_far();
    }
}