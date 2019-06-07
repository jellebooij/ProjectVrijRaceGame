using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarding : MonoBehaviour {

    private Vector3 targetPosition;

    // Update is called once per frame
    void Update() {
        targetPosition = Camera.main.transform.position;
        transform.LookAt(targetPosition, Vector3.up);
    }
}
