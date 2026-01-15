using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_cam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 oui = new Vector3(1,0,0);
        if (Input.GetKey(KeyCode.UpArrow))
        {
            oui.x = 1;   
            transform.Rotate(oui);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            oui.x = -1;   
            transform.Rotate(oui);
        }   
    }
}
