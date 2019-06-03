using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpDamage : MonoBehaviour {

    public CarController thisCarController;
    public Health healthReference;
    // Start is called before the first frame update
    void Start() {
        
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Pod") {
            CarController otherCar;
            Health otherCarHealth;
            if (collision.transform.GetComponentInParent<CarController>() != null) {
                otherCar = collision.transform.GetComponentInParent<CarController>();
            }
            if (collision.transform.GetComponentInParent<Health>() != null) {
                otherCarHealth = collision.transform.GetComponentInParent<Health>();
            }
        }
    }
}
