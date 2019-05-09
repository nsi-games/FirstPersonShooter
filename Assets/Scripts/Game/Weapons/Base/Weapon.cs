using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage = 10;
    public int maxReserve = 500;
    public int maxClip = 30;
    public float spread = 2f;
    public float recoil = 1f;
    public float range = 10f;
    public float shootRate = .2f;
    public Transform shotOrigin;
    public GameObject bulletPrefab;

    public bool canShoot = false;

    
    private int currentReserve = 0;
    private int currentClip = 0;
    private float shootTimer = 0f;

    private void Awake()
    {
        currentReserve = maxReserve;
        currentClip = maxClip;
    }

    private void Update()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootRate)
        {
            canShoot = true;
        }
    }

    public void Reload()
    { 
        if (currentReserve > 0)
        {
            if (currentReserve >= maxClip)
            {
                currentReserve -= maxClip - currentClip;
                currentClip = maxClip;
            }
            if (currentClip < maxClip)
            {
                // Note (Manny): Look into this
                int tempMag = currentReserve;
                currentClip = tempMag;
                currentReserve -= tempMag;
            }
        }
    }
    public void Shoot(Collider colliderToIgnore)
    {
        // Reduce clip
        currentClip--;
        shootTimer = 0f;
        canShoot = false;

        Camera attachedCamera = Camera.main;
        Transform camTransform = attachedCamera.transform;

        Vector3 bulletOrigin = camTransform.position;
        Quaternion bulletRotation = camTransform.rotation;

        Vector3 lineOrigin = shotOrigin.position;

        Vector3 direction = camTransform.forward;

        // Spawn bullet
        GameObject clone = Instantiate(bulletPrefab, bulletOrigin, bulletRotation);
        Collider bulletCollider = clone.GetComponent<Collider>();
        Collider playerCollider = colliderToIgnore;
        Physics.IgnoreCollision(bulletCollider, playerCollider);

        Bullet bullet = clone.GetComponent<Bullet>();
        bullet.Fire(lineOrigin, direction);
    }
}
