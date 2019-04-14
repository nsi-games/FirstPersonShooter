using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float delay = 3f;

    private float timer = 0f;
    
    // Update is called once per frame
    void Update()
    {
        // Count up timer
        timer += Time.deltaTime;
        // If timer reaches delay
        if(timer >= delay)
        {
            // Spawn!
            GameObject clone = Instantiate(prefab, transform.position, transform.rotation, transform);
            // Activate object
            clone.SetActive(true);
            // Reset timer
            timer = 0f;
        }
    }
}
