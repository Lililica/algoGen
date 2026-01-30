using UnityEngine;

public class camera_management : MonoBehaviour
{
    public GameObject camera_holder1;
    public GameObject camera_holder2;
    public Transform spawn_point;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click()
    {
        if(camera_holder1.activeSelf)
        {
            camera_holder2.SetActive(true);
            camera_holder1.SetActive(false);
        }
        else 
        {
            camera_holder2.SetActive(false);
            camera_holder1.SetActive(true);
        }
        camera_holder1.transform.position = spawn_point.position;
        camera_holder2.transform.position = spawn_point.position;

    }
}
