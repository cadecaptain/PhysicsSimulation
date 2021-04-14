using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{

    [SerializeField] private GameObject planetPrefab = null;
    [SerializeField] private GameObject sun = null;
    [SerializeField] private GameObject centerObject = null;

    private Vector2 sunVelocity = Vector2.zero;
    private int numberOfPlanets = 5;


    ///private float radius = 20f;


    private float minPlanetMass = 5f;
    private float maxPlanetMass = 25f;
    private float spawnDelay = 15f; 
    private float maxPlanetSize = 0.2f;



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


    public void SpawnNewPanet()
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
        Rigidbody prb = planet.GetComponent<Rigidbody>();
        if (prb)
        {
            prb.mass = m;

            //Static setting of velocity
            prb.velocity = new Vector2(3, 0);
        }

        planet.GetComponent<Renderer>().material.color = new Color(0.5f + Random.value / 2, 0.5f + Random.value / 2, 0.5f + Random.value / 2);

        planets.Add(planet);
    }

    private void Update()
    {

    }
}
