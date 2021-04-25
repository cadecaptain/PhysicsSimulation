using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerSetup : MonoBehaviour
{
    public GameObject massText;
    public GameObject massSlider;
    public GameObject trailText;
    public GameObject trailSlider;

    public void setup(Gravity g, int i) {
        massText.GetComponent<Text>().text = "Mass of Object " + i;
        Slider mSlider = massSlider.GetComponent<Slider>();
        mSlider.value = Mathf.Sqrt(g.rigidbody.mass);
        mSlider.onValueChanged.AddListener(f => g.changeMass(f * f));

        trailText.GetComponent<Text>().text = "Length of Trail "+ i;
        Slider tSlider = trailSlider.GetComponent<Slider>();
        tSlider.value = 5;
        tSlider.onValueChanged.AddListener(l => g.changeTrailLength(l));

    }

}
