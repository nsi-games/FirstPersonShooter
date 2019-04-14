using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class Bullet : MonoBehaviour
{
    public int damage = 5;

    public GameObject effectsPrefab;
    public Transform line;
    public float speed = 10f;
    public float distance = 10f;

    protected List<Vector3> trajectory = new List<Vector3>();
    protected float progress = 0f;

    protected Vector3 shotDirection;
    protected Ray bulletRay;


    protected List<Ray> rays = new List<Ray>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = line.transform.position;
        Gizmos.DrawLine(position, position + bulletRay.direction * distance);

        Gizmos.color = Color.blue;
        foreach (var ray in rays)
        {
            Gizmos.DrawRay(ray);
        }
    }

    protected virtual void Update()
    {
        Vector3 start = trajectory.First();
        Vector3 end = trajectory.Last();
        Vector3 direction = (end - start).normalized;

        // Get current Progress
        float fraction = speed / Vector3.Distance(start, end);
        progress += fraction * Time.deltaTime;

        // Move model to progress
        line.position = Vector3.Lerp(start, end, progress);
        line.rotation = Quaternion.LookRotation(direction);

        // If progress is finished
        //Ray bulletRay = new Ray(line.position, direction);
        //RaycastHit hit;
        //if(Physics.Raycast(bulletRay, out hit, distance))
        //{
        //    // Hit something!
        //    end = hit.point;
        //}

        // If finished
        if(progress >= 1f)
        {
            progress = 0f;
            RaycastHit hit;
            Ray ray = new Ray(start, direction);
            if (Physics.Raycast(ray, out hit, distance))
            {
                rays.Add(new Ray(end, -hit.normal));
                GameObject clone = Instantiate(effectsPrefab, end, Quaternion.LookRotation(hit.normal));
            }
            // Debug
            GameManager.Instance.AddHitPoint(end);
            Destroy(gameObject);
        }
    }


    private Vector3 normal;
    public virtual void Fire(Vector3 start, Vector3 end, Vector3 normal)
    {
        // Add initial point for trajectory
        trajectory.Add(start);
        trajectory.Add(end);

        this.normal = normal;
    }
}
