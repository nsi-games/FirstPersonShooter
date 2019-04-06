using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Weapon : Interactable
{
    public int damage = 10;
    public int maxAmmo = 500;
    public int maxClip = 30;
    public float range = 10f;
    public float shootRate = .2f;
    public float lineDelay = .1f;
    public Transform shotOrigin;
    public GameObject bulletPrefab;

    private int ammo = 0;
    private int clip = 0;
    private float shootTimer = 0f;
    private bool canShoot = false;

    private Rigidbody rigid;
    private BoxCollider boxCollider;
    private LineRenderer lineRenderer;

    public override void Reset()
    {
        base.Reset();

        rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = false;

        boxCollider = GetComponent<BoxCollider>();
        boxCollider.center = bounds.center - transform.position;
        boxCollider.size = bounds.size;        
    }
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }
    private void Update()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootRate)
        {
            canShoot = true;
        }
    }

    public override void Pickup()
    {
        base.Pickup(); // Turns off triggers
        // Disable Rigidbody
        rigid.isKinematic = true;
    }
    public override void Drop()
    {
        base.Pickup(); // Turns on triggers
        // Enable Rigidbody
        rigid.isKinematic = false;
    }

    IEnumerator ShowLine(Ray bulletRay, float lineDelay)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, bulletRay.origin);
        lineRenderer.SetPosition(1, bulletRay.origin + bulletRay.direction * range);

        yield return new WaitForSeconds(lineDelay);
        
        lineRenderer.enabled = false;
    }

    public virtual void Reload()
    {
        clip += ammo;
        ammo -= maxClip;
    }
    public virtual void Shoot()
    {
        if (canShoot)
        {
            shootTimer = 0f;
            canShoot = false;

            GameObject clone = Instantiate(bulletPrefab, shotOrigin.position, shotOrigin.rotation);

            //Ray bulletRay = new Ray(shotOrigin.position, shotOrigin.forward);
            //RaycastHit hit;
            //// Display trails
            //StartCoroutine(ShowLine(bulletRay, lineDelay));
            //if (Physics.Raycast(bulletRay, out hit, range))
            //{
            //    Enemy enemy = hit.collider.GetComponent<Enemy>();
            //    if (enemy)
            //    {
            //        enemy.TakeDamage(damage);
            //    }
            //}
        }
    }
}
