using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class calculate the force between the objects and the vector / direction of the force.
/// https://en.wikipedia.org/wiki/Gravitational_constant
/// </summary>
public class Attractor : MonoBehaviour
{

    private SpawnScript spawnScript;

    [SerializeField] private Rigidbody2D GO_RigidBody = null;

    // The gravitational constant for the calculation.
    // https://en.wikipedia.org/wiki/Gravitational_constant
    private float G = 0.667408f;

    public static List<Attractor> attractors;

    private Vector2 direction;
    private float distance;
    private float theForce;
    private Vector2 force;

    public List<Attractor> GetAttractors()
    {
        return attractors;
    }

    private void Start()
    {
        spawnScript = GameObject.FindGameObjectWithTag("Scripts").GetComponent<SpawnScript>();
    }

    /// <summary>
    /// Call attractor method on all objects.
    /// </summary>
    private void FixedUpdate()
    {
        foreach (Attractor attractor in attractors)
        {
            if (attractor != this)
            {
                Attract(attractor);
            }
        }
    }

    /// <summary>
    /// Add this object to the list.
    /// </summary>
    private void OnEnable()
    {
        if (attractors == null)
        {
            attractors = new List<Attractor>();
        }

        attractors.Add(this);
    }

    /// <summary>
    /// Remove this object from the list.
    /// </summary>
    private void OnDisable()
    {
        attractors.Remove(this);
    }

    /// <summary>
    /// Calc force and add it to the objects rigidbody.
    /// </summary>
    /// <param name="objToAttract"></param>
    public void Attract(Attractor objToAttract)
    {
        direction = GO_RigidBody.position - objToAttract.GO_RigidBody.position;
        distance = direction.magnitude;

        if (distance == 0f)
        {
            return;
        }

        force = direction.normalized * ((GO_RigidBody.mass * objToAttract.GO_RigidBody.mass) / (distance * distance)) * G;
        objToAttract.GO_RigidBody.AddForce(force);
    }

    //Doesn't despawn the object that hits the sun
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.tag.Equals("Sun") && collision.gameObject.tag.Equals(tag) == false)
        {
            Destroy(collision.gameObject);
            if (spawnScript)
            {
                spawnScript.SpawnNewPanet();
            }
        }
    }
}
