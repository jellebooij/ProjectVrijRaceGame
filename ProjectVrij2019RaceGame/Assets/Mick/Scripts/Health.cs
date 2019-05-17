using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public float maxHealth = 100;
    public float health;

    // Start is called before the first frame update
    void Start() {
        health = maxHealth;
    }
}
