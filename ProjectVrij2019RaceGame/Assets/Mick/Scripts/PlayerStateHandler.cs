﻿using System.Collections;
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
    private Rigidbody rb;
    private float watchingPodThisLong;
    public float staticWatchingTime = 10f;
    private bool isSpectatingAutomatically;
    private PowerupController powerupController;

    // Start is called before the first frame update
    void Start()
    {
        powerupController = GetComponent<PowerupController>();
        isSpectatingAutomatically = true;
        rb = GetComponent<Rigidbody>();
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
            ClientBehaviour.instance.Die();

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

        carCamera.isSpectating = true;

        if (!rb.isKinematic) {
            rb.isKinematic = true;
        }
        powerupController.enabled = false;
        carController.enabled = false;
        pod.SetActive(false);

        if (isSpectatingAutomatically) {
            watchingPodThisLong += Time.deltaTime;
            if (watchingPodThisLong > staticWatchingTime) {
                SwitchSpectatorToPlayer();
                watchingPodThisLong = 0;
            }
        }
        otherCarTransforms = parentOfOtherCarTransforms.GetComponentsInChildren<Transform>();
        if (Input.GetButtonDown("Fire1")) {
            isSpectatingAutomatically = false;
            SwitchSpectatorToPlayer();
        }
    }

    private void SwitchSpectatorToPlayer() {
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

    private void Playing() {
        carCamera.target = myCarTarget;
    }
}
public enum PlayerState { Spectating, Playing }
