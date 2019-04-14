using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Weapon : MonoBehaviour
{
    public int damage = 10;
    public int maxAmmo = 500;
    public int maxClip = 30;
    public float spread = 2f;
    public float recoil = 1f;
    public float range = 10f;
    public float shootRate = .2f;
    public float lineDelay = .1f;
    public Transform shotOrigin;
    public GameObject bulletPrefab;

    private int ammo = 0;
    private int clip = 0;
    private float shootTimer = 0f;
    public bool canShoot = false;

    private LineRenderer lineRenderer;

    private List<Ray> testRays = new List<Ray>();

    #region Editor

    #endregion

    #region Internal
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var ray in testRays)
        {
            Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * range);
        }
    }
    private void GetReferences()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    private void Reset()
    {
        GetReferences();
        lineRenderer.enabled = false;
    }
    private void Start()
    {
        GetReferences();
    }
    private void Update()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootRate)
        {
            canShoot = true;
        }
    }
    IEnumerator ShowLine(Vector3 start, Vector3 end, float lineDelay)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, end);
        lineRenderer.SetPosition(1, start);
        yield return new WaitForSeconds(lineDelay);
        lineRenderer.enabled = false;
    }
    #endregion

    #region External
    public virtual void Reload()
    {
        clip += ammo;
        ammo -= maxClip;
    }
    public virtual void Shoot()
    {
        shootTimer = 0f;
        canShoot = false;

        Camera attachedCamera = Camera.main;
        Transform camTransform = attachedCamera.transform;

        Vector3 origin = camTransform.position;
        Vector3 direction = camTransform.forward;

        Vector3 start = shotOrigin.position;
        Vector3 end = start + shotOrigin.forward * range;

        Ray hitScan = new Ray(origin, direction);
        RaycastHit hit;
        if (Physics.Raycast(hitScan, out hit, range))
        {
            // Replace end with hit point
            end = hit.point;
        }
        //StartCoroutine(ShowLine(start, end, lineDelay));

        // Spawn bullet
        GameObject clone = Instantiate(bulletPrefab, shotOrigin.position, shotOrigin.rotation);
        Bullet bullet = clone.GetComponent<Bullet>();
        bullet.Fire(start, end, hit.normal);
    }
    #endregion
}
