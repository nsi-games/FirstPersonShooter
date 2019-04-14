using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Extras
{
    public class Projectile : MonoBehaviour
    {
        public float scale = 1f;
        public int amount = 10;
        public Vector3 force = new Vector3(0, 0, 10);
        public Vector3 gravity = new Vector3(0f, -9.7f, 0f);

        [Header("Debug")]
        public float radius = .1f;

        [Header("Bullet")]
        public Transform bullet;
        public float speed = 10f;

        private int targetwaypoint = 0;
        private Vector3 startPoint, endPoint;
        private Quaternion startRotation, endRotation;
        private float percentage = 0f;

        private List<Vector3> trajectory = new List<Vector3>();

        void Fire(Vector3 force)
        {
            // Reset list (for testing)
            trajectory = new List<Vector3>();
            // Start point from transform
            Vector3 point = transform.position;
            // Loop through 1 - amount
            for (int i = 1; i < amount; i++)
            {
                float frame = (float)i / (float)amount;
                Vector3 pull = gravity * frame;

                Vector3 prevPoint = point;
                // Percentage of current iteration
                point += (force + pull).normalized * scale;

                // Perform Raycast here
                Ray ray = new Ray(prevPoint, point - prevPoint);
                float distance = Vector3.Distance(prevPoint, point);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, distance))
                {
                    trajectory.Add(hit.point);
                    return; // Exit the function
                }

                // Add point to trajectory
                trajectory.Add(point);
            }
        }

        void DrawPoints(List<Vector3> points)
        {
            Gizmos.color = Color.blue;
            foreach (var point in points)
            {
                Gizmos.DrawSphere(point, radius);
            }
            Gizmos.color = Color.red;
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 pointA = points[i];
                Vector3 pointB = points[i + 1];
                Gizmos.DrawLine(pointA, pointB);
            }
        }

        void OnDrawGizmos()
        {
            Fire(transform.forward);
            DrawPoints(trajectory);

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(trajectory.Last(), .25f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPoint, endPoint);
        }

        void Start()
        {
            Fire(transform.forward);

            startRotation = transform.rotation;
            endRotation = transform.rotation;

            startPoint = bullet.position;
            endPoint = trajectory[targetwaypoint];
        }

        void Update()
        {
            float distance = Vector3.Distance(startPoint, endPoint);
            float duration = speed / distance;
            float fraction = duration * scale;
            percentage += fraction * Time.deltaTime;

            bullet.position = Vector3.Lerp(startPoint, endPoint, percentage);
            bullet.rotation = Quaternion.Lerp(startRotation, endRotation, percentage);

            if (percentage >= 1f)
            {
                percentage = 0f;
                // increment and wrap the target waypoint index
                targetwaypoint++;
                if (targetwaypoint < trajectory.Count)
                {
                    // assign the new lerp waypoints
                    startPoint = endPoint;
                    endPoint = trajectory[targetwaypoint];
                    // Perform Raycast here
                    Ray bulletRay = new Ray(startPoint, endPoint - startPoint);
                    float rayDistance = Vector3.Distance(startPoint, endPoint);
                    RaycastHit hit;
                    if (Physics.Raycast(bulletRay, out hit, rayDistance))
                    {
                        endPoint = hit.point;
                    }

                    startRotation = bullet.rotation;
                    endRotation = Quaternion.LookRotation(bulletRay.direction);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}