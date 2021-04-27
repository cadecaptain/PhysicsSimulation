using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    static float G = -1f;
    float cameraShiftTolerance = 3;
    float zoomInTolerance = .4f;
    float zoomOutTolerance = .1f;
    int zoomTicks = 0;
    const int zoomDelay = 5;

    public static GameManager Instance { get; private set; }
    List<Gravity> physObjects;
    Dictionary<Gravity, GameObject> planetControllers = new Dictionary<Gravity, GameObject>();
    public Camera camera;

    public GameObject startButton, creditsButton, howToButton, volumeButton, backButton, pauseButton, menuButton;
    public GameObject titleText, creditsText, howToText;
    public GameObject volumeSlider;
    public GameObject canvas;
    public GameObject events;
    public GameObject background, howToBackground, creditsBackground;
    public GameObject cellContainer;
    public GameObject dropdown;
    public List<string> presetLevels;
    private int selectedLevel;
    public AudioMixer mixer;
    public GameObject backgroundAudio;

    public GameObject CenterOfMassPrefab;
    GameObject centerOfMassIndicator;

    public GameObject planetPrefab;
    public GameObject sunPrefab;

    public GameObject scrollView;
    public GameObject Content;
    public GameObject PlanetControllerPrefab;

    int ObjectCounter = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(canvas);
            DontDestroyOnLoad(events);
            DontDestroyOnLoad(backgroundAudio);
            dropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener((i) => changeSelectedLevel(i));

        }
        else
        {
            Destroy(gameObject);
            Destroy(canvas);
            Destroy(events);
            Destroy(backgroundAudio);
        }
    }

    public void Update()
    {
        foreach (KeyValuePair<Gravity, GameObject> keyValue in planetControllers)
        {
            Gravity g = keyValue.Key;
            GameObject pc = keyValue.Value;

            UIControllerSetup ui = pc.GetComponent<UIControllerSetup>();
            ui.massText.GetComponent<Text>().text = "Mass: " + Mathf.Round(g.GetComponent<Rigidbody2D>().mass);
            ui.posXText.GetComponent<Text>().text = "X: " + Mathf.Round(g.GetComponent<Rigidbody2D>().position.x * 100);
            ui.posYText.GetComponent<Text>().text = "Y: " + Mathf.Round(g.GetComponent<Rigidbody2D>().position.y * 100);
            ui.velocityXText.GetComponent<Text>().text = "Vx: " + Mathf.Round(g.GetComponent<Rigidbody2D>().velocity.x * 100);
            ui.velocityYText.GetComponent<Text>().text = "Vy: " + Mathf.Round(g.GetComponent<Rigidbody2D>().velocity.y * 100);
        }
    }

    void LoadScene()
    {
        physObjects = new List<Gravity>();
        camera = FindObjectOfType<Camera>();

        foreach (Gravity g in FindObjectsOfType<Gravity>())
        {
            physObjects.Add(g);
            NewPlanetController(g, ++ObjectCounter);
        }

        SetUpCoM();
    }

    void SetUpCoM() {
        centerOfMassIndicator = Instantiate(CenterOfMassPrefab, CenterOfSystem(), Quaternion.identity);
        SpriteRenderer sr = centerOfMassIndicator.gameObject.GetComponent<SpriteRenderer>();
        //CoM is slightly translucent
        sr.color = new Color(1, 1, 1, .5f);
        //and it is drawn in front of other objects
        sr.sortingOrder = 5;        
    }

    void FixedUpdate()
    {
        if (!camera) 
            LoadScene();
        CameraReposition();
        CullFaroffObjects();
        centerOfMassIndicator.transform.position = CenterOfSystem();
    }

    void CullFaroffObjects()
    {
        Vector2 c = CenterOfSystem();
        HashSet<Gravity> toRemove = new HashSet<Gravity>();
        foreach (Gravity go in physObjects) {
            if ((go.GetComponent<Rigidbody2D>().position - c).magnitude > 40) 
            {
                toRemove.Add(go);
            }
        }

        foreach (Gravity go in toRemove) {
            DestroyBody(go);
            --ObjectCounter;
        }
    }

    public void NewPlanetController(Gravity g, int i) {
        GameObject pc = Instantiate(PlanetControllerPrefab);
        pc.GetComponent<UIControllerSetup>().setup(g,i);
        pc.transform.SetParent(Content.gameObject.transform, false);
        planetControllers.Add(g, pc);
    }

    public void CreateBody(Vector2 pos, GameObject planet) {
        Gravity g = SpawnScript.SpawnNewPlanet(pos, planet);
        physObjects.Add(g);
        NewPlanetController(g, ++ObjectCounter);
    }

    public void DestroyBody(Gravity g) {
        physObjects.Remove(g);
        Destroy(g.gameObject.transform.parent.gameObject);
        Destroy(planetControllers[g]);
        planetControllers.Remove(g);
        --ObjectCounter;
    }

    public Vector2 CenterOfSystem()
    {
        Vector2 v = Vector2.zero;
        float totalMass = 0.0f;
        foreach (Gravity go in physObjects)
        {
            Vector3 p = go.gameObject.transform.position;
            v += go.GetComponent<Rigidbody2D>().mass * new Vector2(p.x, p.y);
            totalMass += go.GetComponent<Rigidbody2D>().mass;
        }
        if (physObjects.Count > 0) v /= totalMass;
        return v;
    }

    public void CameraReposition()
    {
        Vector3 center = CenterOfSystem();
        center.z = camera.transform.position.z;
        if ((camera.transform.position - center).magnitude > cameraShiftTolerance) 
            camera.transform.position = Vector3.Lerp(camera.transform.position, center, .005f);

        bool needToZoomOut = false;
        bool needToZoomIn = true;

        foreach (Gravity g in physObjects)
        {
            Vector3 v = camera.WorldToViewportPoint(g.transform.position);
            needToZoomOut |= v.x < zoomOutTolerance
                          || v.x > (1 - zoomOutTolerance)
                          || v.y < zoomOutTolerance
                          || v.y > (1 - zoomOutTolerance);

            needToZoomIn &= v.x > zoomInTolerance
                         && v.x < (1 - zoomInTolerance)
                         && v.y > zoomInTolerance
                         && v.y < (1 - zoomInTolerance);
        }

        if (needToZoomOut)
        {
            zoomTicks++;
        }
        else if (needToZoomIn)
        {
            zoomTicks--;
        }
        else { zoomTicks = 0; }

        if (zoomTicks > zoomDelay || zoomTicks < -zoomDelay)
        {
            camera.orthographicSize += .025f * Mathf.Sign(zoomTicks);
        }
    }

    public Vector2 deltaV(Gravity obj)
    {
        Vector2 c = Vector2.zero;

        foreach (Gravity go in physObjects)
        {
            Vector2 diff = obj.GetComponent<Rigidbody2D>().position - go.GetComponent<Rigidbody2D>().position;
            float rsq = diff.sqrMagnitude;
            if (go != obj)
                c += go.GetComponent<Rigidbody2D>().mass * diff.normalized / rsq;
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
        menuButton.SetActive(true);
        scrollView.SetActive(true);
        cellContainer.SetActive(true);
        StartCoroutine(LoadYourAsyncScene(presetLevels[selectedLevel]));
    }

    public void menuOnClick()
    {
        titleText.SetActive(true);
        startButton.SetActive(true);
        howToButton.SetActive(true);
        creditsButton.SetActive(true);
        background.SetActive(true);
        dropdown.SetActive(true);
        pauseButton.SetActive(false);
        menuButton.SetActive(false);
        scrollView.SetActive(false);
        cellContainer.SetActive(false);
        foreach (Transform child in Content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        planetControllers.Clear();
        StartCoroutine(LoadYourAsyncScene("Main Menu"));
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
        creditsBackground.SetActive(true);
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
        howToBackground.SetActive(true);
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
        howToBackground.SetActive(false);
        creditsBackground.SetActive(false);
    }

    public void pauseOnClick()
    {
        Time.timeScale = 1 - Time.timeScale;
    }

    public void pauseTime()
    {
        Time.timeScale = 0;
    }

    public void startTime()
    {
        Time.timeScale = 1;
    }

    public bool isPaused()
    {
        if (Time.timeScale == 0)
        {
            return true;
        }
        return false;
    }

    IEnumerator LoadYourAsyncScene(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void setVolume(float sliderValue)
    {
        mixer.SetFloat("backgroundVolume", (Mathf.Log10(sliderValue) * 20));
    }
}