using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform target;
    public float distance;
    public float targetHeight = 2f;

    private float currentDistance;

    public float height;
    public float targetPositionSmoothing = 3f;
    public float backToDistanceSmoothness = 5f;
    public Transform car;
    private CarController carController;

    public Vector3 targetPosition;

    private Vector3 positionTurnOffset;

    public bool isSpectating;

    private void Awake() {
        currentDistance = distance;
        carController = car.GetComponent<CarController>();
    }
    private void UpdateCam() {
        targetPosition = target.position - new Vector3(target.forward.x, 0, target.forward.z).normalized * currentDistance;
        targetPosition.y = target.position.y + height;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * targetPositionSmoothing);
            
        if (target.GetComponent<CarController>() != null)
            positionTurnOffset = Vector3.Lerp(positionTurnOffset, -target.right * 2 * carController.steeringWheelHorizontal, Time.deltaTime * 3f);
            
        transform.LookAt(target.position + Vector3.up * targetHeight);
    }

    private void LateUpdate() {
        CheckCollision();

        if (isSpectating)
            UpdateCam();
    }

    private void FixedUpdate() {
        if (!isSpectating)
            UpdateCam();
    }

    private void CheckCollision() {
        RaycastHit hit;
        if (Physics.Linecast(target.position, target.position - new Vector3(target.forward.x, 0, target.forward.z).normalized * distance + target.up * height, out hit, layerMask)){
            Debug.Log("Hit");
            currentDistance = Mathf.Lerp(currentDistance, Vector3.Distance(targetPosition, hit.point) - 1f , Time.deltaTime * 20f);
        } else {
            currentDistance = Mathf.Lerp(currentDistance, distance, Time.deltaTime * backToDistanceSmoothness);
        }
    }
}
