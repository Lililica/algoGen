using UnityEngine;


public class CameraController : MonoBehaviour
{

    

    public float fastSpeed = 0.05f;
    public float normalSpeed = 0.01f;
    public float movementSpeed = 0.01f;
    public float movementTime = 5f;
    public float rotationAmount = 0.15f;
    public Transform spawn_point;

    public Vector3 newPosition;
    public Quaternion newRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
    }

    void check_too_far()
    {
        Vector3 vect_distance = spawn_point.position - transform.position;
        if(vect_distance.magnitude > 50)
        {
            transform.position = spawn_point.position;
        }
    }
    
    void HandleMovementInput()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) )
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) )
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) )
        {
            newPosition += (transform.right * -movementSpeed);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);

        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
            
        }
        if(Input.GetKey(KeyCode.R))
        {
            newPosition += (transform.up * movementSpeed);
        }
        if(Input.GetKey(KeyCode.F))
        {
            newPosition += (transform.up * -movementSpeed);
        }
        // if(Input.GetKey(KeyCode.UpArrow))
        // {
        //     // Rotate up
        //     newRotation *= Quaternion.Euler(Vector3.right * -rotationAmount);
        // }
        // if(Input.GetKey(KeyCode.DownArrow))
        // {
        //     // Rotate down
        //     newRotation *= Quaternion.Euler(Vector3.right * rotationAmount);
        // }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }

}


