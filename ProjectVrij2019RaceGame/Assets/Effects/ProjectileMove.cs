using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{

    public float speed;
    public float fireRate;

    void Start()
    {
        
    }

    void Update()
    {
        if (speed != 0) {
            transform.position += transform.forward * (speed * Time.deltaTime);
            StartCoroutine("DeleteProjectile");

        } else {
            Debug.Log("No Speed");
        }
        
    }

    private void OnCollisionEnter(Collision collision) {
        speed = 0;

        Destroy(gameObject);
         
    }

    IEnumerator DeleteProjectile() {

        yield return new WaitForSeconds(3f);

        Object.Destroy(gameObject);

    }
}
