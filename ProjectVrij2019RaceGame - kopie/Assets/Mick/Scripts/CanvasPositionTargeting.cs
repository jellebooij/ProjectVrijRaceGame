using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasPositionTargeting : MonoBehaviour {

    public Vector3 offset;
    public Transform carPosition;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (carPosition == null) {
            return;
        }
        transform.position = carPosition.position + offset;
    }
}
