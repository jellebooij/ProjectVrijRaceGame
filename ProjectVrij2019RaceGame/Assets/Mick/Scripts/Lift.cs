using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour {

    public float speed = 10f;
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += Vector3.up * Time.deltaTime * speed;
        transform.RotateAround(transform.position, Vector3.up, 10f * Time.deltaTime);
    }
}
