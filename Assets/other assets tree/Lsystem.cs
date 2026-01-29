using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Lsystem : MonoBehaviour
{

    public string chaine_instruction ; //permet que pendant que ca tourne on puisse changer l'instruction
    private string chaine_instruction_precedent; //note la chaine de l'instant précédent 

    public GameObject myPrefab;

    public GameObject prefab_direction;

    public float PI=3.1415926535897f;

    public List<Vector3> intersection_point_list = new List<Vector3>();// utilisé pour circlexcircle

    public float max_height =100f;  //la hauteur max de l'arbre , important pour la fonction qui donne la courbe des branches 
    public float min_height_branches =80f;  
    public float accelerateur_croissance = 1f; // permet de changer la croissance en hauteur du paterne C

    public bool last_bias_angle_was_180 = true; //pour l'instruction D 
    public int D_bool = 0; //pour l'instruction D

    public GameObject parent;

    public  class Morceau 
    {
        public GameObject go ;
        public int flag_extremite;//1 si c'est une extrémité 0 sinon;

        public Morceau(GameObject gameobject, int extremite)
        {
            go = gameobject;
            flag_extremite = extremite;
        }
    }

    public List<Morceau> morceau_list = new List<Morceau>();// liste de tout nos morceaux


    //règles du L system //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    private float croissance_y(float y)// "y" est la hauteur 
    {
        //y part de [min_height,max_height] , on va vert [0,1] puis vers [0,6]
        float y_centered = y*6f/(max_height-min_height_branches);//on veut centrer les données entrantes de 0 et 6 afin d'avoir une courbe jolie 
        float f_y = -2.1f +1.8f/(0.1f*(y_centered-(-2.4f))); // ces nombres sont choisis grâce à un logiciel de visualisation de courbes, ils ont été ajustés afin d'imiter la pente voulue
        return f_y*accelerateur_croissance; 
    }

    private float croissance_x(float y)// "y" est la hauteur 
    {
        float f_y =6-croissance_y(y)/accelerateur_croissance; 
        return f_y*accelerateur_croissance; 
    }

    private void circlexcircle(int i)//remplie intersection_point_list[0] et intersection_point_list[1] des points d'intersections de 2 cercles précis , suppose qu'il y a 2 intersections, on travaille en 2D à Y constant //Attention remplie aussi intersection_point_list[2]
    {
        // méthode : https://lucidar.me/fr/mathematics/how-to-calculate-the-intersection-points-of-two-circles/
        
        //les deux points doivent être à la même hauteur y 

        float size = morceau_list[i].go.transform.localScale.y;

        Vector3 P2 = morceau_list[i].go.transform.localPosition+morceau_list[i].go.transform.up*size/2f; //le centre du second cercle est l'extrémité de morceau[i]

        Vector3 P1 = morceau_list[0].go.transform.localPosition; // le centre du premier cercle est le tron de l'arbre, ce cercle contient le centre de morceau[i] d'où le calcul de d

        P1.y=P2.y; //on l'élève à la même hauteur que P2

        float d =  Mathf.Sqrt((P1.x-P2.x)*(P1.x-P2.x)+(P1.z-P2.z)*(P1.z-P2.z));// module de la droite qui les relie

        float r1=d;

        float r2= morceau_list[i].go.transform.localScale.y;
        
        float a = (r1*r1-r2*r2+d*d)/(2*d);

        float b = (r2*r2-r1*r1+d*d)/(2*d);

        float h = Mathf.Sqrt(r1*r1-a*a);

        Vector3 P5 = new Vector3(P1.x+(P2.x-P1.x)*a/d,P1.y,P1.z+(P2.z-P1.z)*a/d);

        Vector3 P3 = new Vector3(P5.x-h*(P2.z-P1.z)/d,P1.y,P5.z+h*(P2.x-P1.x)/d);

        Vector3 P4 = new Vector3(P5.x+h*(P2.z-P1.z)/d,P1.y,P5.z-h*(P2.x-P1.x)/d);

        intersection_point_list[0]=P3;
        intersection_point_list[1]=P4;
        intersection_point_list[2]=P5;
    }

        public void PlaceCylinderBetweenPoints(GameObject cylinder, Vector3 start, Vector3 end)//made by gpt 
    {
        Vector3 direction = end - start;
        Vector3 center = (start + end) / 2f;

        cylinder.transform.position = center;
        cylinder.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);

        Vector3 scale = cylinder.transform.localScale;
        scale.y = direction.magnitude / 2f;
        cylinder.transform.localScale = scale;
    }

    private float random_angle()//renvoie un entier floattant entre 0 et 360
    {
        return Random.Range(0f, 360f);

    }

    private float random_variation(float variable, int variability)//variability exprimée en %
    {
        return Random.Range(variable*(1-variability/100f),variable*(1+variability/100f));
    }
    
    private void Lprogram(char instruction,int i) 
    {

        if  (instruction=='0')//si c'est le premier morceau on le créer en 0,0,0 
        {

            Morceau morceau = new Morceau(myPrefab,1);
            Vector3 pos =new Vector3(0, 0,0); 
            Quaternion rotation = morceau.go.transform.localRotation;
            //        Quaternion rot = Quaternion.Euler(rotation.x,rotation.y ,rotation.z);

            int flag_extremite=0;
            if (morceau.flag_extremite==1)// si le morceau était une extrémité, il n'en est plus et le nouveau morceau le devient
            {
                flag_extremite=1;

            }
        
            // on créer notre nouvelle tige là où elle doit être 
            Morceau nouveau = new Morceau(Instantiate(myPrefab, pos, rotation, parent.transform),flag_extremite) ;
            
            //Debug.Log("0");
            morceau_list.Add(nouveau);//on ajoute le nouveau morceau à la liste 


        }
        else if  (instruction=='A')//première règle , A patern ajouter une branche au bout d'une branche , les deux sont alignés
        {
            Morceau morceau = morceau_list[i];
            Transform t = morceau.go.transform;
            float size = t.localScale.y;
            Vector3 rot = t.localEulerAngles; 
            Vector3 pos = t.localPosition;
            Vector3 position = pos + t.up * size;
            Quaternion rotation = Quaternion.Euler(rot.x,rot.y ,rot.z);

            int flag_extremite=0;
            if (morceau.flag_extremite==1)// si le morceau était une extrémité, il n'en est plus et le nouveau morceau le devient
            {
                flag_extremite=1;
                morceau_list[i].flag_extremite=0;

            }
        
            // on créer notre nouvelle tige là où elle doit être 
            Morceau nouveau = new Morceau(Instantiate(myPrefab, position, rotation, parent.transform),flag_extremite) ;
            
            morceau_list.Add(nouveau);//on ajoute le nouveau morceau à la liste 
            
            //Debug.Log("A");

        }

        else if (instruction=='B') //on va faire deux branches qui partent à droite et gauche 
        {

            //récupérer le morceau 

            Morceau morceau = morceau_list[i];

            int flag_extremite=0;
            if (morceau.flag_extremite==1)// si le morceau était une extrémité, il n'en est plus et le nouveau morceau le devient
            {
                flag_extremite=1;
                morceau_list[i].flag_extremite=0;
            }

            Transform t = morceau.go.transform;
            float size = t.localScale.y;
            
            Morceau nouveau1 = new Morceau(Instantiate(myPrefab, t.localPosition+t.up * size/2f, t.localRotation, parent.transform),flag_extremite) ;

            //modifier le morceau

            Transform t_nouveau1  = nouveau1.go.transform; 
            t_nouveau1.localEulerAngles =new Vector3(t.localRotation.x,t.localRotation.y+random_angle(),t.localRotation.z+45);//d'abord rotate
            t_nouveau1.localPosition+=t_nouveau1.up*size/2f;//puis position, pcq position dépend de rotate 
            
            morceau_list.Add(nouveau1);

            Morceau nouveau2 = new Morceau(Instantiate(myPrefab, t.localPosition+t.up * size/2f, t.localRotation, parent.transform),flag_extremite) ;

            Transform t_nouveau2 = nouveau2.go.transform; 
            t_nouveau2.localEulerAngles =new Vector3(t.localRotation.x,t.localRotation.y+random_angle() ,t.localRotation.z-45);
            t_nouveau2.localPosition+=t_nouveau2.up*size/2f;

            morceau_list.Add(nouveau2);
            
            //Debug.Log("B");
        }

        else if (instruction=='C') //on va faire deux branches qui partent à droite et gauche , on utilise le calcul d'intersections de cercle pour trouver leur position et rotation
        {
            

            //récupérer le morceau duquel on fait pousser 

            Morceau morceau = morceau_list[i];
            Transform t = morceau.go.transform; 

            // si le morceau était une extrémité, il n'en est plus et le nouveau morceau le devient

            int flag_extremite=0;
            if (morceau.flag_extremite==1)
            {
                flag_extremite=1;
                morceau_list[i].flag_extremite=0;
            }

            float size = morceau.go.transform.localScale.y;

            //on calcule les positions et rotations des nouveaux cylindres, ils sont mis dans intersection point list 
            circlexcircle(i);

            // creér le premier nouveau morceau

            Morceau nouveau1 = new Morceau(Instantiate(myPrefab, t.localPosition, t.localRotation, parent.transform),flag_extremite) ;

            //on place le cylindre au bon endroit dans la bonne rotation, pour cela d'abord on trouve la bonne direction

            Vector3 start = morceau_list[i].go.transform.localPosition+morceau_list[i].go.transform.up*size/2f;

            Vector3 up = new Vector3(0,1,0);
            Vector3 origine = new Vector3(0,0,0);
            Vector3 copie_intersection1=intersection_point_list[0];
            copie_intersection1.y=0;
            PlaceCylinderBetweenPoints(prefab_direction,origine,copie_intersection1);//on trouve la position de laquelle doivent partirent nos points en placant prefab_direction au bon endroit
            float y = morceau_list[i].go.transform.localPosition.y;
            float c_x = croissance_x(y);
            float c_y = croissance_y(y);

            Vector3 end1 = intersection_point_list[0]+c_y*up+c_x*prefab_direction.transform.up; // intersection_point_list est le résultat de notre intersection de cercles, les croissance ont pour but que l'arbre prenne la forme d'une courbe
            end1.x=random_variation(end1.x,5);
            end1.y=random_variation(end1.y,5);
            end1.z=random_variation(end1.z,5);

            PlaceCylinderBetweenPoints(nouveau1.go,start,end1);//puis on le place et rotate 

            morceau_list.Add(nouveau1);

            //le second 

            Morceau nouveau2 = new Morceau(Instantiate(myPrefab, t.localPosition, t.localRotation, parent.transform),flag_extremite) ;

            //on place le cylindre au bon endroit dans la bonne rotation, pour cela d'abord on trouve la bonne direction

            Vector3 copie_intersection2=intersection_point_list[1];
            copie_intersection2.y=0;
            PlaceCylinderBetweenPoints(prefab_direction,origine,copie_intersection2);

            Vector3 end2 = intersection_point_list[1]+c_y*up+c_x*prefab_direction.transform.up; // intersection_point_list est le résultat de notre intersection de cercles, les croissance ont pour but que l'arbre prenne la forme d'une courbe
            end2.x=random_variation(end2.x,5);
            end2.y=random_variation(end2.y,5);
            end2.z=random_variation(end2.z,5);

            PlaceCylinderBetweenPoints(nouveau2.go,start,end2);//puis on le place et rotate 

            morceau_list.Add(nouveau2);



            // Morceau nouveau2 = new Morceau(Instantiate(myPrefab, t.localPosition, t.localRotation),flag_extremite) ;

            // Vector3 start2 = morceau_list[i].go.transform.localPosition+morceau_list[i].go.transform.up*size/2f;

            // PlaceCylinderBetweenPoints(nouveau2.go,start2,intersection_point_list[1]);//on place le cylindre au bon endroit dans la bonne rotation 

            // morceau_list.Add(nouveau2);
           
            //Debug.Log("C");
        }

        else if (instruction=='D') //trois branchent 
        {
            //récupérer l'angle , 180 ou 0 pour faire tout tourner 
            int bias_angle = 0;
            if(last_bias_angle_was_180) bias_angle = 0;
            else bias_angle = 180;
            last_bias_angle_was_180 =! last_bias_angle_was_180;

            //récupérer le morceau 

            Morceau morceau = morceau_list[i];

            int flag_extremite=0;
            if (morceau.flag_extremite==1)// si le morceau était une extrémité, il n'en est plus et le nouveau morceau le devient
            {
                flag_extremite=1;
                morceau_list[i].flag_extremite=0;
            }

            Transform t = morceau.go.transform;
            float size = t.localScale.y;

            //pour mettre le placement selon la courbe qui dirige "C"
            Vector3 up = new Vector3(0,1,0);
            Vector3 origine = new Vector3(0,0,0);
            float y = t.localPosition.y;
            float c_x = croissance_x(y);
            float c_y = croissance_y(y);
            
            Morceau nouveau1 = new Morceau(Instantiate(myPrefab, t.localPosition+t.up * size/2f, t.localRotation, parent.transform),flag_extremite) ;

            //modifier le morceau

            Transform t_nouveau1  = nouveau1.go.transform; 
            t_nouveau1.localEulerAngles =new Vector3(random_variation(t.localRotation.x,5),random_variation(t.localRotation.y-60,50)+bias_angle,random_variation(t.localRotation.z+45,5));//d'abord rotate
            t_nouveau1.localPosition+=t_nouveau1.up*size/2f;//puis position, pcq position dépend de rotate 
            
            //pour mettre le placement selon la courbe qui dirige "C"
            Vector3 direction=t_nouveau1.up*t_nouveau1.localScale.y/2f;//l'extrémité du morceau 
            direction.y=0;
            PlaceCylinderBetweenPoints(prefab_direction,origine,direction);
            Vector3 end1 = t_nouveau1.localPosition+c_y*up+c_x*prefab_direction.transform.up;

            if(D_bool==0)
            {
                end1.x-=(end1.x-t_nouveau1.localPosition.x)*5/10f;
                end1.y-=(end1.y-t_nouveau1.localPosition.y)*9/10f;
                end1.z-=(end1.z-t_nouveau1.localPosition.z)*5/10f;
            }
            else if(D_bool<4)
            {
                end1.x-=(end1.x-t_nouveau1.localPosition.x)*3/10f;
                end1.y-=(end1.y-t_nouveau1.localPosition.y)*5/10f;
                end1.z-=(end1.z-t_nouveau1.localPosition.z)*3/10f;
            }


            PlaceCylinderBetweenPoints(nouveau1.go,t_nouveau1.localPosition,end1);

            morceau_list.Add(nouveau1);

            //morceau 2

            Morceau nouveau2 = new Morceau(Instantiate(myPrefab, t.localPosition+t.up * size/2f, t.localRotation, parent.transform),flag_extremite) ;

            Transform t_nouveau2 = nouveau2.go.transform; 
            t_nouveau2.localEulerAngles =new Vector3(random_variation(t.localRotation.x,5),random_variation(t.localRotation.y+240,50)+bias_angle, random_variation(t.localRotation.z-45,5));
            t_nouveau2.localPosition+=t_nouveau2.up*size/2f;

            //pour mettre le placement selon la courbe qui dirige "C"
            direction=t_nouveau2.up*t_nouveau2.localScale.y/2f;//l'extrémité du morceau 
            direction.y=0;
            PlaceCylinderBetweenPoints(prefab_direction,origine,direction);
            Vector3 end2 = t_nouveau2.localPosition+c_y*up+c_x*prefab_direction.transform.up;
            if(D_bool==0)
            {
                end2.x-=(end2.x-t_nouveau2.localPosition.x)*5/10f;
                end2.y-=(end2.y-t_nouveau2.localPosition.y)*9/10f;
                end2.z-=(end2.z-t_nouveau2.localPosition.z)*5/10f;
            }
            else if(D_bool<4)
            {
                end2.x-=(end2.x-t_nouveau2.localPosition.x)*3/10f;
                end2.y-=(end2.y-t_nouveau2.localPosition.y)*5/10f;
                end2.z-=(end2.z-t_nouveau2.localPosition.z)*3/10f;
            }
            PlaceCylinderBetweenPoints(nouveau2.go,t_nouveau2.localPosition,end2);

            morceau_list.Add(nouveau2);

            //morceau 3

            Morceau nouveau3 = new Morceau(Instantiate(myPrefab, t.localPosition+t.up * size/2f, t.localRotation, parent.transform),flag_extremite) ;

            Transform t_nouveau3 = nouveau3.go.transform; 
            t_nouveau3.localEulerAngles =new Vector3(random_variation(t.localRotation.x,5),random_variation(t.localRotation.y+1,50)+bias_angle, random_variation(t.localRotation.z-45,5));
            t_nouveau3.localPosition+=t_nouveau3.up*size/2f;

            //pour mettre le placement selon la courbe qui dirige "C"
            direction=t_nouveau3.up*t_nouveau3.localScale.y/2f;//l'extrémité du morceau 
            direction.y=0;
            PlaceCylinderBetweenPoints(prefab_direction,origine,direction);
            Vector3 end3 = t_nouveau3.localPosition+c_y*up+c_x*prefab_direction.transform.up;
            if(D_bool==0)
            {
                end3.x-=(end3.x-t_nouveau3.localPosition.x)*5/10f;
                end3.y-=(end3.y-t_nouveau3.localPosition.y)*9/10f;
                end3.z-=(end3.z-t_nouveau3.localPosition.z)*5/10f;
                D_bool++;
            }
            else if(D_bool<4)
            {
                end3.x-=(end3.x-t_nouveau3.localPosition.x)*3/10f;
                end3.y-=(end3.y-t_nouveau3.localPosition.y)*5/10f;
                end3.z-=(end3.z-t_nouveau3.localPosition.z)*3/10f;
                D_bool++;
            }
            PlaceCylinderBetweenPoints(nouveau3.go,t_nouveau3.localPosition,end3);

            morceau_list.Add(nouveau3);
            
            //Debug.Log("D");
        }
                
        else 
        {
            Debug.Log("mauvaise instruction");
        }
    }

    private void Step(char instruction)// fait une étape du Lsystem
    {
        if (morceau_list.Count==0)
        {
            Lprogram('0',0);
        }
        else 
        {
        //on cherche chaque extrémité à laquelle on va appliquer la prochaine étape du L_system
            int Count = morceau_list.Count;
            for (int i=0; i<Count;i++)
            {
                if (morceau_list[i].flag_extremite==1) //si c'est une extrémité on fait pousser notre plante 
                {
                    Lprogram(instruction,i);
                }
            }
        }


    }

    public void Plantifier(string chaine)
    {
        D_bool=0;
        for(int i=0;i<chaine.Length;i++)
        {
           Step(chaine[i]);
        }
    }

    private void Deplantifier()
    {
        int Count = morceau_list.Count;
        for (int i=0; i<Count;i++)
        {
           Destroy(morceau_list[0].go);
           morceau_list.RemoveAt(0);
        }
    }

    bool coucou = true;
    void Start()
    {
        //pour que l'utilisation de intersection_point_list marche bien 
        Vector3 identity =new Vector3(1,1,1);
        for (int i=0;i<5;i++)
        {
            intersection_point_list.Add(identity);
        }
        
        //pour que chaine_instruction_precedent marche bien 
        chaine_instruction_precedent ="";
    }

    void Update()
    {
        
        if(chaine_instruction_precedent!=chaine_instruction)//compare à chaque instant ce qu'il y a écrit et ce qui est dessiné pour voir s'il faut le changer
        {
        Deplantifier();
        Plantifier(chaine_instruction);
        //GameObject arbre = Instantiate(my_tree_sprite, position, rotation);
        group_objects g_o = parent.AddComponent<group_objects>();
        }
        chaine_instruction_precedent = chaine_instruction;


        if(coucou)
        {
            Debug.Log("instanciation Lsystem");
            GameObject arbreInstance = Instantiate(parent, new Vector3(0, 0, 0), Quaternion.identity);
            arbreInstance.transform.position = new Vector3(Random.Range(-50,50),0,Random.Range(-50,50));
            coucou = false;
        }

    }
}
