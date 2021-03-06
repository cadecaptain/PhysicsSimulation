 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    static float G = -1f;
    float cameraShiftTolerance = 3;
    float zoomInTolerance = .4f;
    float zoomOutTolerance = .1f;
    int zoomTicks = 0;
    const int zoomDelay = 5;
    const float SIM_TIME_STEP = 1 / 100;

    public static GameManager Instance { get; private set; }
    List<Gravity> physObjects;
    Dictionary<Gravity, GameObject> planetControllers = new Dictionary<Gravity, GameObject>();
    public Camera camera;
    

    public GameObject startButton, creditsButton, howToButton, volumeButton, volumeButtontemp, backButton, timeSlider, menuButton, showButton, hideButton, pauseMenu, autoCameraToggle;
    public GameObject titleText, creditsText, howToText;
    public GameObject volumeSlider;
    public GameObject canvas;
    public GameObject events;
    EventSystem eventSystem;
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

    public Color colortemp;
    public string planettemp;

    Vector2 mouseDragPos;

    int ObjectCounter = 0;
    bool AutoCamera = true;
    Vector3 startCameraPos;
    bool draggingNothing;

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
            timeSlider.GetComponent<Slider>().onValueChanged.AddListener((i) => setTimeScale(i));
            autoCameraToggle.GetComponent<Toggle>().onValueChanged.AddListener(b => setAutoCamera(b));
            Time.fixedDeltaTime = SIM_TIME_STEP;
            eventSystem = events.GetComponent<EventSystem>();
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
        if (physObjects.Count > 0)
        {
            CameraReposition();
        }
    }

    void LoadScene()
    {
        physObjects = new List<Gravity>();
        camera = FindObjectOfType<Camera>();

        foreach (Gravity g in FindObjectsOfType<Gravity>())
        {
            physObjects.Add(g);
            NewPlanetController(g, "Normal " + g.getTag());
        }

        SetUpCoM();
    }

    public string colorToString(Color color)
    {
        Vector4 temp = color;
        if (temp == new Vector4(1, 0, 0, 1))
        {
            return "Red";
        }
        else if (temp == new Vector4(0, 0, 0, 1))
        {
            return "Black";
        }
        else if (temp == new Vector4(0, 0, 1, 1))
        {
            return "Blue";
        }
        else if (temp == new Vector4(0.5f, 0.5f, 0.5f, 1))
        {
            return "Gray";
        }
        else if (temp == new Vector4(0, 1, 0, 1))
        {
            return "Green";
        }
        else if (temp == new Vector4(1, 0, 1, 1))
        {
            return "Magenta";
        }
        else if (temp == new Vector4(0, 1, 1, 1))
        {
            return "Cyan";
        }
        else if (temp == new Vector4(1, 1, 1, 1))
        {
            return "White";
        }
        else if (temp == new Vector4(1, 0.92f, 0.016f, 1))
        {
            return "Yellow";
        }
        else
            return "Normal";

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
        {
            LoadScene();
        }
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

    public void NewPlanetController(Gravity g, string name) {
        GameObject pc = Instantiate(PlanetControllerPrefab);
        pc.GetComponent<UIControllerSetup>().setup(g,name);
        pc.transform.SetParent(Content.gameObject.transform, false);
        planetControllers.Add(g, pc);
    }

    public void CreateBody(Vector2 pos, GameObject planet) {
        Gravity g = SpawnScript.SpawnNewPlanet(pos, planet);
        colortemp = SpawnScript.getColor();
        planettemp = SpawnScript.getPrefab();
        physObjects.Add(g);
        ++ObjectCounter;
        NewPlanetController(g, colorToString(colortemp) + "  " + planettemp);
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
        if (AutoCamera)
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
        else {

            if (camera.orthographicSize > .01) 
                camera.orthographicSize -= .2f * Input.mouseScrollDelta.y;

            Vector3 dragLoc = camera.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                mouseDragPos = camera.ScreenToWorldPoint(Input.mousePosition);
                startCameraPos = camera.transform.position;
                draggingNothing = !eventSystem.IsPointerOverGameObject() && !Physics2D.OverlapPoint(dragLoc);
            }

            if (Input.GetMouseButton(0) && draggingNothing)
            {
                Vector2 move = new Vector2(dragLoc.x, dragLoc.y) - mouseDragPos;
                camera.transform.position = startCameraPos - .85f * (Vector3) move;
            }
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

    public void setAutoCamera(bool c)
    {
        AutoCamera = c;
    }

    public void startOnClick()
    {
        titleText.SetActive(false);
        startButton.SetActive(false);
        howToButton.SetActive(false);
        creditsButton.SetActive(false);
        background.SetActive(false);
        dropdown.SetActive(false);
        showButton.SetActive(true);
        scrollView.SetActive(true);
        cellContainer.SetActive(true);
        Destroy(volumeButtontemp);
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
        pauseMenu.SetActive(false);
        showButton.SetActive(false);
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
        pauseMenu.SetActive(false);
        dropdown.SetActive(true);
        backButton.SetActive(false);
        creditsText.SetActive(false);
        howToText.SetActive(false);
        howToBackground.SetActive(false);
        creditsBackground.SetActive(false);
    }

    public void pauseOnClick()
    {
        setTimeScale(1 - Time.timeScale);
    }

    public void deleteOnClick(GameObject container = null)
    {
        int check1 = planetControllers.Count;
        Debug.Log("we have this many:" + check1);
        foreach (KeyValuePair<Gravity, GameObject> item in planetControllers)
        {
            if (container = item.Value)
            {
                planetControllers.Remove(item.Key);
                physObjects.Remove(item.Key);
                item.Key.delete();
                Destroy(item.Key);
                Destroy(item.Value);
                --ObjectCounter;
                break;
            }
        }
        int check2 = planetControllers.Count;
        Debug.Log("we have this many:" + check2);
    }

    public void setTimeScale(float t)
    {
        Time.timeScale = t;
        Time.fixedDeltaTime = SIM_TIME_STEP * Time.timeScale;
    }

    public void pauseTime()
    {
        setTimeScale(0);
    }

    public void startTime()
    {
        setTimeScale(1);
    }

    public bool isPaused()
    {
        if (Time.timeScale == 0)
        {
            return true;
        }
        return false;
    }

    public void menuToggle()
    {
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            showButton.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(true);
            showButton.SetActive(false);

        }
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