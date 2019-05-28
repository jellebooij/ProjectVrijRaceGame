using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectiles : MonoBehaviour {

    public GameObject firePoint;
    public List<GameObject> vfx = new List<GameObject>();
    public RotateToMouse rotateToMouse;

    private GameObject effectSpawn;
    private float timeToFire = 0;

    void Start() {
        effectSpawn = vfx[0];
        
    }

    void Update() {
        if(Input.GetMouseButton(0) && Time.time >= timeToFire) {
            timeToFire = Time.time + 1 / effectSpawn.GetComponent<ProjectileMove>().fireRate;
            SpawnVFX();
            
        }

        
    }

    void SpawnVFX(){

        GameObject vfx;

        if (firePoint !=null) {
            vfx = Instantiate(effectSpawn, firePoint.transform.position, Quaternion.identity);

            if (rotateToMouse != null) {
                vfx.transform.localRotation = rotateToMouse.GetRotation();

            }

        } else {
            Debug.Log("No Fire Point");
        }
    }



}
