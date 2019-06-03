using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public Transform homingTransform;
    public float damage = 70;
    public GameObject missileOwner;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.forward* Time.deltaTime * speed;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, homingTransform.position - transform.position, Time.deltaTime * rotationSpeed, 0.0f);
        // Move our position a step closer to the target.
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.gameObject != missileOwner) {
            if (other.gameObject.GetComponent<Health>() != null) {
                other.gameObject.GetComponent<Health>().TakeDamage(damage);
            }
            Destroy(this.gameObject);
        }
    }
}
