using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpDamage : MonoBehaviour {

    public CarController thisCarController;
    public Health healthReference;
    public float damageMultiplier = 1f;
    // Start is called before the first frame update
    void Start() {
        thisCarController = GetComponent<CarController>();
        healthReference = GetComponent<Health>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Pod") {
            Health otherCarHealth;
            CarController otherCar = null;
            Rigidbody rb;
            if (collision.transform.GetComponentInParent<Health>() != null) {
                otherCarHealth = collision.transform.GetComponentInParent<Health>();
            } else if (collision.transform.GetComponent<Health>() != null) {
                otherCarHealth = collision.transform.GetComponent<Health>();
            }

            if (collision.transform.GetComponentInParent<CarController>() != null) {
                otherCar = collision.transform.GetComponentInParent<CarController>();
            } else if (collision.transform.GetComponent<CarController>() != null) {
                otherCar = collision.transform.GetComponent<CarController>();
            }

            if (collision.transform.GetComponentInParent<Rigidbody>() != null) {
                rb = collision.transform.GetComponentInParent<Rigidbody>();
            } else if (collision.transform.GetComponent<Rigidbody>() != null) {
                rb = collision.transform.GetComponent<Rigidbody>();
            }

            float angle = Vector3.Angle(otherCar.velocity, transform.position - otherCar.transform.position);
            float forceOfImpact = Mathf.Cos(angle) * thisCarController.velocity.magnitude * damageMultiplier;
            if (forceOfImpact > 0) {
                ClientBehaviour.instance.TakeDamage(otherCar.transform.gameObject.GetComponent<NetworkPlayer>().id, forceOfImpact);
            }
        }
    }
}
