using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunBullet : MonoBehaviour
{

    public float speed = 20f;
    public float lifeTime = 1f;
    private float timer;
    public LayerMask layerMask;
    public bool isOwner = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > lifeTime) {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {
        CheckRayCollision();
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void CheckRayCollision() {
        
            RaycastHit hit;
            if (Physics.Linecast(transform.position, transform.position + transform.forward * speed * Time.deltaTime, out hit, layerMask)) {
                if (hit.transform.gameObject.GetComponent<NetworkPlayer>() != null) {
                    if (isOwner)
                    {
                        ClientBehaviour.instance.TakeDamage(hit.transform.gameObject.GetComponent<NetworkPlayer>().id, 2f);
                    }
                    Debug.Log("Hit Player");
                }
                Destroy(gameObject);
            }
        }
    }
}
