using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDelay : MonoBehaviour
{
    public float delay = 1f;
    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, delay);
    }
}
