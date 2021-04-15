using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static float G = -1f;
    float cameraShiftTolerance = 3;
    public static GameManager Instance { get; private set; }
    List<Gravity> physObjects;
    Camera camera;

    public GameObject startButton, creditsButton, howToButton, volumeButton, backButton, pauseButton;
    public GameObject titleText, creditsText, howToText;
    public GameObject volumeSlider;
    public GameObject canvas;
    public GameObject events;
    public GameObject background;
    public GameObject dropdown;
    public List<string> presetLevels;
    private int selectedLevel;


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

    void LoadScene()
    {
        physObjects = new List<Gravity>();
        camera = FindObjectOfType<Camera>();

        foreach (Gravity g in FindObjectsOfType<Gravity>())
        {
            physObjects.Add(g);
        }

    }



    // Update is called once per frame
    void Update()
    {
        if (!camera)
            LoadScene();

        CameraReposition();
    }

    public static void PauseSimulation()
    {
        Time.timeScale = 1 - Time.timeScale;
    }

    Vector2 CenterOfSystem()
    {
        Vector2 v = Vector2.zero;
        foreach (Gravity go in physObjects)
        {
            Vector3 p = go.gameObject.transform.position;
            v += new Vector2(p.x, p.y);
        }
        if (physObjects.Count > 0) v /= physObjects.Count;
        return v;
    }


    void CameraReposition()
    {
        Vector3 center = CenterOfSystem();
        center.z = camera.transform.position.z;
        if ((camera.transform.position - center).magnitude > cameraShiftTolerance) 
            camera.transform.position = Vector3.Lerp(camera.transform.position, center, .005f);
    }

    public Vector2 deltaV(Gravity obj)
    {
        Vector2 c = Vector2.zero;

        foreach (Gravity go in physObjects)
        {
            Vector2 diff = obj.rigidbody.position - go.rigidbody.position;
            float rsq = diff.sqrMagnitude;
            if (go != obj)
                c += go.rigidbody.mass * diff.normalized / rsq;
        }

        return G * c * Time.fixedDeltaTime;
    }

    public void changeSelectedLevel(int level) 
    {
        this.selectedLevel = level;
    }

    public void startOnClick()
    {
        titleText.SetActive(false);
        startButton.SetActive(false);
        howToButton.SetActive(false);
        creditsButton.SetActive(false);
        background.SetActive(false);
        dropdown.SetActive(false);
        pauseButton.SetActive(true);
        StartCoroutine(LoadYourAsyncScene(presetLevels[selectedLevel]));
    }

    public void creditsOnClick()
    {
        titleText.SetActive(false);
        startButton.SetActive(false);
        howToButton.SetActive(false);
        creditsButton.SetActive(false);
        dropdown.SetActive(false);
        backButton.SetActive(true);
        creditsText.SetActive(true);
    }

    public void howToOnClick()
    {
        titleText.SetActive(false);
        startButton.SetActive(false);
        howToButton.SetActive(false);
        creditsButton.SetActive(false);
        dropdown.SetActive(false);
        backButton.SetActive(true);
        howToText.SetActive(true);
    }

    public void volumeOnClick()
    {
        if (volumeSlider.activeSelf == true)
        {
            volumeSlider.SetActive(false);
        }
        else
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
        dropdown.SetActive(true);
        backButton.SetActive(false);
        creditsText.SetActive(false);
        howToText.SetActive(false);
    }

    IEnumerator LoadYourAsyncScene(string scene)
    {
        Debug.Log("Loading " + scene);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}