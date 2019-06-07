using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSwitch : MonoBehaviour
{

    public bool isOn;
    public GameObject shield;
    public float time;
   

    private void Update() {
        if(time > 0) {
            time -= Time.deltaTime;
        } else {
            shield.SetActive(false);
        }
    }

    // Update is called once per frame
    public void EnableShield()
    {
        time = 10;
        shield.SetActive(true);
    }
    public void DisableShield() {
        shield.SetActive(true);
    }
}
