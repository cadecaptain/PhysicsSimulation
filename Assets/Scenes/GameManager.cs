using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static float G = -0.01f;
    public static GameManager Instance { get; private set; }
    List<Gravity> physObjects;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
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
}
