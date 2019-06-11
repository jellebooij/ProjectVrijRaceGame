using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldSpaceHealthUI : MonoBehaviour {

    public Image healthBarRight;
    public float health;

    [SerializeField]
    private float dangerLevel = 30f;

    [SerializeField]
    private Color fullColor;

    [SerializeField]
    private Color lowColor;

    [SerializeField]
    private Color whiteColor;

    private void Update() {
        HealthBar();
    }

    private void HealthBar() {
        healthBarRight.fillAmount = health / 100.0f;

        if (health < dangerLevel) {
            healthBarRight.color = lowColor;
        } else {
            healthBarRight.color = fullColor;
        }
    }
}
