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
        if (collision.transform.root.tag == "Pod") {
            Health otherCarHealth;
            Transform otherCar = collision.transform;
            Rigidbody rb;
           

            if (collision.transform.root.GetComponent<Rigidbody>() != null) {
                rb = collision.transform.root.GetComponent<Rigidbody>();
            } else if (collision.transform.root.GetComponent<Rigidbody>() != null) {
                rb = collision.transform.root.GetComponent<Rigidbody>();
            }

            if (otherCar != null) {
                float angle = Vector3.Angle(otherCar.forward, transform.position - otherCar.transform.position);
                float forceOfImpact = Mathf.Cos(angle) * thisCarController.velocity.magnitude * damageMultiplier;
                if (forceOfImpact > 0) {
                    Debug.Log(forceOfImpact);
                    ClientBehaviour.instance.TakeDamage(otherCar.transform.gameObject.GetComponent<NetworkPlayer>().id, forceOfImpact);
                }
            }
        }
    }
}
