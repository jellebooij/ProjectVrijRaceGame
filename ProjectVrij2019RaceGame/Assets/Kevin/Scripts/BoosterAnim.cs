using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class BoosterAnim : MonoBehaviour {

    private CarController controller;
    private Rigidbody rb;
    public GameObject leftBooster, rightBooster, topBooster;
    private float idleRotX = 80f;
    private float idleRotXTop = 25f;
    private float rotateX, rotateXTop, rotateXSide;

    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<CarController>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        RotateBooster();
    }

    private void RotateBooster() {
        rotateX = rb.velocity.magnitude * 2.5f;
        rotateXSide = Mathf.Clamp(rotateX, 0, idleRotX);
        rotateXTop = Mathf.Clamp(rotateX, 0, idleRotXTop);

        Vector3 rotationSide = new Vector3(idleRotX + -rotateXSide, 0, 0);
        Vector3 rotationTop = new Vector3(idleRotXTop + -rotateXTop, 0, 0);

        leftBooster.transform.localRotation = Quaternion.Euler(rotationSide);
        rightBooster.transform.localRotation = Quaternion.Euler(rotationSide);
        topBooster.transform.localRotation = Quaternion.Euler(rotationTop);
    }
}
