using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public float maxHealth = 100;
    public float health;

    public float maxArmor;
    public float armor;


    // Start is called before the first frame update
    void Start() {
        health = maxHealth;
        armor = 0;
    }

    private void Update() {
        if (health > maxHealth) {
            health = maxHealth;
        }
        if (health < 0) {
        }
    }

    public void TakeDamage(float amount) {
        float combinedHealth = health + armor;
        if (amount < combinedHealth) {
            combinedHealth -= amount;
        }
        if (combinedHealth >= 100) {
            health = 100;
            armor = combinedHealth - 100;
        }
        if (combinedHealth < 100) {
            health = combinedHealth;
        }

        if (amount >= combinedHealth) {
            Debug.Log("Death");
            //perform game over
        }


    }
}
