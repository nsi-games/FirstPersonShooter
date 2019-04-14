using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
    public Transform target;
    
    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position);
        }
    }
}
