using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoosterUI : MonoBehaviour {

    public Image boosterBarLeft;
    public Image boosterBarRight;
    public TextMeshProUGUI boosterText;
    public Boost boost;

    void Update() {
        BoostBar();
    }

    private void BoostBar() {
        boosterBarLeft.fillAmount = boost.currentBoostLevel / boost.maxBoostLevel;
        boosterBarRight.fillAmount = boost.currentBoostLevel / boost.maxBoostLevel;
        boosterText.text = Mathf.RoundToInt(Mathf.Clamp(boost.currentBoostLevel, 0, boost.maxBoostLevel)).ToString() + "%";
    }
}
