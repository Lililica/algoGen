using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class group_objects : MonoBehaviour
{
    public bool done;
    public GameObject L_arbre; 
    //bool once;

    // Start is called before the first frame update
    void Start()
    {
        done = false; 
        //once = false;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] instance = new CombineInstance[meshFilters.Length];
        int i=0;
        while(i < meshFilters.Length)
        {
            instance[i].mesh = meshFilters[i].sharedMesh;
            instance[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            //Destroy(meshFilters[i].gameObject);
            i++;
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.CombineMeshes(instance);
        transform.GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.gameObject.SetActive(true);
        L_arbre = transform.gameObject;

        int nb_game_object = meshFilters.Length;
        i=0;
        while(i < nb_game_object)
        {
            if(meshFilters[i].gameObject.name != "L_system_parent") Destroy(meshFilters[i].gameObject);
            i++;
        }
        
        done = true;
        transform.GetComponent<group_objects>().enabled = false;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if(!done)
    //     {
    //         MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
    //         CombineInstance[] instance = new CombineInstance[meshFilters.Length];
    //         int i=0;
    //         while(i < meshFilters.Length)
    //         {
    //             instance[i].mesh = meshFilters[i].sharedMesh;
    //             instance[i].transform = meshFilters[i].transform.localToWorldMatrix;
    //             meshFilters[i].gameObject.SetActive(false);
    //             //Destroy(meshFilters[i].gameObject);
    //             i++;
    //         }

    //         Mesh mesh = new Mesh();
    //         mesh.CombineMeshes(instance);
    //         transform.GetComponent<MeshFilter>().sharedMesh = mesh;
    //         transform.gameObject.SetActive(true);
    //         L_arbre = transform.gameObject;

    //         //nettoyer
    //         int nb_game_object = meshFilters.Length;
    //         i=0;
    //         while(i < nb_game_object)
    //         {

    //             Destroy(meshFilters[i].gameObject);
    //             i++;
    //         }

    //         done = true;
    //     }

    //     // if(done && !once)
    //     // {
    //     //     send done; 
    //     //     once = true;
    //     // }
        
    // }
}
