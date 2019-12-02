using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    [SerializeField]
    private GameObject camRotator;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;

    private Rigidbody rigidBod;

    private void Start()
    {
        rigidBod = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public void Rotate(Vector3 rotation)
    {
        this.rotation = rotation;
    }

    public void RotateCamera(Vector3 cameraRotation)
    {
        this.cameraRotation = cameraRotation;
    }

    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    private void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rigidBod.MovePosition(rigidBod.position + velocity * Time.fixedDeltaTime);
        }
    }

    void PerformRotation ()
    {
        rigidBod.MoveRotation(rigidBod.rotation * Quaternion.Euler(rotation));
        camRotator.transform.Rotate(-cameraRotation);
    }

}
