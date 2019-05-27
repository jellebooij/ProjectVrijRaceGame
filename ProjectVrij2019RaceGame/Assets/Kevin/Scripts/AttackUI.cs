using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackUI : MonoBehaviour {

    public PowerupController powerUp;
    private Vector2 refPos;
    private Vector2 refSize;
    private float animSpeed;
    private string attackName;

    [SerializeField] private RectTransform attackInactive;
    [SerializeField] private RectTransform attackActive;
    [SerializeField] private Image attackTimer;
    [SerializeField] private Image attackIcon;
    [SerializeField] private TextMeshProUGUI attackText;

    public List<Sprite> icons = new List<Sprite>();

    public void Activate(RectTransform oldState, RectTransform newState, Sprite attackSprite, string name, Vector2 refPos, float animSpeed) {
        oldState.anchoredPosition = Vector2.Lerp(oldState.anchoredPosition, refPos, animSpeed);
        newState.anchoredPosition = Vector2.Lerp(newState.anchoredPosition, Vector2.zero, animSpeed);
        attackTimer.fillAmount = powerUp.currentPowerup.timer / powerUp.currentPowerup.duration;
        attackIcon.sprite = attackSprite;
        attackName = name;
    }

    public void Deactivate(RectTransform oldState, RectTransform newState, Vector2 refPos, float animSpeed) {
        oldState.anchoredPosition = Vector2.Lerp(oldState.anchoredPosition, Vector2.zero, animSpeed);
        newState.anchoredPosition = Vector2.Lerp(newState.anchoredPosition, refPos, animSpeed);
    }

    private void Update() {
        if (powerUp.currentPowerup.type == PowerupType.Laser) {
            Activate(attackInactive, attackActive, icons[0], "Laser", new Vector2(250, 0), 0.075f);
        }
        else if (powerUp.currentPowerup.type == PowerupType.None) {
            Deactivate(attackInactive, attackActive, new Vector2(250, 0), 0.075f);
        }
    }
}
