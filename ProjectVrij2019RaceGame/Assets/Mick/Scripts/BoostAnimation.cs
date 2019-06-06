using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostAnimation : MonoBehaviour
{
    public Animator boostAnimation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Boost")) {
            boostAnimation.SetBool("Boost", true);
        } else {
            boostAnimation.SetBool("Boost", false);
        }
    }
}
