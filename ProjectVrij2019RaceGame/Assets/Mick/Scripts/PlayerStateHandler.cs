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

    // Start is called before the first frame update
    void Start()
    {
        myCarTarget = GetComponent<Transform>();
        targetIndex = 1;
        type = PlayerState.Playing;
        otherCarTransforms = parentOfOtherCarTransforms.GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Debug.Log("Spectating");
            type = PlayerState.Spectating;
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            type = PlayerState.Playing;
        }
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
        if (Input.GetButtonDown("Fire1")) {
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
