using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    public Vector2 startVelocity = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = startVelocity;
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log(rigidbody.velocity);
    }

    private void FixedUpdate()
    {
        rigidbody.velocity += GameManager.Instance.deltaV(this);
    }

    private void OnDrawGizmos()
    {
        if (rigidbody)
            Gizmos.DrawRay(rigidbody.position, rigidbody.velocity);
    }

    public void changeMass(float m) {

        rigidbody.mass = m;
        float screenSize = Mathf.Sqrt(Mathf.Sqrt(m));
        this.gameObject.transform.localScale = Vector3.one * screenSize;


    }

}
