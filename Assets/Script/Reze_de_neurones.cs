using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reze_de_neurones
{
    public float max_poids_generation = 1f;
    public int nb_couches_cachees = 4;
    public int nb_neurone_par_couche_cachee = 4;
    public int nb_sorties = 4;
    public List<List<List<float>>> liste_poids_cc;
    public List<List<float>> liste_poids_entrees;
    public List<List<float>> liste_poids_sorties;
    public List<float> entrees;
    private List<List<int>> bar_graphs;

    //ON PARCOURS TJR COUCHE SUIVANTE PUIS COUCHE PRECEDENTE ; C EST LA REGLE ICI SI VOUS VOULEZ SURVIVRE 
    // ENSUITE, pour chaque paire de couche cachee c'est "i" ; pour chaque neurone d'une couche c'est "j" 
    // pour chaque synapse connecté à un neurone c'est "k"

    public void create_liste_poids_cc()
    {
        //initialise le tableau
        liste_poids_cc = new List<List<List<float>>>(); //va contenir tout les poids des synpases 
        
        for (int i = 0; i < nb_couches_cachees-1; i++) // pour chaque paire de couches successives
        {
            liste_poids_cc.Add(new List<List<float>>()); //va contenir tout les poids des synpases de la couche 
        }
        for (int i = 0; i < nb_couches_cachees-1; i++) // pour chaque paire de couches successives
        {
            for (int j = 0; j < nb_neurone_par_couche_cachee; j++)//chaque neurone de la couche suivante
            {
                liste_poids_cc[i].Add(new List<float>()); //va contenir tout les poids des synpases qui vont vers ce neurone
            }
            for (int j = 0; j < nb_neurone_par_couche_cachee; j++)//chaque neurone de la couche suivante
            {
                //remplie le tableau
                for (int k = 0; k < nb_neurone_par_couche_cachee; k++) //chaque synpase de la couche précédente vers ce neurone
                {
                    liste_poids_cc[i][j].Add(Random.Range(-max_poids_generation,max_poids_generation));
                }
            }
        }
    }

    public void create_liste_poids_entrees()
    {
        List<float> entrees = new List<float> {0f, 0f, 0f, 0f, 0f};
        liste_poids_entrees = new List<List<float>>();
        for (int j = 0; j < nb_neurone_par_couche_cachee; j++)//chaque neurone de la couche suivante
        {
            liste_poids_entrees.Add(new List<float>()); //va contenir tout les poids des synpases qui vont vers lui
            for (int k = 0; k < entrees.Count; k++) //un synapse par neurone de la couche précédente
            {
                liste_poids_entrees[j].Add(Random.Range(-max_poids_generation,max_poids_generation));
            }
        }
    }

    public void create_liste_poids_sorties()
    {
       liste_poids_sorties = new List<List<float>>();
        for (int j = 0; j < nb_neurone_par_couche_cachee; j++) //chaque neurone de la couche suivante
        {
            liste_poids_sorties.Add(new List<float>()); //va contenir tout les poids des synpases qui vont vers lui
            for (int k = 0; k < nb_sorties; k++) //un synapse par neurone de la couche précédente
            {
                liste_poids_sorties[j].Add(Random.Range(-max_poids_generation,max_poids_generation));
            }
        }
    }

    float fonction_d_activation(float entree_neurone)
    {
        return 1f/(1f+Mathf.Exp(-entree_neurone));
    }

    public List<float> compute(List<float> inputs)
    {
        entrees = inputs;
        List<float> precedent = new List<float>();
        List<float> suivant = new List<float>();

        //on calcul ce qu'on envoie à la couche cachée 1, que l'on stocke dans précédent

        for(int j = 0; j < nb_neurone_par_couche_cachee; j++)
        {
            precedent.Add(0);
            suivant.Add(0); // j'en profite pour initialiser suivant 
            for(int k = 0; k < entrees.Count; k++) 
            {
                precedent[j]+=entrees[k]*liste_poids_entrees[j][k];
            }
        }

        //on utilise la fonction d'activation pour transformer l'entrée de la couche 1 en sortie de la couche 1 

        for(int j = 0; j < nb_neurone_par_couche_cachee; j++)
        {
            precedent[j] = fonction_d_activation(precedent[j]); //on supprime l'entrée on note que la sortie dans notre tableau, on en a plus besoin
        }

        //on passe de la première à la dernière couche cachee
        for (int i = 0; i < nb_couches_cachees-1 ; i++)
        {   
            //calcule l'entrée
            for (int j = 0 ; j < suivant.Count; j++)
            {
                for(int k = 0 ; k < precedent.Count && k < liste_poids_cc[i][j].Count; k++) 
                {
                    suivant[j] =+ precedent[k]*liste_poids_cc[i][j][k];
                }
            }
            //échange suivant et précédent et transforme entrée en sortie 
            for (int j = 0 ; j < nb_neurone_par_couche_cachee ; j++)
            {
                precedent[j] = fonction_d_activation(suivant[j]);
                suivant[j] = 0;
            }
        }

        //on passe de la dernière couche à la sortie  

        List<float> resultat = new List<float>();

        for(int j = 0 ; j < nb_sorties ; j++)
        {
            resultat.Add(0f);
            for(int k = 0; k < precedent.Count && k < liste_poids_sorties[j].Count; k++)
            {
                resultat[j] += precedent[k]*liste_poids_sorties[j][k];
            }
        }
        return resultat;
    }

    int analysis(float compute_result)//TODO 
    {
        return 1; 
    }

    public void random_entrees(int nb_entrees, float valeur_max)
    {
        for(int i=0;i<nb_entrees;i++)
        {
            if(entrees.Count<nb_entrees)entrees.Add(Random.Range(-valeur_max,valeur_max));
            else entrees[i]=Random.Range(-1f,1f);
        }

    }

        void create_bar_graphs()
    {
        //créer les graphs avant de les remplir 
        bar_graphs = new List<List<int>>();
        for(int i = 0 ; i<4 ; i++)
        {
            List<int> new_bar_graph = new List<int>();
            bar_graphs.Add(new_bar_graph);
            for(int j=0 ; j<11 ; j++)//10 case de graphs et 1 pour ceux qui sont à l'ext des limites du graph
            {
                bar_graphs[i].Add(0);
            }
        }
        
    }

    void fill_bar_graphs(float resultat)
    {
        //remplir les graphs
        float interval = 0.1f;
        bool in_bool = false;
        for(int i=0 ; i<4 ; i++)
        {
            //add_to_graph(i,resultat);
            if(resultat>10*interval)
            {
                bar_graphs[i][10]++;
                in_bool = true;
            }
            for(int j=0 ; j<10 ; j++)
            {
                if(!in_bool)
                {
                    if(resultat<(j+1)*interval)
                    {
                        bar_graphs[i][j]++;   
                        in_bool = true;
                    }
                }
            }
            in_bool = false;
            interval = interval*10;
        }
    }

    void show_bar_graphs()
    {
        int nb=1;
        for(int i=0 ; i<4 ; i++)
        {
            Debug.Log("show_bard_graphs numéro: "+nb);
            for(int j=0 ; j<11 ; j++)
            {
                Debug.Log(bar_graphs[i][j]);
            }
            nb = nb*10;
        }
    }

    void test(int boucle)//ne respecte pas la règle de i , j ,k 
    {
        List<float> resultat;
        create_bar_graphs();
        for(int i=0;i<boucle;i++)
        {
            random_entrees(10,1f);
            create_liste_poids_cc();
            create_liste_poids_entrees();
            create_liste_poids_sorties();
            resultat = compute(entrees);
            for(int j = 0; j < resultat.Count; j++)
            {
                fill_bar_graphs(Mathf.Abs(resultat[j]));
            }

            //int comportement =analysis(resultat);
            //Debug.Log("test, resultat : "+resultat);
        }
        show_bar_graphs();
    }

    public static Reze_de_neurones merge(Reze_de_neurones reze1, Reze_de_neurones reze2)
    {
        const float MUTATION_OFFSET = 0.1f;

        Reze_de_neurones rezeMerge = new Reze_de_neurones();
        
        /*
         *
        public List<List<List<float>>> liste_poids_cc;
        public List<List<float>> liste_poids_entrees;
        public List<List<float>> liste_poids_sorties;
        */
        rezeMerge.liste_poids_cc = new List<List<List<float>>>();
        for (int i = 0; i < reze1.liste_poids_cc.Count; i++) {
            rezeMerge.liste_poids_cc.Add(new List<List<float>>());
            for (int j = 0; j < reze1.liste_poids_cc[i].Count; j++) {
                rezeMerge.liste_poids_cc[i].Add(new List<float>());
                for (int k = 0; k < rezeMerge.liste_poids_cc[i][j].Count; k++) {
                    rezeMerge.liste_poids_cc[i][j].Add(Random.Range(
                        Mathf.Min(reze1.liste_poids_cc[i][j][k], reze2.liste_poids_cc[i][j][k]) - MUTATION_OFFSET,
                        Mathf.Max(reze1.liste_poids_cc[i][j][k], reze2.liste_poids_cc[i][j][k]) + MUTATION_OFFSET
                    ));
                }
            }
        }

        rezeMerge.liste_poids_entrees = new List<List<float>>();
        for (int i = 0; i < reze1.liste_poids_entrees.Count; i++) {
            rezeMerge.liste_poids_entrees.Add(new List<float>());
            for (int j = 0; j < reze1.liste_poids_entrees[i].Count; j++) {
                rezeMerge.liste_poids_entrees[i].Add(Random.Range(
                    Mathf.Min(reze1.liste_poids_entrees[i][j], reze2.liste_poids_entrees[i][j]) - MUTATION_OFFSET,
                    Mathf.Max(reze1.liste_poids_entrees[i][j], reze2.liste_poids_entrees[i][j]) + MUTATION_OFFSET
                ));
            }
        }

        rezeMerge.liste_poids_sorties = new List<List<float>>();
        for (int i = 0; i < reze1.liste_poids_sorties.Count; i++) {
            rezeMerge.liste_poids_sorties.Add(new List<float>());
            for (int j = 0; j < reze1.liste_poids_sorties[i].Count; j++) {
                rezeMerge.liste_poids_sorties[i].Add(Random.Range(
                    Mathf.Min(reze1.liste_poids_sorties[i][j], reze2.liste_poids_sorties[i][j]) - MUTATION_OFFSET,
                    Mathf.Max(reze1.liste_poids_sorties[i][j], reze2.liste_poids_sorties[i][j]) + MUTATION_OFFSET
                ));
            }
        }

        return rezeMerge;
    }
}
