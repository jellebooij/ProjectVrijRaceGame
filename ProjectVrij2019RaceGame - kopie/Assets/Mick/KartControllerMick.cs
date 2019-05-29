using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class KartControllerMick : MonoBehaviour {
    public float drag;
    public float rotationDrag;
    public float downForce = 10f;
    public float speed = 30f;

    public LayerMask hitmask;
    Rigidbody rb;
    Vector3[] cornersLocal = new Vector3[4];
    Vector3[] cornersWorld = new Vector3[4];


    private Quaternion steeringRotation;

    void Start() {
        steeringRotation = Quaternion.Euler(Vector3.zero);

        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = -Vector3.up * 3;


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

        rb.AddForce(new Vector3(transform.forward.x, 0, transform.forward.z) * gasPedal * speed);

        steeringRotation =  Quaternion.Euler(transform.rotation.x, steeringRotation.eulerAngles.y + steeringWheel * 5f, transform.rotation.z);
        rb.MoveRotation(steeringRotation);

        Matrix4x4 carTransform = transform.localToWorldMatrix;
        Vector3 scale = transform.localScale;
        carTransform *= Matrix4x4.Scale(new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z));

        for (int i = 0; i < cornersLocal.Length; i++) {

            cornersWorld[i] = carTransform.MultiplyPoint3x4(cornersLocal[i]);
            Debug.DrawLine(cornersWorld[i], cornersWorld[i] - transform.up, Color.red);

            RaycastHit hit;
            float hitDistance = 1;

            if (Physics.Linecast(cornersWorld[i] + transform.up, cornersWorld[i] - transform.up * 10f, out hit, hitmask)) {
                hitDistance = hit.distance;
            }

            float mass = rb.mass / 4;
            float compressionRatio = 1 - hitDistance;
            float force = mass * Physics.gravity.magnitude * compressionRatio * 10;

            rb.AddForceAtPosition(transform.up * force, cornersWorld[i] + -transform.up);

        }

        Vector3 vectorDownForce = -Vector3.up * downForce;
        rb.AddForce(vectorDownForce);

        Debug.DrawLine(transform.position, transform.position + Vector3.down * 10);


    }
}
