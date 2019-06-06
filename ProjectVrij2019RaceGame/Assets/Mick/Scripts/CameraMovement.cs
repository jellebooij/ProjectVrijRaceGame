using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour {
    public Vector3 positionOffset;
    public float smoothTime = 0.5f;
    public Transform targetTransform;
    public List<Transform> targets;
    private bool focusOnGroup = false;
    private Vector3 velocity;
    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 50f;
    private Camera cam;
	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        for (int i = 0; i < targetTransform.childCount; i++) {
            targets.Add(targetTransform.GetChild(i));
        }
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (targets.Count == 0)
            return;
        Move();
        Zoom();
	}
    void Zoom() {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    private void Move() {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + positionOffset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    private float GetGreatestDistance() {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i<targets.Count; i++) {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x;
    }

    Vector3 GetCenterPoint() {
        if (targets.Count == 1) {
            return targets[0].position;
        }
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++) {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }
}
