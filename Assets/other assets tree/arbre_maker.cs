using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arbre_maker : MonoBehaviour
{
    public int nb_arbres = 100; 
    private int nb_arbres_flag;
    public GameObject my_tree_sprite;
    public GameObject player;

    private bool cld_done;
    private bool once;
    private group_objects cld;
    private List<GameObject> list_arbres;
    private List<GameObject> list_arbres_L_system;
    public GameObject L_arbre;
    //public GameObject camera;
    // Start is called before the first frame update

    bool check_lsystem_done()
    {
        cld = Object.FindAnyObjectByType<group_objects>();

        if (cld == null)
        {
            // Debug.LogError("get_tab_tiles "+"Impossible de trouver WFCBuilder dans la scène ");
            return false;
        }

        //L_arbre = cld.L_arbre;

        return cld.done;
    }

    void forest_manager() // ajoute ou retire les arbres
    {
        if (nb_arbres > nb_arbres_flag)//add trees
        {
            for(int i = 0 ; i < nb_arbres - nb_arbres_flag ; i++)
            {
                Vector3 position = new Vector3(Random.Range(-50,50),my_tree_sprite.transform.position.y,Random.Range(-50,50));
                Vector3 change_rotation = new Vector3(15,45,125); 
                Quaternion rotation = Quaternion.Euler( change_rotation);
                //camera.transform.eulerAngles 
                
                GameObject arbre = Instantiate(my_tree_sprite, position, rotation);
                //Arbre_own_afficheur aoa = arbre.AddComponent<Arbre_own_afficheur>();
                list_arbres.Add(arbre);
 
            }

        }
        if (nb_arbres < nb_arbres_flag)//remove trees
        {
            for(int i = 0 ; i < nb_arbres_flag - nb_arbres ; i++)
            {
                //TODO
            }
        }

        nb_arbres_flag = nb_arbres;
    }

    void forest_afficheur_initialiser(int nb_L_arbres) // créer les arbres 3D 
    {
        for(int i = 0 ; i < nb_L_arbres ; i++)
        {
            GameObject L_arbre_copie = Instantiate(L_arbre, L_arbre.transform.position, L_arbre.transform.rotation);
            list_arbres_L_system.Add(L_arbre_copie);
            //Debug.Log("instantiated L_arbre");
        }
    }

    float dm(Vector3 position_gameobject) //distance_magnitude ,  renvoie la distance au joueur du gameobject
    {
        Vector3 distance = position_gameobject - player.transform.position;
        return distance.magnitude;
    }

    List<int> find_nearest(int nb_nearest) // trouve les 5 arbres les plus proches du joueur 
    {
        List<int> i_nearest = new List<int>();
        List<Vector3> nearest = new List<Vector3>();
        //2List<float> nearest_magnitudes = new List<float>();
        //TODO
        for(int i = 0 ; i < nb_nearest ; i++)
        {
            //nearest.Add(list_arbres[i].transform.position);
            i_nearest.Add(i);

        }

        //trie les 5 premiers 
        bool bordel = true;
        // while (bordel)
        // {
        //     bordel = false;
        //     for(int i = 0 ; i < nearest.Count ; i++)
        //     {
        //         int ip = i;
        //         for(int j = i ; j < nearest.Count ; j++)
        //         {
        //             if(dm(nearest[j]) < dm(nearest[ip]))
        //             {
        //                 ip = j;
        //                 bordel = true;
        //             } 
        //         }
        //         Vector3 p = nearest[i];
        //         nearest[i] = nearest[ip];
        //         nearest[ip] = p;
        //     }
        // }
        while (bordel)
        {
            bordel = false;
            for(int i = 0 ; i < nearest.Count ; i++)
            {
                int ip = i;
                for(int j = i ; j < nearest.Count ; j++)
                {
                    if(dm(list_arbres[i_nearest[j]].transform.position) < dm(list_arbres[i_nearest[ip]].transform.position))
                    {
                        ip = j;
                        bordel = true;
                    } 
                }
                int p = i_nearest[i];
                i_nearest[i] = i_nearest[ip];
                i_nearest[ip] = p;
            }
        }

        //on va chercher les 5 plus petits 
        // for(int i = nearest.Count - 1 ; i < list_arbres.Count ; i++)
        // {
        //     if(dm(list_arbres[i].transform.position) < dm(nearest[nearest.Count-1]))
        //     {
        //         int k = nearest.Count;
        //         bool little = true;
        //         while(k > 0 && little)
        //         {
        //             if(dm(list_arbres[i].transform.position) < dm(nearest[k-1])) k--;
        //             else little = false;
        //         }
        //         //while( (dm(list_arbres[i].transform.position) < dm(nearest[k-1])) && k > 0) k--;
        //         Vector3 p1 = list_arbres[i].transform.position;
        //         while(k < nearest.Count)
        //         {
        //             Vector3 p2 = nearest[k];
        //             nearest[k] = p1;
        //             p1 = p2;
        //             k++; 
        //         }
        //     }
        // }

        for(int i = nb_nearest - 1 ; i < list_arbres.Count ; i++)
        {
            if(dm(list_arbres[i].transform.position) < dm(list_arbres[i_nearest[nb_nearest-1]].transform.position))
            {
                int k = nb_nearest;
                bool little = true;
                while(k > 0 && little)
                {
                    if(dm(list_arbres[i].transform.position) < dm(list_arbres[i_nearest[k-1]].transform.position)) k--;
                    else little = false;
                }
                //while( (dm(list_arbres[i].transform.position) < dm(nearest[k-1])) && k > 0) k--;
                int p1 = i;
                while(k < nb_nearest)
                {
                    int p2 = i_nearest[k];
                    i_nearest[k] = p1;
                    p1 = p2;
                    k++; 
                }
            }
        }

        //Debug.Log("i_nearest");
        return i_nearest;
    }

    void forest_afficheur() // change l'affichage de 2D à 3D ou inversement 
    {
        List<int> i_nearest = find_nearest(list_arbres_L_system.Count);

        // for(int i=0 ; i < i_nearest.Count ; i++)
        // {
        //     Debug.Log(i_nearest[i]);
        // }

        for (int i = 0 ; i < list_arbres.Count ; i++)
        {   
            list_arbres[i].SetActive(true);
        }
        for (int i = 0 ; i < i_nearest.Count ; i++)
        {   
            list_arbres_L_system[i].transform.position = list_arbres[i_nearest[i]].transform.position;
            list_arbres_L_system[i].transform.position = new Vector3(list_arbres_L_system[i].transform.position.x, 0, list_arbres_L_system[i].transform.position.z);
            list_arbres[i_nearest[i]].SetActive(false);
        }
    }

    void Start()
    {
        nb_arbres_flag = 0;
        cld_done = false;
        once = false;
        list_arbres = new List<GameObject>();
        list_arbres_L_system = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!cld_done) cld_done = check_lsystem_done();
        else 
        {
            if (!once)
            {
                forest_afficheur_initialiser(10); // on créer 5 copies de l'arbre L_system
                once = true;
            }
            forest_manager(); // on créer tout les arbres png 
            forest_afficheur(); // on gère l'affiche des arbres png et des arbres L_system
        }
    }
}
