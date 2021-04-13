using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static float G = -.1f;
    public static GameManager Instance { get; private set; }
    List<Gravity> physObjects;
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        physObjects = new List<Gravity>();
        camera = FindObjectOfType<Camera>();
        
        foreach (Gravity g in FindObjectsOfType<Gravity>()) {
            physObjects.Add(g);
        }

    }

    // Update is called once per frame
    void Update()
    {
        CameraReposition();
    }

    public void PauseSimulation() {
        Time.timeScale = 1-Time.timeScale;
    }

    Vector2 CenterOfSystem() {
        Vector2 v = Vector2.zero;
        foreach (Gravity go in physObjects) {
            Vector3 p = go.gameObject.transform.position;
            v += new Vector2(p.x, p.y);
        }

        return v/physObjects.Count;
    }


    void CameraReposition() {
        Vector3 center = CenterOfSystem();
        center.z = camera.transform.position.z;
        camera.transform.position = Vector3.Lerp(camera.transform.position,center,.001f);
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
}
