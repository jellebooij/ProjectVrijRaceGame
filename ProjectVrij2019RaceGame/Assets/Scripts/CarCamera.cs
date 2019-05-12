using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour
{
    public Transform target;
    public float distance;
    public float height;

    private void Update() {
        
        transform.position = target.position - target.forward * distance + Vector3.up * height; 
        transform.LookAt(target);

    }
}
