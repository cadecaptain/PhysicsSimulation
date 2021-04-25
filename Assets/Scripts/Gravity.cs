using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    internal Rigidbody2D rigidbody;
    public Vector2 startVelocity = Vector2.zero;
    TrailRenderer trail;
    internal bool beingDragged;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        rigidbody.velocity = startVelocity;
        SetSize();
    }

    private void OnMouseDown()
    {
        beingDragged = true;
        startVelocity = rigidbody.velocity;
    }

    private void OnMouseDrag()
    {
        rigidbody.velocity = Vector2.zero;
        Vector3 pos = GameManager.Instance.camera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        this.gameObject.transform.position = pos;

    }

    private void OnMouseUp()
    {
        beingDragged = false;
        rigidbody.velocity = startVelocity;
    }

    private void FixedUpdate()
    {
        if (!beingDragged) 
        { 
            Vector2 v = GameManager.Instance.deltaV(this);
            if (!IsInvalid(v))
                rigidbody.velocity += v;
        }
    }

    bool IsInvalid(Vector2 v) {
        return float.IsNaN(v.x) || float.IsInfinity(v.x)
            || float.IsNaN(v.y) || float.IsInfinity(v.y);
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
        SetSize();
    }
        
    public void changeTrailLength(float l)
    {
        trail.time = l;
    }

    void SetSize()
    {
        float screenSize = 1+Mathf.Log(rigidbody.mass)/8;
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
            this.rigidbody.mass += otherGrav.rigidbody.mass;
            this.rigidbody.velocity = netMomentum / this.rigidbody.mass;
            SetSize();
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