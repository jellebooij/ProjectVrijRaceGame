using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayUI : MonoBehaviour {

    public Slider boostSlider;
    public Boost boost;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        BoostSlider();
    }

    private void BoostSlider() {
        if (boostSlider == null || boost == null) {
            return;
        }
        boostSlider.value = boost.currentBoostLevel / boost.maxBoostLevel;
    }
}
