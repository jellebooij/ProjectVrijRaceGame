using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHandler : MonoBehaviour
{
    public CarCamera carCamera;
    private Transform myCarTarget;
    private int targetIndex;
    public Transform[] otherCarTransforms;
    public Transform parentOfOtherCarTransforms;
    public PlayerState type { get; protected set; }
    private Health health;
    private CarController carController;
    public GameObject pod;

    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<CarController>();
        myCarTarget = GetComponent<Transform>();
        targetIndex = 1;
        type = PlayerState.Playing;
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || health.health <= 0) {
            Debug.Log("Spectating");
            type = PlayerState.Spectating;
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            type = PlayerState.Playing;
        }
    }

    private void LateUpdate() {
        switch (type) {
            case PlayerState.Spectating:
                Spectating();
                break;
            case PlayerState.Playing:
                Playing();
                break;
        }
    }

    private void Spectating() {
        carController.enabled = false;
        pod.SetActive(false);
        otherCarTransforms = parentOfOtherCarTransforms.GetComponentsInChildren<Transform>();
        if (Input.GetButtonDown("Fire1")) {
            if (otherCarTransforms.Length <= 1) {
                return;
            }
            if (targetIndex < otherCarTransforms.Length) {
                targetIndex += 1;
            } else {
                targetIndex = 1;
            }

            carCamera.target = otherCarTransforms[targetIndex];
            carCamera.gameObject.transform.position = carCamera.targetPosition;
        }
    }

    private void Playing() {
        carCamera.target = myCarTarget;
    }
}
public enum PlayerState { Spectating, Playing }
