using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Interactable : MonoBehaviour
{
    public enum Type
    {
        Weapon,
        Item,
        Consumable
    }
    public Type type;
    public string title;
    protected SphereCollider sphereCollider;
    protected Bounds bounds;

    public virtual void Reset()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        bounds = new Bounds(transform.position, Vector3.zero);
        foreach (Transform child in children)
        {
            Renderer rend = child.GetComponent<MeshRenderer>();
            if (rend)
            {
                bounds.Encapsulate(rend.bounds);
            }
        }

        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;

        sphereCollider.center = bounds.center - transform.position;
        sphereCollider.radius = bounds.extents.magnitude;
    }

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
    
    public virtual void Pickup()
    {
        sphereCollider.enabled = false;
    }
    public virtual void Drop()
    {
        sphereCollider.enabled = true;
    }
}
