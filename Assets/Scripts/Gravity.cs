using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    internal Rigidbody2D rigidbody;
    public Vector2 startVelocity = Vector2.zero;
    internal bool beingDragged;
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = startVelocity;
        camera = FindObjectOfType<Camera>();
    }

    private void OnMouseDown()
    {
        beingDragged = true;
    }

    private void OnMouseDrag()
    {
        rigidbody.velocity = Vector2.zero;
        rigidbody.position = camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        beingDragged = false;
    }

    private void FixedUpdate()
    {
        if (!beingDragged) rigidbody.velocity += GameManager.Instance.deltaV(this);
    }

    private void OnDrawGizmos()
    {
        if (rigidbody)
        {
            Gizmos.DrawRay(rigidbody.position, rigidbody.velocity);
            //Gizmos.DrawRay(rigidbody.position, GameManager.Instance.deltaV(this));
        }
    }


    public void changeMass(float m)
    {
        rigidbody.mass = m;
        float screenSize = Mathf.Sqrt(Mathf.Sqrt(m));
        this.gameObject.transform.localScale = Vector3.one * screenSize;
    }

    public void changeVelocity(Vector2 v)
    {
        this.rigidbody.velocity = v;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collided");
        Gravity otherGrav = collision.gameObject.GetComponent<Gravity>();

        //The collided object that survives is either the more massive one, or arbitrarily decided
        if (this.rigidbody.mass > otherGrav.rigidbody.mass ||
            (this.rigidbody.mass == otherGrav.rigidbody.mass && Compare(otherGrav)))
        {
            Vector2 netMomentum = rigidbody.velocity * rigidbody.mass + otherGrav.rigidbody.velocity * otherGrav.rigidbody.mass;
            this.rigidbody.mass = this.rigidbody.mass + otherGrav.rigidbody.mass;
            this.rigidbody.velocity = netMomentum / this.rigidbody.mass;
        } else {
            GameManager.Instance.DestroyBody(this);
        }



    }

    bool Compare(Gravity other){

        return this.gameObject.transform.position.x > other.gameObject.transform.position.x
            || this.gameObject.transform.position.y > other.gameObject.transform.position.y
            || this.gameObject.transform.position.z > other.gameObject.transform.position.z;

    }


}