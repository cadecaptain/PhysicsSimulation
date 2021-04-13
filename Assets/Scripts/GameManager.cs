using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static float G = -.1f;
    public static GameManager Instance { get; private set; }
    List<Gravity> physObjects;

    public GameObject startButton, creditsButton, howToButton, volumeButton, backButton;
    public GameObject titleText, creditsText, howToText;
    public GameObject volumeSlider;
    public GameObject canvas;
    public GameObject events;
    public GameObject background;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(canvas);
            DontDestroyOnLoad(events);
        }
        else
        {
            Destroy(gameObject);
            Destroy(canvas);
            Destroy(events);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        physObjects = new List<Gravity>();
        
        foreach (Gravity g in FindObjectsOfType<Gravity>()) {
            physObjects.Add(g);
        }

    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public Vector2 deltaV(Gravity obj) {
        Vector2 c = Vector2.zero;
        
        foreach (Gravity go in physObjects) {
            Vector2 diff = obj.rigidbody.position - go.rigidbody.position;
            float rsq = diff.sqrMagnitude;
            if (go != obj)
                c += go.rigidbody.mass * diff / rsq;
        }

        return G * c;
    }

    public void startOnClick()
    {

    }

    public void creditsOnClick()
    {
        titleText.SetActive(false);
        startButton.SetActive(false);
        howToButton.SetActive(false);
        creditsButton.SetActive(false);
        backButton.SetActive(true);
        creditsText.SetActive(true);
    }

    public void howToOnClick()
    {
        titleText.SetActive(false);
        startButton.SetActive(false);
        howToButton.SetActive(false);
        creditsButton.SetActive(false);
        backButton.SetActive(true);
        howToText.SetActive(true);
    }

    public void volumeOnClick()
    {
        if (volumeSlider.activeSelf == true)
        {
            volumeSlider.SetActive(false);
        } else
        {
            volumeSlider.SetActive(true);
        }
    }

    public void backOnClick()
    {
        titleText.SetActive(true);
        startButton.SetActive(true);
        howToButton.SetActive(true);
        creditsButton.SetActive(true);
        backButton.SetActive(false);
        creditsText.SetActive(false);
        howToText.SetActive(false);
    }
}
