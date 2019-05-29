using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform target;
    public float distance;
    private float currentDistance;
    public float height;
    public float targetPositionSmoothing = 3f;
    public Transform car;

    private Vector3 targetPosition;

    private void Awake() {
        currentDistance = distance;
    }
    private void FixedUpdate() {
        CheckCollision();
        targetPosition = target.position - new Vector3(target.forward.x, 0, target.forward.z).normalized * currentDistance;
        targetPosition.y = target.position.y + height;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * targetPositionSmoothing); 
        transform.LookAt(target);

    }

    private void CheckCollision() {
        RaycastHit hit;
        if (Physics.Linecast(target.position, target.position - new Vector3(target.forward.x, 0, target.forward.z).normalized * distance + target.up * height, out hit, layerMask)){
            Debug.Log("Hit");
            currentDistance = Vector3.Distance(targetPosition, hit.point) - 1f;
        } else {
            currentDistance = distance;
        }
    }
}
