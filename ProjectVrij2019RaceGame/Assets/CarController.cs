using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class CarController : MonoBehaviour {
    public bool inTestMode;
    public float drag;
    public float rotationDrag;
    public float rotationSpeed = 0.1f;
    private float currentRotationSpeed;
    public float minimumRotationSpeed = 2f;

    public float currentSpeed;
    public float speed = 5f;

    public LayerMask hitmask;
    Rigidbody rb;
    Vector3[] cornersLocal = new Vector3[4];
    Vector3[] cornersWorld = new Vector3[4];

    private float yRotation;
    public Vector3 velocity;

    public float downForce = -3f;

    public Vector3 extents;

    public float boostSpeed;
    public float rayHeight = 0.7f;

    public float steeringWheelHorizontal;

    void Start() {
        currentSpeed = speed;
        yRotation = transform.rotation.y;

        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = -Vector3.up * 2.5f;


        BoxCollider col = GetComponent<BoxCollider>();

        //extents = col.bounds.extents;

        cornersLocal[0] = new Vector3(-extents.x, -extents.y, extents.z);
        cornersLocal[1] = new Vector3(extents.x, -extents.y, extents.z);
        cornersLocal[2] = new Vector3(-extents.x, -extents.y, -extents.z);
        cornersLocal[3] = new Vector3(extents.x, -extents.y, -extents.z);

        Debug.Log(extents);
    }

    void FixedUpdate() {

        rb.drag = drag;
        rb.angularDrag = rotationDrag;

        float gasPedal;
        if (Input.GetButton("Gas")) { gasPedal = 1; } else gasPedal = 0;
        if (inTestMode) {
            gasPedal = 1;
        }

        steeringWheelHorizontal = Input.GetAxisRaw("Horizontal");
        float steeringWheelVertical = Input.GetAxisRaw("Vertical");

        velocity = new Vector3(transform.forward.x, transform.forward.y * 0f, transform.forward.z) * gasPedal * currentSpeed;

        if (boostSpeed == 0) {
            currentRotationSpeed = rb.velocity.magnitude*rotationSpeed + minimumRotationSpeed;
        } else if (boostSpeed > speed) {
            velocity = new Vector3(transform.forward.x, transform.forward.y * 0f, transform.forward.z) * boostSpeed;
        }


        rb.AddForce(velocity);
        //rb.AddTorque(rb.inertiaTensorRotation * transform.up * 5 * steeringWheel);
        transform.Rotate(0, steeringWheelHorizontal * currentRotationSpeed, 0, Space.World);

        //yRotation = Mathf.Lerp(yRotation, steeringWheel * 5 + yRotation, Time.deltaTime * 20);
        //velocity = Vector3.Lerp(velocity, new Vector3(transform.forward.x, 0, transform.forward.z) * gasPedal * 0.01f * currentSpeed, Time.deltaTime * 5);

        //rb.MovePosition(transform.position + velocity);

        Matrix4x4 carTransform = transform.localToWorldMatrix;
        Vector3 scale = transform.localScale;
        carTransform *= Matrix4x4.Scale(new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z));

        float compressionRatio = 2;

        for (int i = 0; i < cornersLocal.Length; i++) {

            cornersWorld[i] = carTransform.MultiplyPoint3x4(cornersLocal[i]);
            Debug.DrawLine(cornersWorld[i], cornersWorld[i] - transform.up * rayHeight, Color.red);

            RaycastHit hit;
            float hitDistance = 1;

            if (Physics.Linecast(cornersWorld[i], cornersWorld[i] - transform.up * rayHeight, out hit, hitmask)) {
                hitDistance = hit.distance;
            } 

            float mass = rb.mass / 8;
            compressionRatio = 1 - Mathf.Pow(hitDistance, 2f);
            float force = mass * Physics.gravity.magnitude * compressionRatio * 12f;

            rb.AddForceAtPosition(transform.up * force, cornersWorld[i]);

        }

        Debug.DrawLine(transform.position, transform.position + Vector3.down * 20);

        rb.AddForce(transform.up * downForce);
        RotateToFlatPosition();

    }

    private void RotateToFlatPosition() {
        Quaternion newRotation =  Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Time.deltaTime * 30);
    }


}
