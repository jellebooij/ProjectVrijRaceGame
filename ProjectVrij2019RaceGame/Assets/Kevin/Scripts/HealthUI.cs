using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour {

    public Image healthBarLeft;
    public Image healthBarRight;
    public Image healthIcon;
    public TextMeshProUGUI healthText;
    public Health health;

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

        //if (Input.GetKeyDown(KeyCode.E)) {
        //    health.health -= 10f;
        //}
        //health.health += 0.1f;
    }

    private void HealthBar() {
        healthBarLeft.fillAmount = health.health / health.maxHealth;
        healthBarRight.fillAmount = health.health / health.maxHealth;
        healthText.text = Mathf.RoundToInt(Mathf.Clamp(health.health, 0, health.maxHealth)).ToString() + "%";

        if (health.health < dangerLevel) {
            healthBarLeft.color = lowColor;
            healthBarRight.color = lowColor;
            healthText.color = lowColor;
            healthIcon.color = lowColor;
        } else {
            healthBarLeft.color = fullColor;
            healthBarRight.color = fullColor;
            healthText.color = whiteColor;
            healthIcon.color = fullColor;
            //healthBarLeft.color = Color.Lerp(lowColor, fullColor, healthBarLeft.fillAmount);
            //healthBarRight.color = Color.Lerp(lowColor, fullColor, healthBarRight.fillAmount);
        }
    }
}
