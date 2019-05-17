using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour
{
    public Transform target;
    public float distance;
    public float height;
    public float targetPositionSmoothing = 3f;

    private void FixedUpdate() {
        Vector3 targetPosition = target.position - target.forward * distance + Vector3.up * height;
        targetPosition.y = target.position.y + height;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * targetPositionSmoothing); 
        transform.LookAt(target);

    }
}
