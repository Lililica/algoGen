using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copie_tree : MonoBehaviour
{
    public GameObject to_copie;
    public List<GameObject> copie_position;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("copie done");
            // Transform to_copie = this.gameObject.transform.GetChild(0);
            // GameObject my_copie = Instantiate(to_copie.gameObject,copie.transform.position,Quaternion.identity);
            // list_trees.Add(my_copie);
        for(int i = 0; i<copie_position.Count ; i++)
        {
            if(i==0)
            {
                to_copie.transform.position = copie_position[i].transform.position;
            }
            else
            Instantiate(to_copie, copie_position[i].transform.position, Quaternion.identity);
        }
        //list_trees.Add(my_copie_2);
        //once = false;
        return;
    }

}
