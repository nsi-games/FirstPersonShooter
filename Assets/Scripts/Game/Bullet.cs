using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public GameObject effectsPrefab;
    public Transform line;

    private Rigidbody rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        line.transform.rotation = Quaternion.LookRotation(rigid.velocity);
    }

    void OnCollisionEnter(Collision col)
    {
        ContactPoint contact = col.contacts[0];
        Instantiate(effectsPrefab, contact.point, Quaternion.LookRotation(contact.normal));
        Destroy(gameObject);
    }

    public void Fire(Vector3 lineOrigin, Vector3 direction)
    {
        rigid.AddForce(direction * speed, ForceMode.Impulse);
        line.transform.position = lineOrigin;
    }
}
