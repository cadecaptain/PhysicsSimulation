using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float mass = 1;
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
        rigidbody.velocity += GameManager.Instance.deltaV(this);
       // Debug.Log(rigidbody.velocity);
    }

    private void OnDrawGizmos()
    {
        if (rigidbody)
            Gizmos.DrawRay(rigidbody.position, rigidbody.velocity);
    }
}
