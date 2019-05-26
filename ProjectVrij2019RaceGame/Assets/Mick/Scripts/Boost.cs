using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour {
    public float currentBoostLevel;
    public float maxBoostLevel = 100;
    public float boostRegenerationSpeed = 10f;
    public float boostFallOffSpeed = 20f;
    public float boostMultiplier = 4f;

    private float waitForNewBoostTimer = 0;
    private float waitForNewBoostTime = 2;

    public CarController carController;
    // Start is called before the first frame update
    void Start() {
        currentBoostLevel = maxBoostLevel;
        carController = this.gameObject.GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update() {
        BoostControl();

        if (waitForNewBoostTimer > 0) {
            waitForNewBoostTimer -= Time.deltaTime;
        }

        if (currentBoostLevel <= 0) {
            currentBoostLevel = 0;
            waitForNewBoostTimer = waitForNewBoostTime;
        }
    }

    private void BoostRegeneration() {
        if (currentBoostLevel < maxBoostLevel) {
            currentBoostLevel += boostRegenerationSpeed * Time.deltaTime;
        }
    }

    private void BoostControl() {
        if (Input.GetButton("Boost") && waitForNewBoostTimer <= 0) {
            currentBoostLevel -= boostFallOffSpeed * Time.deltaTime;
            carController.boostSpeed = carController.speed * boostMultiplier;
        } else {
            carController.boostSpeed = 0;
            BoostRegeneration();
        }
    }
}
