using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBehaviour : MonoBehaviour {
    
    private PowerupController controller;

    private Transform transform;

    private CarController carController;

    public string powerupName;


    private void Awake() {
        transform = base.transform;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "GameController") {
            carController = other.GetComponentInParent<CarController>();
            controller = carController.gameObject.GetComponentInParent<PowerupController>();

            Destroy(gameObject);
        }
    }
}
