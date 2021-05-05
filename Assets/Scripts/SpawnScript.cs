using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{




    /*
    private Vector2 sunVelocity = Vector2.zero;
    private int numberOfPlanets = 5;


    ///private float radius = 20f;

    */
    private static float minPlanetMass = .1f;
    private static float maxPlanetMass = 3f;
    //private static float spawnDelay = 15f;
    private static float maxPlanetSize = 0.2f;
    static float orbitConstant = 15;
    public static Color temp = Color.red;
    public static string planettemp;
    public static Color[] allcol = new Color[9];

    /*

    private List<GameObject> planets = new List<GameObject>();

    void Start()
    {   
        StartCoroutine(SpawnPlanets());

        sun.GetComponent<Rigidbody2D>().velocity = sunVelocity;
    }

    private IEnumerator SpawnPlanets()
    {
        for (int i = 0; i < numberOfPlanets; i++)
        {
            GameObject planet = Instantiate(planetPrefab);
            float x = 5;
            float y = 5;


            //Add below back when fixed the limits on random spawn location
            ///float x = -radius / 2f + Random.value * radius;
            ///float y = -radius / 2f + Random.value * radius;
            ///


            Vector2 position = new Vector2(x, y);
            planet.transform.position = position;
            float scale = 1f + Random.value * maxPlanetSize;
            planet.transform.localScale = new Vector2(scale, scale);

            float m = minPlanetMass + Random.value * (maxPlanetMass - minPlanetMass);
            Rigidbody2D prb = planet.GetComponent<Rigidbody2D>();
            prb.mass = m;

            //Static setting of velocity
            prb.velocity = new Vector2(3, 0);

            planets.Add(planet);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    */

    static public Gravity SpawnNewPlanet(Vector2 position, GameObject prefab)
    {
        allcol[0] = new Color(0, 0, 0, 1);
        allcol[1] = new Color(0, 0, 1, 1);
        allcol[2] = new Color(0, 1, 1, 1);
        allcol[3] = new Color(0.5f, 0.5f, 0.5f, 1);
        allcol[4] = new Color(0, 1, 0, 1);
        allcol[5] = new Color(1, 0, 1, 1);
        allcol[6] = new Color(1, 0, 0, 1);
        allcol[7] = new Color(1, 0.92f, 0.016f, 1);
        allcol[8] = new Color(1, 1, 1, 1);
        GameObject planet = Instantiate(prefab);
        planettemp = prefab.tag;
        Gravity g = planet.GetComponentInChildren<Gravity>();
        //RigidBody2D rbody = planet.GetComponentInChildren<Rigidbody2D>();
        planet.transform.position = position;
        g.transform.position = position;
        g.GetComponent<Rigidbody2D>().position = position;

        float m = minPlanetMass + (Random.value * (maxPlanetMass - minPlanetMass));
        if (prefab.CompareTag("Black Hole")) { m = 100 + Random.value * 1000; }

        g.GetComponent<Rigidbody2D>().mass = m;

        float scale = 1f + Random.value * maxPlanetSize;
        planet.transform.localScale = new Vector2(scale, scale);

        Vector2 toCenter = GameManager.Instance.deltaV(g);

        float distToCenter = (GameManager.Instance.CenterOfSystem() - position).magnitude;

        g.startVelocity =
           Quaternion.Euler(0, 0, 90) * toCenter.normalized *
           orbitConstant / Mathf.Sqrt(distToCenter);
        temp = allcol[(int)Mathf.Round(Random.Range(0, 8))];
        planet.GetComponentInChildren<SpriteRenderer>().color = temp;

        return g;
    }

    static public Color getColor()
    {
        return temp;
    }

    static public string getPrefab()
    {
        return planettemp;
    }

    private void Update()
    {

    }
}
