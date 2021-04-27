using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerSetup : MonoBehaviour
{
    public GameObject massLabel;
    public GameObject massSlider;
    public GameObject trailLabel;
    public GameObject trailSlider;
    public GameObject massText, posXText, posYText, velocityXText, velocityYText, massInputField, posXInputField, posYInputField, velocityXInputField, velocityYInputField;
    public Gravity grav;

    public void setup(Gravity g, int i) {
        grav = g;
        massLabel.GetComponent<Text>().text = "Mass of Object " + i;
        Slider mSlider = massSlider.GetComponent<Slider>();
        mSlider.value = Mathf.Sqrt(g.GetComponent<Rigidbody2D>().mass);
        mSlider.onValueChanged.AddListener(f => g.changeMass(f * f));
        massInputField.GetComponent<InputField>().onEndEdit.AddListener(f => g.changeMass(float.Parse(f)));
        posXInputField.GetComponent<InputField>().onEndEdit.AddListener(f => g.changePosX(float.Parse(f)/100));
        posYInputField.GetComponent<InputField>().onEndEdit.AddListener(f => g.changePosY(float.Parse(f)/100));
        velocityXInputField.GetComponent<InputField>().onEndEdit.AddListener(f => g.changeVelocityX(float.Parse(f)/100));
        velocityYInputField.GetComponent<InputField>().onEndEdit.AddListener(f => g.changeVelocityY(float.Parse(f)/100));

        trailLabel.GetComponent<Text>().text = "Length of Trail "+ i;
        Slider tSlider = trailSlider.GetComponent<Slider>();
        tSlider.value = 5;
        tSlider.onValueChanged.AddListener(l => g.changeTrailLength(l));

    }


}
