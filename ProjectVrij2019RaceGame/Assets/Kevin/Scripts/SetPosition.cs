using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPosition : MonoBehaviour {

    public Vector3 offset;
    public Transform target;

    // Update is called once per frame
    void Update() {
        transform.position = target.position + offset;
    }
}
