using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    internal Rigidbody2D rbody;
    public Vector2 startVelocity = Vector2.zero;
    TrailRenderer trail;
    internal bool beingDragged;
    public GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        rbody.velocity = startVelocity;
        SetSize();
    }

    private void OnMouseDown()
    {
        beingDragged = true;
        startVelocity = rbody.velocity;
    }

    private void OnMouseDrag()
    {
        rbody.velocity = Vector2.zero;
        Vector3 pos = GameManager.Instance.camera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        this.gameObject.transform.position = pos;

    }

    private void OnMouseUp()
    {
        beingDragged = false;
        rbody.velocity = startVelocity;
        if (isHoveringTrash()) GameManager.Instance.DestroyBody(this);
    }


    private bool isHoveringTrash()
    {
        PointerEventData p = new PointerEventData(EventSystem.current);
        p.position = Input.mousePosition;
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(p, hits);
        bool hoveringTrash = false;

        foreach (RaycastResult r in hits)
        {
            hoveringTrash |= r.gameObject.CompareTag("Trash");
        }

        return hoveringTrash;
    }


    private void FixedUpdate()
    {
        if (!beingDragged) 
        { 
            Vector2 v = GameManager.Instance.deltaV(this);
            if (!IsInvalid(v))
                rbody.velocity += v;
        }
    }

    bool IsInvalid(Vector2 v) {
        return float.IsNaN(v.x) || float.IsInfinity(v.x)
            || float.IsNaN(v.y) || float.IsInfinity(v.y);
    }

    private void OnDrawGizmos()
    {
        if (rbody)
        {
            Gizmos.DrawRay(rbody.position, rbody.velocity);
        }
    }


    public void changeMass(float m)
    {
        rbody.mass = m;
        SetSize();
    }

    public void changePosX(float m)
    {
        Vector3 temp = new Vector3(m, rbody.position.y, 0);
        rbody.transform.position = temp;
        SetSize();
    }

    public void changePosY(float m)
    {
        Vector3 temp = new Vector3(rbody.position.x, m, 0);
        rbody.position = temp;
        SetSize();
    }

    public void changeVelocityX(float m)
    {
        Vector3 temp = new Vector3(m, rbody.velocity.y, 0);
        rbody.velocity = temp;
        SetSize();
    }

    public void changeVelocityY(float m)
    {
        Vector3 temp = new Vector3(rbody.velocity.x, m, 0);
        rbody.velocity = temp;
        SetSize();
    }


    public void changeTrailLength(float l)
    {
        trail.time = l;
    }

    void SetSize()
    {
        float screenSize = 1+Mathf.Log(rbody.mass)/8;
        this.gameObject.transform.localScale = Vector3.one * screenSize;
    }

    public void changeVelocity(Vector2 v)
    {
        this.rbody.velocity = v;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("trigger collision");
        Gravity otherGrav = collision.gameObject.GetComponent<Gravity>();

        //The collided object that survives is either the more massive one, or arbitrarily decided
        if (this.rbody.mass > otherGrav.rbody.mass ||
            (this.rbody.mass == otherGrav.rbody.mass && Compare(otherGrav)))
        {
            Vector2 netMomentum = rbody.velocity * rbody.mass + otherGrav.rbody.velocity * otherGrav.rbody.mass;
            this.rbody.mass += otherGrav.rbody.mass;
            this.rbody.velocity = netMomentum / this.rbody.mass;
            SetSize();
        } else {
            GameManager.Instance.DestroyBody(this);
            GameObject e = Instantiate(explosion) as GameObject;
            e.transform.position = transform.position;
            e.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].wrapMode = WrapMode.Once;
        }

    }

    bool Compare(Gravity other){

        return this.gameObject.transform.position.x > other.gameObject.transform.position.x
            || this.gameObject.transform.position.y > other.gameObject.transform.position.y
            || this.gameObject.transform.position.z > other.gameObject.transform.position.z;

    }


}