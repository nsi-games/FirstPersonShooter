using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class Bullet : MonoBehaviour
{
    public int damage = 5;

    public GameObject particlesPrefab;
    public Transform line;
    public float speed = 10f;
    public float distance = 10f;

    protected List<Vector3> trajectory = new List<Vector3>();
    protected float progress = 0f;

    protected Vector3 shotDirection;
    protected Ray bulletRay;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = line.transform.position;
        Gizmos.DrawLine(position, position + bulletRay.direction * distance);
    }

    protected virtual void Update()
    {
        Vector3 start = trajectory.First();
        Vector3 end = trajectory.Last();
        Vector3 direction = (end - start).normalized;

        // Get current Progress
        progress += speed * Time.deltaTime;

        // Move model to progress
        line.position = Vector3.Lerp(start, end, progress);
        line.rotation = Quaternion.LookRotation(direction);

        // If progress is finished
        Ray bulletRay = new Ray(line.position, direction);
        RaycastHit hit;
        if(Physics.Raycast(bulletRay, out hit, distance))
        {
            // Hit something!
            end = hit.point;
        }

        // If finished
        if(progress >= 1f)
        {
            // Debug
            GameManager.Instance.AddHitPoint(end);

            GameObject clone = Instantiate(particlesPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            ParticleSystem particles = clone.GetComponent<ParticleSystem>();
            particles.Play();

            Destroy(gameObject);
        }
    }

    public virtual void Fire(Vector3 start, Vector3 end)
    {
        // Add initial point for trajectory
        trajectory.Add(start);
        trajectory.Add(end);
    }
}
