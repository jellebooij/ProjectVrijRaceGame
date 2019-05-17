using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour {
    public float currentBoostLevel;
    public float maxBoostLevel = 100;
    public float boostRegenerationSpeed = 10f;
    public float boostFallOffSpeed = 20f;
    public float boostMultiplier = 4f;

    public CarController carController;
    // Start is called before the first frame update
    void Start() {
        currentBoostLevel = maxBoostLevel;
        carController = this.gameObject.GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update() {
        BoostControl();

        if (currentBoostLevel <= 0) {
            currentBoostLevel = 0;
        }
    }

    private void BoostRegeneration() {
        if (currentBoostLevel < maxBoostLevel) {
            currentBoostLevel += boostRegenerationSpeed * Time.deltaTime;
        }
    }

    private void BoostControl() {
        if (Input.GetButton("Jump")) {
            currentBoostLevel -= boostFallOffSpeed * Time.deltaTime;
            carController.currentSpeed = carController.speed * boostMultiplier;
        } else {
            carController.currentSpeed = carController.speed;
            BoostRegeneration();
        }
    }
}
