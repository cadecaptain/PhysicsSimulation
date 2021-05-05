using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityHandler : MonoBehaviour
{

    bool shouldRestart;
    public Rigidbody2D body;
    float stretchFactor = 10f;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void SetVector(Vector3 delta)
    {
        //Debug.Log("rebasing velocity to " + delta);
        this.gameObject.transform.rotation = Quaternion.Euler(0,0,90) * Quaternion.LookRotation(Vector3.forward, delta);
        float stretch = delta.magnitude / stretchFactor;
        if (stretch < 1) { stretch = Mathf.Max(.25f, Mathf.Sqrt(stretch)); }
        this.gameObject.transform.localScale = new Vector3(stretch, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = body.position;
        SetVector(body.velocity);
    }
    

    private void OnMouseDown()
    {
        shouldRestart = !gameManager.isPaused();
        gameManager.pauseTime();
    }

    private void OnMouseDrag()
    {
        Vector3 delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - body.transform.position;
        Vector3 v = delta * stretchFactor;
        body.velocity = v;
        SetVector(v);
    }

    private void OnMouseUp()
    {
        if (shouldRestart) gameManager.startTime();
    }
}
