using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class CarController : MonoBehaviour {
    public float drag;
    public float rotationDrag;

    public float currentSpeed;
    public float speed = 5f;

    public LayerMask hitmask;
    Rigidbody rb;
    Vector3[] cornersLocal = new Vector3[4];
    Vector3[] cornersWorld = new Vector3[4];

    private float yRotation;
    private Vector3 velocity;

    void Start() {
        currentSpeed = speed;
        yRotation = transform.rotation.y;

        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = -Vector3.up * 2f;


        BoxCollider col = GetComponent<BoxCollider>();

        Vector3 extents = col.bounds.extents;

        cornersLocal[0] = new Vector3(-extents.x, -extents.y, extents.z);
        cornersLocal[1] = new Vector3(extents.x, -extents.y, extents.z);
        cornersLocal[2] = new Vector3(-extents.x, -extents.y, -extents.z);
        cornersLocal[3] = new Vector3(extents.x, -extents.y, -extents.z);

        Debug.Log(extents);
    }

    void FixedUpdate() {

        rb.drag = drag;
        rb.angularDrag = rotationDrag;

        float gasPedal = Input.GetAxisRaw("Vertical");
        float steeringWheel = Input.GetAxisRaw("Horizontal");

        rb.AddForce(new Vector3(transform.forward.x, 0, transform.forward.z) * gasPedal * currentSpeed);
        rb.AddTorque(rb.inertiaTensorRotation * transform.up * 10 * steeringWheel);

        //yRotation = Mathf.Lerp(yRotation, steeringWheel * 5 + yRotation, Time.deltaTime * 20);
        velocity = Vector3.Lerp(velocity, new Vector3(transform.forward.x, 0, transform.forward.z) * gasPedal * 0.01f * currentSpeed, Time.deltaTime * 5);

        //rb.MoveRotation(Quaternion.Euler(transform.rotation.x, yRotation, transform.rotation.z));
        rb.MovePosition(transform.position + velocity);

        Matrix4x4 carTransform = transform.localToWorldMatrix;
        Vector3 scale = transform.localScale;
        carTransform *= Matrix4x4.Scale(new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z));

        for (int i = 0; i < cornersLocal.Length; i++) {

            cornersWorld[i] = carTransform.MultiplyPoint3x4(cornersLocal[i]);
            Debug.DrawLine(cornersWorld[i], cornersWorld[i] - transform.up, Color.red);

            RaycastHit hit;
            float hitDistance = 1;

            if (Physics.Linecast(cornersWorld[i], cornersWorld[i] - transform.up, out hit, hitmask)) {
                hitDistance = hit.distance;
            }

            float mass = rb.mass / 8;
            float compressionRatio = 1 - hitDistance;
            float force = mass * Physics.gravity.magnitude * compressionRatio * 10f;

            rb.AddForceAtPosition(transform.up * force, cornersWorld[i]);

        }

        Debug.DrawLine(transform.position, transform.position + Vector3.down * 20);

        rb.AddForce(-transform.up * 3);
    }


}


