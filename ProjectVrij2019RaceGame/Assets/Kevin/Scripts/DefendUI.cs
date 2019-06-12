using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefendUI : MonoBehaviour {

    public PowerupController powerUp;
    private Vector2 refPos;
    private Vector2 refSize;
    private string attackName;

    [SerializeField] private RectTransform defenseInactive;
    [SerializeField] private RectTransform defenseActive;
    [SerializeField] private Image defendTimer;
    [SerializeField] private Image defendIcon;
    [SerializeField] private TextMeshProUGUI defendText;

    public List<Sprite> icons = new List<Sprite>();

    private void Start() {
        Deactivate(defenseInactive, defenseActive, new Vector2(-800, 0), 0.075f);
    }

    private void Update() {
        if (powerUp.currentDefensePowerup.type == PowerupType.Shield) {
            Activate(defenseInactive, defenseActive, icons[0], "Shield", new Vector2(-800, 0), 0.075f);
        }
        else if (powerUp.currentDefensePowerup.type == PowerupType.None) {
            Deactivate(defenseInactive, defenseActive, new Vector2(-800, 0), 0.075f);
        }
    }

    private void Activate(RectTransform oldState, RectTransform newState, Sprite defendSprite, string name, Vector2 refPos, float animSpeed) {
        oldState.anchoredPosition = Vector2.Lerp(oldState.anchoredPosition, refPos, animSpeed);
        newState.anchoredPosition = Vector2.Lerp(newState.anchoredPosition, Vector2.zero, animSpeed);
        defendTimer.fillAmount = powerUp.currentDefensePowerup.timer / powerUp.currentDefensePowerup.duration;
        defendIcon.sprite = defendSprite;
        attackName = name;
    }

    private void Deactivate(RectTransform oldState, RectTransform newState, Vector2 refPos, float animSpeed) {
        oldState.anchoredPosition = Vector2.Lerp(oldState.anchoredPosition, Vector2.zero, animSpeed);
        newState.anchoredPosition = Vector2.Lerp(newState.anchoredPosition, refPos, animSpeed);
    }
}
